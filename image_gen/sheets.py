import os, re, json, hashlib
from collections import defaultdict
import gspread
from google.oauth2.service_account import Credentials

SCRIPT_DIR         = os.path.dirname(os.path.abspath(__file__))
PARENT             = os.path.join(SCRIPT_DIR, "..")
PLACEHOLDERS_FILE  = os.path.join(SCRIPT_DIR, ".placeholders.json")
SERVICE_ACCOUNT    = os.path.join(SCRIPT_DIR, "service_account.json")
SHEET_ID           = "1adgDbqi4A7oHqtAb2klUFrUsl4-TQR_AIWqDdDQUQ1g"
GITHUB_RAW         = "https://raw.githubusercontent.com/lamali292/Downfall/main/image_gen"
SHEETS_CACHE_FILE  = os.path.join(SCRIPT_DIR, ".sheets_cache.json")

# Formatting Constants
ROW_HEIGHT_PX      = 130
IMG_COL_PX         = 170
COL_MISSING_BG     = {"red": 0.99, "green": 0.80, "blue": 0.80}
COL_PLACEHOLDER_BG = {"red": 1.00, "green": 0.95, "blue": 0.70}
COL_FINAL_BG       = {"red": 0.80, "green": 0.95, "blue": 0.80}
COL_HEADER_BG      = {"red": 0.18, "green": 0.18, "blue": 0.25}
COL_HEADER_FG      = {"red": 1.0,  "green": 1.0,  "blue": 1.0}
COL_TAB_MISSING    = {"red": 0.85, "green": 0.27, "blue": 0.27}
COL_WHITE          = {"red": 1.0,  "green": 1.0,  "blue": 1.0}

HEADER = ["Status", "Rarity", "Character", "File Name", "Title", "Description", "Image"]

# ── Helpers ───────────────────────────────────────────────────

def to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

def normalize(name, is_power_type=False):
    snake = to_snake(name)
    if is_power_type:
        snake = re.sub(r'_power$', '', snake)
    return snake.replace("_", "")

def clean_desc(s):
    if not s: return ""
    s = re.sub(r'\{[^}]+\}', '', s)
    s = re.sub(r'\[#?[0-9a-fA-F]{6}\]|\[/?b\]|\[nl\]', ' ', s, flags=re.IGNORECASE)
    s = re.sub(r'\[/?[^\]]+\]', '', s)
    return s.replace('\n', ' ').strip()

def file_hash(path):
    if not os.path.exists(path): return ""
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()

def rows_hash(rows):
    return hashlib.md5(json.dumps(rows, sort_keys=True).encode()).hexdigest()

def load_cache(path):
    if os.path.exists(path):
        print(f"  [cache] Loaded: {path}")
        return json.load(open(path))
    print(f"  [cache] Not found, starting fresh: {path}")
    return {}

# ── Character Discovery (mirrors fill_missing.py) ─────────────

def discover_characters():
    """Returns {char_id: {code_dir, project, project_path}} by scanning PARENT for *Code folders."""
    characters = {}
    for project in os.listdir(PARENT):
        project_path = os.path.join(PARENT, project)
        if not os.path.isdir(project_path): continue
        for entry in os.listdir(project_path):
            if entry.endswith("Code") and os.path.isdir(os.path.join(project_path, entry)):
                char_id = entry[:-4].lower()
                characters[char_id] = {
                    "code_dir":     os.path.join(project_path, entry),
                    "project":      project,
                    "project_path": project_path,
                }
    return characters

# ── Placeholder Detection ─────────────────────────────────────

print("[1/7] Loading placeholder cache...")
placeholders = load_cache(PLACEHOLDERS_FILE)
print(f"  {len(placeholders)} placeholder entries loaded.")

def is_placeholder(rel_path, abs_path):
    return placeholders.get(rel_path) == file_hash(abs_path)

# ── Image Indexing ────────────────────────────────────────────

def index_images(subdirs, is_power_type):
    """
    Indexes images from image_gen/{subdir}/{char_id}/*.png
    Returns {norm_name: (status, subdir_name, rel_path_from_subdir_root)}
    """
    index = {}
    for subdir_name in subdirs:
        subdir = os.path.join(SCRIPT_DIR, subdir_name)
        if not os.path.exists(subdir):
            print(f"    [skip] Directory not found: {subdir}")
            continue
        found = 0
        for root, _, files in os.walk(subdir):
            for file in files:
                if not file.lower().endswith(".png"): continue
                abs_p = os.path.join(root, file)
                rel_p = os.path.relpath(abs_p, SCRIPT_DIR)
                stem  = os.path.splitext(file)[0]
                norm  = normalize(stem, is_power_type)

                status = "final"
                if "beta" in subdir_name: status = "placeholder"
                if "missing" in subdir_name or is_placeholder(rel_p, abs_p):
                    status = "missing"

                if norm in index:
                    if index[norm][0] == "final": continue
                    if index[norm][0] == "placeholder" and status == "missing": continue
                index[norm] = (status, subdir_name, os.path.relpath(abs_p, subdir))
                found += 1
        print(f"    [images] {subdir_name}: {found} PNGs indexed.")
    print(f"  Total unique image keys: {len(index)}")
    return index

