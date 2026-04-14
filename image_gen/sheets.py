import os, re, json, hashlib
from collections import defaultdict
import gspread
from google.oauth2.service_account import Credentials
import time

SCRIPT_DIR        = os.path.dirname(os.path.abspath(__file__))
card_base         = os.path.join(SCRIPT_DIR, "..", "Code", "Cards")
power_base        = os.path.join(SCRIPT_DIR, "..", "Code", "Powers")
power_img_base    = os.path.join(SCRIPT_DIR, "powers")
card_loc          = os.path.join(SCRIPT_DIR, "..", "Downfall", "localization", "eng", "cards.json")
power_loc         = os.path.join(SCRIPT_DIR, "..", "Downfall", "localization", "eng", "powers.json")
PLACEHOLDERS_FILE = os.path.join(SCRIPT_DIR, ".placeholders.json")
SERVICE_ACCOUNT   = os.path.join(SCRIPT_DIR, "service_account.json")
SHEET_ID          = "1adgDbqi4A7oHqtAb2klUFrUsl4-TQR_AIWqDdDQUQ1g"
GITHUB_RAW        = "https://raw.githubusercontent.com/lamali292/Downfall/main/image_gen"
SHEETS_CACHE_FILE = os.path.join(SCRIPT_DIR, ".sheets_cache.json")

ROW_HEIGHT_PX  = 130
IMG_COL_PX     = 170
HEADER_ROW_PX  = 30

CARD_DIRS = [
    os.path.join(SCRIPT_DIR, "cards"),
    os.path.join(SCRIPT_DIR, "cards_beta"),
    os.path.join(SCRIPT_DIR, "cards_missing"),
]

# Colours
COL_MISSING_BG  = {"red": 0.99, "green": 0.80, "blue": 0.80}
COL_BETA_BG     = {"red": 1.00, "green": 0.95, "blue": 0.70}
COL_DONE_BG     = {"red": 0.85, "green": 0.95, "blue": 0.85}
COL_HEADER_BG   = {"red": 0.18, "green": 0.18, "blue": 0.25}
COL_HEADER_FG   = {"red": 1.0,  "green": 1.0,  "blue": 1.0}
COL_TAB_MISSING = {"red": 0.85, "green": 0.27, "blue": 0.27}
COL_TAB_CHAMP   = {"red": 0.20, "green": 0.40, "blue": 0.65}
COL_TAB_POWER   = {"red": 0.30, "green": 0.55, "blue": 0.35}
COL_WHITE       = {"red": 1.0,  "green": 1.0,  "blue": 1.0}

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
        if norm not in data:
            data[norm] = {}
        data[norm][field] = value
    return data

def clean_desc(s):
    return re.sub(r'\[/?[^\]]+\]', '', s).strip()

# ── Placeholders ──────────────────────────────────────────────

placeholders = {}
if os.path.exists(PLACEHOLDERS_FILE):
    with open(PLACEHOLDERS_FILE) as f:
        placeholders = json.load(f)

def is_placeholder(path):
    rel = os.path.relpath(path, SCRIPT_DIR)
    return rel in placeholders and file_hash(path) == placeholders[rel]

# ── Card image index ──────────────────────────────────────────
# norm_stem -> (subdir_name, rel_in_subdir)
# "cards" = final, "cards_beta" = beta, "cards_missing" = placeholder

card_image_index = {}

for subdir_name in ["cards", "cards_beta", "cards_missing"]:
    subdir = os.path.join(SCRIPT_DIR, subdir_name)
    if not os.path.exists(subdir):
        continue
    for root, dirs, files in os.walk(subdir):
        for file in files:
            if not file.lower().endswith(".png"):
                continue
            stem = os.path.splitext(file)[0]
            norm = normalize(stem)
            if norm in card_image_index:
                continue  # first-wins (cards > cards_beta > cards_missing)
            rel_in_subdir = os.path.relpath(os.path.join(root, file), subdir)
            card_image_index[norm] = (subdir_name, rel_in_subdir)

print(f"Card image index: {len(card_image_index)} entries")

def card_source(stem):
    """Returns 'final', 'beta', or 'missing'."""
    entry = card_image_index.get(normalize(stem))
    if not entry:
        return None
    return {"cards": "final", "cards_beta": "beta", "cards_missing": "missing"}[entry[0]]

def card_image_url(stem):
    entry = card_image_index.get(normalize(stem))
    if not entry:
        return ""
    subdir_name, rel_in_subdir = entry
    url = f"{GITHUB_RAW}/{subdir_name}/{rel_in_subdir.replace(os.sep, '/')}"
    return f'=IMAGE("{url}")'

def power_image_url(stem):
    for root, dirs, files in os.walk(power_img_base):
        for file in files:
            if file.lower() == f"{stem}.png":
                rel = os.path.relpath(os.path.join(root, file), SCRIPT_DIR).replace(os.sep, "/")
                return f'=IMAGE("{GITHUB_RAW}/{rel}")'
    return ""

