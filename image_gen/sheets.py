import os, re, json, hashlib
from collections import defaultdict
import gspread
from google.oauth2.service_account import Credentials

SCRIPT_DIR         = os.path.dirname(os.path.abspath(__file__))
card_base          = os.path.join(SCRIPT_DIR, "..", "Code", "Cards")
power_base         = os.path.join(SCRIPT_DIR, "..", "Code", "Powers")
card_loc           = os.path.join(SCRIPT_DIR, "..", "Downfall", "localization", "eng", "cards.json")
power_loc          = os.path.join(SCRIPT_DIR, "..", "Downfall", "localization", "eng", "powers.json")
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

HEADER = ["Status", "Rarity", "Folder", "File Name", "Title", "Description", "Image"]

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
    return json.load(open(path)) if os.path.exists(path) else {}

# ── Placeholder Detection ─────────────────────────────────────

placeholders = load_cache(PLACEHOLDERS_FILE)

def is_placeholder(rel_path, abs_path):
    return placeholders.get(rel_path) == file_hash(abs_path)

# ── Image Indexing ────────────────────────────────────────────

def index_images(subdirs, is_power_type):
    index = {}
    for subdir_name in subdirs:
        subdir = os.path.join(SCRIPT_DIR, subdir_name)
        if not os.path.exists(subdir): continue
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
    return index

# ── Collect Code ──────────────────────────────────────────────

def collect_code(base_path, is_power_type):
    entries = defaultdict(dict)
    for root, dirs, files in os.walk(base_path):
        dirs[:] = [d for d in dirs if d != "Abstract"]
        rel   = os.path.relpath(root, base_path)
        parts = rel.split(os.sep)
        character = parts[0].lower() if rel != "." else ""
        rarity    = parts[1].lower() if len(parts) > 1 else ""
        for file in files:
            if file.endswith(".cs"):
                name = os.path.splitext(file)[0]
                entries[character][normalize(name, is_power_type)] = (to_snake(name), rarity)
    return entries

# ── Localization Loader ───────────────────────────────────────

def load_loc(path):
    if not os.path.exists(path): return {}
    with open(path, encoding="utf-8") as f:
        return json.load(f)

# ── Build Rows ────────────────────────────────────────────────

def build_rows(entries, index, loc_data, is_power_type):
    rows = []
    for folder in sorted(entries.keys()):
        for norm in sorted(entries[folder].keys()):
            snake, rarity = entries[folder][norm]
            entry = index.get(norm)

            status = entry[0].upper() if entry else "MISSING"
            if status == "FINAL": continue

            url = ""
            if entry:
                _, sub, rel_img = entry
                url = f'=IMAGE("{GITHUB_RAW}/{sub}/{rel_img.replace(os.sep, "/")}")'

            loc_key = snake.upper()
            if is_power_type:
                if not loc_key.endswith("_POWER"):
                    loc_key += "_POWER"

            title = loc_data.get(f"DOWNFALL-{loc_key}.title", "")
            desc  = loc_data.get(f"DOWNFALL-{loc_key}.description", "")

            rarity_display = rarity.capitalize() if rarity else "—"
            rows.append([status, rarity_display, folder, snake, title, clean_desc(desc), url])
    return rows

# ── Run ───────────────────────────────────────────────────────

card_img_idx  = index_images(["cards", "cards_beta", "cards_missing"], False)
power_img_idx = index_images(["powers", "powers_beta", "powers_missing"], True)

cards_code  = collect_code(card_base, False)
powers_code = collect_code(power_base, True)

card_loc_raw  = load_loc(card_loc)
power_loc_raw = load_loc(power_loc)

all_card_work  = build_rows(cards_code, card_img_idx, card_loc_raw, False)
all_power_work = build_rows(powers_code, power_img_idx, power_loc_raw, True)

# ── Sheets Sync ───────────────────────────────────────────────

creds = Credentials.from_service_account_file(SERVICE_ACCOUNT, scopes=[
    "https://www.googleapis.com/auth/spreadsheets",
    "https://www.googleapis.com/auth/drive"
])
sh    = gspread.authorize(creds).open_by_key(SHEET_ID)
cache = load_cache(SHEETS_CACHE_FILE)

def sync_sheet(sh, cache, name, rows, color):
    h = rows_hash(rows)
    if cache.get(name) == h:
        print(f"  {name}: No changes.")
        return

    print(f"  {name}: Updating...")
    try:
        ws = sh.worksheet(name)
    except:
        ws = sh.add_worksheet(title=name, rows=max(1000, len(rows)+10), cols=7)

    ws.clear()
    data = [HEADER] + rows if rows else [HEADER, ["All caught up!"]]
    ws.update(data, "A1", value_input_option="USER_ENTERED")

    if rows:
        reqs = [
            {"updateDimensionProperties": {"range": {"sheetId": ws.id, "dimension": "ROWS", "startIndex": 1, "endIndex": len(rows)+1}, "properties": {"pixelSize": ROW_HEIGHT_PX}, "fields": "pixelSize"}},
            {"updateDimensionProperties": {"range": {"sheetId": ws.id, "dimension": "COLUMNS", "startIndex": 6, "endIndex": 7}, "properties": {"pixelSize": IMG_COL_PX}, "fields": "pixelSize"}},
            {"updateSheetProperties": {"properties": {"sheetId": ws.id, "tabColorStyle": {"rgbColor": color}}, "fields": "tabColorStyle"}}
        ]
        sh.batch_update({"requests": reqs})

        formats = []
        for i, row in enumerate(rows, start=2):
            if row[0] == "MISSING":
                status_bg = COL_MISSING_BG
            elif row[0] == "PLACEHOLDER":
                status_bg = COL_PLACEHOLDER_BG
            else:
                status_bg = COL_FINAL_BG

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

    cache[name] = h

sync_sheet(sh, cache, "⚠ Needs Work (Cards)",  all_card_work,  COL_TAB_MISSING)
sync_sheet(sh, cache, "⚠ Needs Work (Powers)", all_power_work, COL_TAB_MISSING)

with open(SHEETS_CACHE_FILE, "w") as f:
    json.dump(cache, f, indent=2)

print("Done!")