# ── Collect Code ──────────────────────────────────────────────

def collect_code(characters, asset_type, is_power_type):
    """
    asset_type: 'Cards' or 'Powers'
    Returns {char_id: {norm: (snake, rarity, loc_prefix)}}
    loc_prefix is e.g. 'HEXAGHOST' derived from the project name.
    """
    all_entries = defaultdict(dict)
    total = 0
    for char_id, info in sorted(characters.items()):
        base_path  = os.path.join(info["code_dir"], asset_type)
        loc_prefix = info["project"].upper()
        if not os.path.exists(base_path):
            print(f"    [skip] No {asset_type} folder for {char_id}: {base_path}")
            continue
        char_count = 0
        for root, dirs, files in os.walk(base_path):
            dirs[:] = [d for d in dirs if d != "Abstract"]
            rel   = os.path.relpath(root, base_path)
            parts = rel.split(os.sep)
            rarity = parts[0].lower() if rel != "." else ""
            for file in files:
                if not file.endswith(".cs"): continue
                name  = os.path.splitext(file)[0]
                norm  = normalize(name, is_power_type)
                snake = to_snake(name)
                if is_power_type:
                    snake = re.sub(r'_power$', '', snake)
                all_entries[char_id][norm] = (snake, rarity, loc_prefix)
                char_count += 1
                total += 1
        print(f"    [{char_id}] {char_count} {asset_type.lower()} .cs files found. (prefix: {loc_prefix}-)")
    print(f"  Total: {total} {asset_type.lower()} entries across {len(all_entries)} characters.")
    return all_entries

# ── Localization Loader ───────────────────────────────────────

def load_loc_for_characters(characters, filename):
    """
    Searches each project for localization/eng/{filename}.
    Tries {project_path}/{project}/localization/eng/ then {project_path}/localization/eng/
    Merges all found into one dict.
    """
    merged = {}
    for char_id, info in sorted(characters.items()):
        project      = info["project"]
        project_path = info["project_path"]
        candidates = [
            os.path.join(project_path, project, "localization", "eng", filename),
            os.path.join(project_path, "localization", "eng", filename),
        ]
        for path in candidates:
            if os.path.exists(path):
                with open(path, encoding="utf-8") as f:
                    data = json.load(f)
                merged.update(data)
                print(f"    [{char_id}] {len(data)} keys from {os.path.relpath(path, PARENT)}")
                break
        else:
            print(f"    [{char_id}] No {filename} localization found.")
    print(f"  Total merged localization keys: {len(merged)}")
    return merged

# ── Build Rows ────────────────────────────────────────────────

def build_rows(entries_by_char, index, loc_data, is_power_type):
    label = "powers" if is_power_type else "cards"
    print(f"  [{label}] build_rows called with {len(entries_by_char)} characters: {list(entries_by_char.keys())}")
    total_entries = sum(len(v) for v in entries_by_char.values())
    print(f"  [{label}] Total entries across all chars: {total_entries}")
    rows = []
    skipped_final = 0
    missing_loc = []
    for char_id in sorted(entries_by_char.keys()):
        entries = entries_by_char[char_id]
        for norm in sorted(entries.keys()):
            snake, rarity, loc_prefix = entries[norm]
            if not is_power_type and not rarity: continue
            entry = index.get(norm)

            status = entry[0].upper() if entry else "MISSING"
            if status == "FINAL":
                skipped_final += 1
                continue

            url = ""
            if entry:
                _, sub, rel_img = entry
                url = f'=IMAGE("{GITHUB_RAW}/{sub}/{rel_img.replace(os.sep, "/")}")'

            loc_key = snake.upper()
            if is_power_type and not loc_key.endswith("_POWER"):
                loc_key += "_POWER"

            title = loc_data.get(f"{loc_prefix}-{loc_key}.title", "")
            desc  = loc_data.get(f"{loc_prefix}-{loc_key}.description", "")

            if not title and not desc:
                missing_loc.append(f"{loc_prefix}-{loc_key}")

            rarity_display = rarity.capitalize() if rarity else "—"
            rows.append([status, rarity_display, char_id, snake, title, clean_desc(desc), url])

    print(f"  {len(rows)} rows to sync ({skipped_final} FINAL skipped).")
    if missing_loc:
        print(f"  [warn] {len(missing_loc)} entries with no localization data:")
        for k in missing_loc[:10]:
            print(f"    - {k}")
        if len(missing_loc) > 10:
            print(f"    ... and {len(missing_loc) - 10} more.")
    return rows

