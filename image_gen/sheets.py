import os, re, json, hashlib
from collections import defaultdict
import gspread
from google.oauth2.service_account import Credentials
import time

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

ROW_HEIGHT_PX  = 130
IMG_COL_PX     = 170
HEADER_ROW_PX  = 30

# Colours
COL_MISSING_BG     = {"red": 0.99, "green": 0.80, "blue": 0.80}  # red
COL_PLACEHOLDER_BG = {"red": 1.00, "green": 0.95, "blue": 0.70}  # yellow
COL_DONE_BG        = {"red": 0.85, "green": 0.95, "blue": 0.85}  # green
COL_HEADER_BG      = {"red": 0.18, "green": 0.18, "blue": 0.25}
COL_HEADER_FG      = {"red": 1.0,  "green": 1.0,  "blue": 1.0}
COL_TAB_MISSING    = {"red": 0.85, "green": 0.27, "blue": 0.27}
COL_WHITE          = {"red": 1.0,  "green": 1.0,  "blue": 1.0}

HEADER = ["Status", "Folder", "File Name", "Title", "Description", "Image"]

# ── Helpers ───────────────────────────────────────────────────

def to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

def normalize(name):
    return to_snake(name).replace("_", "")

def file_hash(path):
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()

def rows_hash(rows):
    return hashlib.md5(json.dumps(rows, sort_keys=True).encode()).hexdigest()

def load_cache(path):
    return json.load(open(path)) if os.path.exists(path) else {}

def save_cache(path, data):
    with open(path, "w") as f:
        json.dump(data, f, indent=2)

def load_loc(path, suffix=""):
    data = {}
    if not os.path.exists(path): return data
    with open(path, encoding="utf-8") as f:
        raw = json.load(f)
    for key, value in raw.items():
        key   = key.removeprefix("DOWNFALL-")
        parts = key.split(".", 1)
        name  = parts[0]
        field = parts[1] if len(parts) > 1 else ""
        if suffix:
            name = re.sub(rf'_{re.escape(suffix)}$', '', name, flags=re.IGNORECASE)
        norm = normalize(name)
        if norm not in data: data[norm] = {}
        data[norm][field] = value
    return data

def clean_desc(s):
    return re.sub(r'\[/?[^\]]+\]', '', s).strip()

# ── Placeholder Detection ─────────────────────────────────────

placeholders = load_cache(PLACEHOLDERS_FILE)

def is_placeholder(rel_path, abs_path):
    # If it's in the .placeholders.json and the hash matches, it's a placeholder
    return placeholders.get(rel_path) == file_hash(abs_path)

# ── Image Indexing ────────────────────────────────────────────

def index_images(subdirs):
    index = {}
    for subdir_name in subdirs:
        subdir = os.path.join(SCRIPT_DIR, subdir_name)
        if not os.path.exists(subdir): continue
        for root, _, files in os.walk(subdir):
            for file in files:
                if not file.lower().endswith(".png"): continue
                abs_path = os.path.join(root, file)
                rel_path = os.path.relpath(abs_path, SCRIPT_DIR)
                
                stem = os.path.splitext(file)[0]
                stem = re.sub(r'_power$', '', stem, flags=re.IGNORECASE)
                norm = normalize(stem)
                
                # Check status
                status = "final"
                if "beta" in subdir_name: status = "placeholder"
                if "missing" in subdir_name or is_placeholder(rel_path, abs_path):
                    status = "missing"
                
                # Priority: final > placeholder > missing
                if norm in index:
                    current_status = index[norm][0]
                    if current_status == "final": continue
                    if current_status == "placeholder" and status == "missing": continue

                index[norm] = (status, subdir_name, os.path.relpath(abs_path, subdir))
    return index

card_image_index = index_images(["cards", "cards_beta", "cards_missing"])
power_image_index = index_images(["powers", "powers_beta", "powers_missing"])

def get_image_url(index, stem, subdir_override=None):
    entry = index.get(normalize(stem))
    if not entry: return ""
    _, subdir_name, rel_in_subdir = entry
    url = f"{GITHUB_RAW}/{subdir_name}/{rel_in_subdir.replace(os.sep, '/')}"
    return f'=IMAGE("{url}")'

# ── Collect Code ──────────────────────────────────────────────

