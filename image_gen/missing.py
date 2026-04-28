import os, re, shutil, json, hashlib
from collections import defaultdict

ROOT   = os.path.dirname(os.path.abspath(__file__))
PARENT = os.path.join(ROOT, "..")

def to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

def normalize(name, is_power_type=False):
    snake = to_snake(name)
    if is_power_type:
        snake = re.sub(r'_power$', '', snake)
    return snake.replace("_", "")

def file_hash(path):
    if not os.path.exists(path): return ""
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""): h.update(chunk)
    return h.hexdigest()

missing_card_png  = os.path.join(ROOT, "missing.png")
missing_power_png = os.path.join(ROOT, "missing_power.png")
PLACEHOLDERS_FILE = os.path.join(ROOT, ".placeholders.json")
MISSING_CARD_HASH  = file_hash(missing_card_png)
MISSING_POWER_HASH = file_hash(missing_power_png)

placeholders = {}
if os.path.exists(PLACEHOLDERS_FILE):
    with open(PLACEHOLDERS_FILE) as f:
        try: placeholders = json.load(f)
        except: pass

def collect_images(base, placeholder_hash, is_power_type):
    found = set()
    if not os.path.exists(base): return found
    for root, _, files in os.walk(base):
        for file in files:
            if not file.lower().endswith(".png"): continue
            abs_path = os.path.join(root, file)
            rel_path = os.path.relpath(abs_path, ROOT)
            if rel_path in placeholders and file_hash(abs_path) == placeholder_hash:
                continue
            found.add(normalize(os.path.splitext(file)[0], is_power_type))
    return found

# Auto-discover characters
characters = {}
for project in os.listdir(PARENT):
    project_path = os.path.join(PARENT, project)
    if not os.path.isdir(project_path): continue
    for entry in os.listdir(project_path):
        if entry.endswith("Code") and os.path.isdir(os.path.join(project_path, entry)):
            char_id = entry[:-4].lower()
            characters[char_id] = os.path.join(project_path, entry)

for char_id, code_dir in sorted(characters.items()):
    print(f"\n=== {char_id.upper()} ===")
    
    card_base  = os.path.join(code_dir, "Cards")
    power_base = os.path.join(code_dir, "Powers")
    img_dir    = os.path.join(ROOT, "cards", char_id)
    pow_dir    = os.path.join(ROOT, "powers", char_id)

    # Cards
    cards = {}
    if os.path.exists(card_base):
        for root, dirs, files in os.walk(card_base):
            dirs[:] = [d for d in dirs if d != "Abstract"]
            for file in files:
                if file.endswith(".cs"):
                    name = os.path.splitext(file)[0]
                    cards[normalize(name)] = to_snake(name)

    card_images = collect_images(img_dir, MISSING_CARD_HASH, False)
    for norm, snake in sorted(cards.items()):
        if norm not in card_images:
            dest_dir = os.path.join(ROOT, "cards_missing", char_id)
            os.makedirs(dest_dir, exist_ok=True)
            dest = os.path.join(dest_dir, f"{snake}.png")
            if not os.path.exists(dest):
                shutil.copy(missing_card_png, dest)
                print(f"  card placeholder: {snake}")
            placeholders[os.path.relpath(dest, ROOT)] = MISSING_CARD_HASH

    # Powers
    powers = {}
    if os.path.exists(power_base):
        for root, dirs, files in os.walk(power_base):
            dirs[:] = [d for d in dirs if d != "Abstract"]
            for file in files:
                if file.endswith(".cs"):
                    name = os.path.splitext(file)[0]
                    powers[normalize(name, True)] = re.sub(r'_power$', '', to_snake(name))

    pow_images = collect_images(pow_dir, MISSING_POWER_HASH, True)
    for norm, snake in sorted(powers.items()):
        if norm not in pow_images:
            dest_dir = os.path.join(ROOT, "powers_missing", char_id)
            os.makedirs(dest_dir, exist_ok=True)
            dest = os.path.join(dest_dir, f"{snake}.png")
            if not os.path.exists(dest):
                shutil.copy(missing_power_png, dest)
                print(f"  power placeholder: {snake}")
            placeholders[os.path.relpath(dest, ROOT)] = MISSING_POWER_HASH

with open(PLACEHOLDERS_FILE, "w") as f:
    json.dump({k: v for k, v in placeholders.items()
               if os.path.exists(os.path.join(ROOT, k)) and file_hash(os.path.join(ROOT, k)) == v}, f, indent=2)
print("\nDone!")