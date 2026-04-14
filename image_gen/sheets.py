import os, re, json, hashlib
from collections import defaultdict
from PIL import Image
import gspread
from google.oauth2.service_account import Credentials

ROOT              = os.path.dirname(__file__)
card_base         = os.path.join(ROOT, "..", "Code", "Cards")
power_base        = os.path.join(ROOT, "..", "Code", "Powers")
power_img_base    = os.path.join(ROOT, "powers")
card_loc          = os.path.join(ROOT, "..", "Downfall", "localization", "eng", "cards.json")
power_loc         = os.path.join(ROOT, "..", "Downfall", "localization", "eng", "powers.json")
PLACEHOLDERS_FILE = os.path.join(ROOT, ".placeholders.json")
SERVICE_ACCOUNT   = os.path.join(ROOT, "service_account.json")
SHEET_ID          = "1adgDbqi4A7oHqtAb2klUFrUsl4-TQR_AIWqDdDQUQ1g"

CARD_DIRS = [
    os.path.join(ROOT, "cards"),
    os.path.join(ROOT, "cards_beta"),
    os.path.join(ROOT, "cards_missing"),
]

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

# ── Placeholders ──────────────────────────────────────────────

placeholders = {}
if os.path.exists(PLACEHOLDERS_FILE):
    with open(PLACEHOLDERS_FILE) as f:
        placeholders = json.load(f)

def is_placeholder(path):
    rel = os.path.relpath(path, ROOT)
    return rel in placeholders and file_hash(path) == placeholders[rel]

# ── Collect cards ─────────────────────────────────────────────

cards, card_images = defaultdict(dict), defaultdict(set)

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
    for root, dirs, files in os.walk(base):
        folder = normalize(os.path.relpath(root, base).split(os.sep)[0])
        for file in files:
            if file.endswith(".png"):
                path = os.path.join(root, file)
                if not is_placeholder(path):
                    card_images[folder].add(normalize(os.path.splitext(file)[0]))

# ── Collect powers ────────────────────────────────────────────

powers, power_images = defaultdict(dict), defaultdict(set)

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

# ── Build rows ────────────────────────────────────────────────

def build_rows(code_dict, image_dict, loc):
    rows = []
    all_folders = sorted(set(list(code_dict.keys()) + list(image_dict.keys())))

    # Missing first
    for folder in all_folders:
        missing = set(code_dict.get(folder, {}).keys()) - image_dict.get(folder, set())
        for norm in sorted(missing):
            snake, _ = code_dict[folder][norm]
            entry    = loc.get(norm, {})
            rows.append([
                "MISSING",
                folder,
                snake,
                entry.get("title", ""),
                re.sub(r'\[/?[^\]]+\]', '', entry.get("description", "")).strip(),
            ])

    # Has art
    for folder in all_folders:
        has_art = set(code_dict.get(folder, {}).keys()) & image_dict.get(folder, set())
        for norm in sorted(has_art):
            snake, _ = code_dict[folder][norm]
            entry    = loc.get(norm, {})
            rows.append([
                "DONE",
                folder,
                snake,
                entry.get("title", ""),
                re.sub(r'\[/?[^\]]+\]', '', entry.get("description", "")).strip(),
            ])

    return rows

card_rows  = build_rows(cards,  card_images,  card_locs)
power_rows = build_rows(powers, power_images, power_locs)

# ── Upload to Google Sheets ───────────────────────────────────

SCOPES = [
    "https://www.googleapis.com/auth/spreadsheets",
    "https://www.googleapis.com/auth/drive",
]

creds  = Credentials.from_service_account_file(SERVICE_ACCOUNT, scopes=SCOPES)
gc     = gspread.authorize(creds)
sh     = gc.open_by_key(SHEET_ID)

HEADER = ["Status", "Folder", "File Name", "Title", "Description"]

def sync_sheet(sh, tab_name, rows):
    try:
        ws = sh.worksheet(tab_name)
        ws.clear()
    except gspread.exceptions.WorksheetNotFound:
        ws = sh.add_worksheet(title=tab_name, rows=1000, cols=10)

    all_rows = [HEADER] + rows
    ws.update(all_rows, "A1")

    # Colour status column — red for MISSING, green for DONE
    missing_count = sum(1 for r in rows if r[0] == "MISSING")
    done_count    = sum(1 for r in rows if r[0] == "DONE")

    if missing_count:
        ws.format(f"A2:E{1 + missing_count}", {
            "backgroundColor": {"red": 1.0, "green": 0.8, "blue": 0.8}
        })
    if done_count:
        start = 2 + missing_count
        ws.format(f"A{start}:E{start + done_count - 1}", {
            "backgroundColor": {"red": 0.8, "green": 1.0, "blue": 0.8}
        })

    # Bold header
    ws.format("A1:E1", {
        "backgroundColor": {"red": 0.267, "green": 0.447, "blue": 0.769},
        "textFormat": {"bold": True, "foregroundColor": {"red": 1, "green": 1, "blue": 1}}
    })

    print(f"  {tab_name}: {missing_count} missing, {done_count} done")

print("Uploading...")
sync_sheet(sh, "Cards",  card_rows)
sync_sheet(sh, "Powers", power_rows)

# Remove default Sheet1 if it exists
try:
    sh.del_worksheet(sh.worksheet("Sheet1"))
except:
    pass

print(f"Done: https://docs.google.com/spreadsheets/d/{SHEET_ID}")