# ── Run ───────────────────────────────────────────────────────

print("\n[1/7] Discovering characters...")
characters = discover_characters()
print(f"  Found {len(characters)} character(s): {', '.join(sorted(characters.keys()))}")
for char_id, info in sorted(characters.items()):
    print(f"    {char_id} -> {os.path.relpath(info['code_dir'], PARENT)}")

print("\n[2/7] Indexing card images...")
card_img_idx  = index_images(["cards", "cards_beta", "cards_missing"], False)

print("\n[3/7] Indexing power images...")
power_img_idx = index_images(["powers", "powers_beta", "powers_missing"], True)

print("\n[4/7] Collecting card code...")
cards_code  = collect_code(characters, "Cards", False)

print("\n[5/7] Collecting power code...")
powers_code = collect_code(characters, "Powers", True)

print("\n[6/7] Loading localization...")
card_loc_raw  = load_loc_for_characters(characters, "cards.json")
power_loc_raw = load_loc_for_characters(characters, "powers.json")

print("\nBuilding card rows...")
all_card_work  = build_rows(cards_code,  card_img_idx,  card_loc_raw,  False)

print("Building power rows...")
all_power_work = build_rows(powers_code, power_img_idx, power_loc_raw, True)

# ── Sheets Sync ───────────────────────────────────────────────

print("\n[7/7] Connecting to Google Sheets...")
creds = Credentials.from_service_account_file(SERVICE_ACCOUNT, scopes=[
    "https://www.googleapis.com/auth/spreadsheets",
    "https://www.googleapis.com/auth/drive"
])
sh    = gspread.authorize(creds).open_by_key(SHEET_ID)
print("  Connected.")

cache = load_cache(SHEETS_CACHE_FILE)

def sync_sheet(sh, cache, name, rows, color):
    h = rows_hash(rows)
    if cache.get(name) == h:
        print(f"  [{name}] No changes, skipping.")
        return

    print(f"  [{name}] Changes detected, updating {len(rows)} rows...")
    try:
        ws = sh.worksheet(name)
        print(f"  [{name}] Found existing worksheet.")
    except:
        ws = sh.add_worksheet(title=name, rows=max(1000, len(rows)+10), cols=7)
        print(f"  [{name}] Created new worksheet.")

    print(f"  [{name}] Clearing sheet...")
    ws.clear()

    data = [HEADER] + rows if rows else [HEADER, ["All caught up!"]]
    print(f"  [{name}] Writing {len(data)} rows (including header)...")
    ws.update(data, "A1", value_input_option="USER_ENTERED")

    if rows:
        print(f"  [{name}] Applying dimension/tab formatting...")
        reqs = [
            {"updateDimensionProperties": {"range": {"sheetId": ws.id, "dimension": "ROWS", "startIndex": 1, "endIndex": len(rows)+1}, "properties": {"pixelSize": ROW_HEIGHT_PX}, "fields": "pixelSize"}},
            {"updateDimensionProperties": {"range": {"sheetId": ws.id, "dimension": "COLUMNS", "startIndex": 6, "endIndex": 7}, "properties": {"pixelSize": IMG_COL_PX}, "fields": "pixelSize"}},
            {"updateSheetProperties": {"properties": {"sheetId": ws.id, "tabColorStyle": {"rgbColor": color}}, "fields": "tabColorStyle"}}
        ]
        sh.batch_update({"requests": reqs})

        print(f"  [{name}] Applying row color formatting...")
        formats = []
        for i, row in enumerate(rows, start=2):
            if row[0] == "MISSING":        status_bg = COL_MISSING_BG
            elif row[0] == "PLACEHOLDER":  status_bg = COL_PLACEHOLDER_BG
            else:                          status_bg = COL_FINAL_BG

            formats.append({"range": f"A{i}", "format": {
                "backgroundColor": status_bg,
                "verticalAlignment": "MIDDLE",
                "horizontalAlignment": "CENTER"
            }})
            formats.append({"range": f"B{i}:F{i}", "format": {
                "backgroundColor": COL_WHITE,
                "verticalAlignment": "MIDDLE"
            }})
            formats.append({"range": f"G{i}", "format": {
                "backgroundColor": COL_WHITE,
                "verticalAlignment": "MIDDLE"
            }})

        ws.batch_format(formats)
        print(f"  [{name}] Formatting done.")

    cache[name] = h
    print(f"  [{name}] Done.")

print("\nSyncing sheets...")
sync_sheet(sh, cache, "⚠ Needs Work (Cards)",  all_card_work,  COL_TAB_MISSING)
sync_sheet(sh, cache, "⚠ Needs Work (Powers)", all_power_work, COL_TAB_MISSING)

print("\nSaving cache...")
with open(SHEETS_CACHE_FILE, "w") as f:
    json.dump(cache, f, indent=2)

print("\nDone!")