def collect_code(base_path, strip_power=False):
    entries = defaultdict(dict)
    for root, dirs, files in os.walk(base_path):
        dirs[:] = [d for d in dirs if d != "Abstract"]
        folder = normalize(os.path.relpath(root, base_path).split(os.sep)[0])
        for file in files:
            if file.endswith(".cs"):
                snake = to_snake(os.path.splitext(file)[0])
                if strip_power: snake = re.sub(r'_power$', '', snake)
                entries[folder][normalize(snake)] = snake
    return entries

cards = collect_code(card_base)
powers = collect_code(power_base, strip_power=True)
card_locs = load_loc(card_loc)
power_locs = load_loc(power_loc, suffix="power")

# ── Build Rows ────────────────────────────────────────────────

def build_rows(entries, index, loc_data):
    all_needs_work = []
    for folder in sorted(entries.keys()):
        for norm in sorted(entries[folder].keys()):
            snake = entries[folder][norm]
            entry = index.get(norm)
            status = entry[0].upper() if entry else "MISSING"
            
            url = get_image_url(index, snake)
            loc = loc_data.get(norm, {})
            
            if status != "DONE":
                all_needs_work.append([
                    status, folder, snake, 
                    loc.get("title", ""), 
                    clean_desc(loc.get("description", "")), 
                    url
                ])
    return all_needs_work

all_card_work = build_rows(cards, card_image_index, card_locs)
all_power_work = build_rows(powers, power_image_index, power_locs)

# ── Sheets Sync ───────────────────────────────────────────────

def sync_sheet(sh, cache, name, rows, color):
    h = rows_hash(rows)
    if cache.get(name) == h:
        print(f"  {name}: No changes.")
        return
    
    try:
        ws = sh.worksheet(name)
    except:
        ws = sh.add_worksheet(title=name, rows=max(1000, len(rows)+10), cols=6)
    
    ws.clear()
    data = [HEADER] + rows
    ws.update(data, "A1", value_input_option="USER_ENTERED")
    
    # Formatting
    sh.batch_update({"requests": [
        {"updateDimensionProperties": {"range": {"sheetId": ws.id, "dimension": "ROWS", "startIndex": 0, "endIndex": 1}, "properties": {"pixelSize": HEADER_ROW_PX}, "fields": "pixelSize"}},
        {"updateDimensionProperties": {"range": {"sheetId": ws.id, "dimension": "ROWS", "startIndex": 1, "endIndex": len(rows)+1}, "properties": {"pixelSize": ROW_HEIGHT_PX}, "fields": "pixelSize"}},
        {"updateDimensionProperties": {"range": {"sheetId": ws.id, "dimension": "COLUMNS", "startIndex": 5, "endIndex": 6}, "properties": {"pixelSize": IMG_COL_PX}, "fields": "pixelSize"}},
        {"updateSheetProperties": {"properties": {"sheetId": ws.id, "tabColorStyle": {"rgbColor": color}}, "fields": "tabColorStyle"}}
    ]})
    
    formats = [{"range": "A1:F1", "format": {"backgroundColor": COL_HEADER_BG, "textFormat": {"bold": True, "foregroundColor": COL_HEADER_FG}, "verticalAlignment": "MIDDLE"}}]
    for i, row in enumerate(rows, start=2):
        bg = COL_MISSING_BG if row[0] == "MISSING" else COL_PLACEHOLDER_BG
        formats.append({"range": f"A{i}:E{i}", "format": {"backgroundColor": bg, "verticalAlignment": "MIDDLE"}})
        formats.append({"range": f"F{i}", "format": {"backgroundColor": COL_WHITE, "verticalAlignment": "MIDDLE"}})
    ws.batch_format(formats)
    cache[name] = h

# ── Run ───────────────────────────────────────────────────────

creds = Credentials.from_service_account_file(SERVICE_ACCOUNT, scopes=["https://www.googleapis.com/auth/spreadsheets", "https://www.googleapis.com/auth/drive"])
sh = gspread.authorize(creds).open_by_key(SHEET_ID)
cache = load_cache(SHEETS_CACHE_FILE)

print("Syncing...")
sync_sheet(sh, cache, "⚠ Needs Work (Cards)", all_card_work, COL_TAB_MISSING)
sync_sheet(sh, cache, "⚠ Needs Work (Powers)", all_power_work, COL_TAB_MISSING)

save_cache(SHEETS_CACHE_FILE, cache)
print(f"Done: https://docs.google.com/spreadsheets/d/{SHEET_ID}")