# ── Collect cards ─────────────────────────────────────────────

cards       = defaultdict(dict)
card_images = defaultdict(set)   # norm stems that have ANY non-placeholder image
card_beta   = defaultdict(set)   # norm stems that have ONLY beta image

for root, dirs, files in os.walk(card_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    rel    = os.path.relpath(root, card_base)
    folder = normalize(rel.split(os.sep)[0])
    for file in files:
        if file.endswith(".cs"):
            snake = to_snake(os.path.splitext(file)[0])
            cards[folder][normalize(snake)] = (snake, rel)

for base in CARD_DIRS:
    if not os.path.exists(base):
        continue
    is_beta = "cards_beta" in base
    for root, dirs, files in os.walk(base):
        folder = normalize(os.path.relpath(root, base).split(os.sep)[0])
        for file in files:
            if file.endswith(".png"):
                path = os.path.join(root, file)
                if not is_placeholder(path):
                    norm = normalize(os.path.splitext(file)[0])
                    card_images[folder].add(norm)
                    if is_beta:
                        card_beta[folder].add(norm)

# ── Collect powers ────────────────────────────────────────────

powers       = defaultdict(dict)
power_images = defaultdict(set)

for root, dirs, files in os.walk(power_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    folder = normalize(os.path.relpath(root, power_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".cs"):
            snake = to_snake(os.path.splitext(file)[0])
            snake = re.sub(r'_power$', '', snake)
            powers[folder][normalize(snake)] = (snake, ".")

for root, dirs, files in os.walk(power_img_base):
    folder = normalize(os.path.relpath(root, power_img_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".png"):
            path = os.path.join(root, file)
            if not is_placeholder(path):
                power_images[folder].add(normalize(os.path.splitext(file)[0]))

card_locs  = load_loc(card_loc)
power_locs = load_loc(power_loc, suffix="power")

# ── Build row data ────────────────────────────────────────────

def make_row(status, folder, snake, norm, loc, img_url):
    entry = loc.get(norm, {})
    return [
        status, folder, snake,
        entry.get("title", ""),
        clean_desc(entry.get("description", "")),
        img_url,
    ]

def card_status(folder, norm, snake):
    """MISSING, BETA, or DONE."""
    if norm not in card_images.get(folder, set()):
        return "MISSING"
    # has image — check if it's only in beta
    final_dirs = [os.path.join(SCRIPT_DIR, "cards")]
    for base in final_dirs:
        p = os.path.join(base, "**", f"{snake}.png")
        # Walk to check
        for root, dirs, files in os.walk(base):
            if f"{snake}.png" in files:
                return "DONE"
    return "BETA"

card_folder_rows = {}
all_card_missing = []
all_card_beta    = []

for folder in sorted(cards.keys()):
    all_norms = set(cards[folder].keys())

    missing_rows = []
    beta_rows    = []
    done_rows    = []

    for norm in sorted(all_norms):
        snake, _ = cards[folder][norm]
        url      = card_image_url(snake)
        src      = card_source(snake)

        if src is None or src == "missing":
            row = make_row("MISSING", folder, snake, norm, card_locs, url)
            missing_rows.append(row)
            all_card_missing.append(row)
        elif src == "beta":
            row = make_row("BETA", folder, snake, norm, card_locs, url)
            beta_rows.append(row)
            all_card_beta.append(row)
        else:
            done_rows.append(make_row("DONE", folder, snake, norm, card_locs, url))

    card_folder_rows[folder] = {
        "missing": missing_rows,
        "beta":    beta_rows,
        "done":    done_rows,
    }

power_folder_rows = {}
all_power_missing = []

for folder in sorted(powers.keys()):
    missing_norms = set(powers[folder].keys()) - power_images.get(folder, set())
    done_norms    = set(powers[folder].keys()) & power_images.get(folder, set())

    missing_rows = []
    for norm in sorted(missing_norms):
        snake, _ = powers[folder][norm]
        row = make_row("MISSING", folder, snake, norm, power_locs, power_image_url(snake))
        missing_rows.append(row)
        all_power_missing.append(row)

    done_rows = [
        make_row("DONE", folder, snake, norm, power_locs, power_image_url(snake))
        for norm in sorted(done_norms)
        for snake, _ in [powers[folder][norm]]
    ]

    power_folder_rows[folder] = {"missing": missing_rows, "done": done_rows}

# ── Connect ───────────────────────────────────────────────────

SCOPES = [
    "https://www.googleapis.com/auth/spreadsheets",
    "https://www.googleapis.com/auth/drive",
]

creds = Credentials.from_service_account_file(SERVICE_ACCOUNT, scopes=SCOPES)
gc    = gspread.authorize(creds)
sh    = gc.open_by_key(SHEET_ID)

sheets_cache = load_cache(SHEETS_CACHE_FILE)

# ── Sheets API helpers ────────────────────────────────────────

def api_batch(sh, requests):
    sh.batch_update({"requests": requests})
    time.sleep(1)

def api_set_row_heights(sh, ws, start_row, end_row, px):
    if end_row <= start_row:
        return
    api_batch(sh, [{"updateDimensionProperties": {
        "range": {"sheetId": ws.id, "dimension": "ROWS",
                  "startIndex": start_row, "endIndex": end_row},
        "properties": {"pixelSize": px}, "fields": "pixelSize"
    }}])

def api_set_col_width(sh, ws, col_index, px):
    api_batch(sh, [{"updateDimensionProperties": {
        "range": {"sheetId": ws.id, "dimension": "COLUMNS",
                  "startIndex": col_index, "endIndex": col_index + 1},
        "properties": {"pixelSize": px}, "fields": "pixelSize"
    }}])

def api_set_tab_color(sh, ws, color):
    api_batch(sh, [{"updateSheetProperties": {
        "properties": {"sheetId": ws.id, "tabColorStyle": {"rgbColor": color}},
        "fields": "tabColorStyle"
    }}])

def get_or_create_ws(sh, name, rows=1000):
    try:
        return sh.worksheet(name)
    except gspread.exceptions.WorksheetNotFound:
        return sh.add_worksheet(title=name, rows=rows, cols=10)

def write_sheet(sh, ws, rows, tab_color):
    ws.clear()
    time.sleep(1)

    if not rows:
        ws.update([HEADER, ["(empty)"]], "A1", value_input_option="USER_ENTERED")
        api_set_tab_color(sh, ws, tab_color)
        return

    all_rows = [HEADER] + [r[:6] for r in rows]
    ws.update(all_rows, "A1", value_input_option="USER_ENTERED")
    time.sleep(1)

    total = len(rows)
    api_set_row_heights(sh, ws, 0, 1, HEADER_ROW_PX)
    api_set_row_heights(sh, ws, 1, total + 1, ROW_HEIGHT_PX)
    api_set_col_width(sh, ws, 5, IMG_COL_PX)
    api_set_tab_color(sh, ws, tab_color)

    # Build format ranges by status
    fmt = [{
        "range": "A1:F1",
        "format": {
            "backgroundColor": COL_HEADER_BG,
            "textFormat": {"bold": True, "fontSize": 10, "foregroundColor": COL_HEADER_FG},
            "verticalAlignment": "MIDDLE",
        }
    }]

    for i, row in enumerate(rows, start=2):
        status = row[0]
        bg = COL_MISSING_BG if status == "MISSING" else COL_BETA_BG if status == "BETA" else COL_DONE_BG
        fmt.append({
            "range": f"A{i}:E{i}",
            "format": {"backgroundColor": bg, "verticalAlignment": "MIDDLE"}
        })
        fmt.append({
            "range": f"F{i}:F{i}",
            "format": {"backgroundColor": COL_WHITE, "verticalAlignment": "MIDDLE"}
        })

    ws.batch_format(fmt)
    time.sleep(1)

    missing = sum(1 for r in rows if r[0] == "MISSING")
    beta    = sum(1 for r in rows if r[0] == "BETA")
    done    = sum(1 for r in rows if r[0] == "DONE")
    print(f"    '{ws.title}': {missing} missing / {beta} beta / {done} done")

def sync(tab_name, rows, tab_color, cache_key=None):
    key = cache_key or tab_name
    h   = rows_hash(rows)
    if sheets_cache.get(key) == h:
        print(f"  {tab_name}: no changes, skipping")
        return
    ws = get_or_create_ws(sh, tab_name, rows=max(1000, len(rows) + 10))
    write_sheet(sh, ws, rows, tab_color)
    sheets_cache[key] = h

# ── Upload ────────────────────────────────────────────────────

print("Syncing tabs...")

sync("⚠ Missing Cards",  all_card_missing,               COL_TAB_MISSING)
sync("🟡 Beta Cards",    all_card_beta,                  COL_TAB_MISSING)
sync("⚠ Missing Powers", all_power_missing,              COL_TAB_MISSING)

for folder in sorted(card_folder_rows.keys()):
    data  = card_folder_rows[folder]
    rows  = data["missing"] + data["beta"] + data["done"]
    label = folder.capitalize()
    sync(f"{label}", rows, COL_TAB_CHAMP, cache_key=f"cards_{folder}")

all_power_rows = []
for folder in sorted(power_folder_rows.keys()):
    data = power_folder_rows[folder]
    all_power_rows += data["missing"] + data["done"]
sync("Powers", all_power_rows, COL_TAB_POWER)

try:
    sh.del_worksheet(sh.worksheet("Sheet1"))
except:
    pass

save_cache(SHEETS_CACHE_FILE, sheets_cache)
print(f"\nDone: https://docs.google.com/spreadsheets/d/{SHEET_ID}")