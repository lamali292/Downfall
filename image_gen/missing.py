import os, re, shutil, json, hashlib
from collections import defaultdict

ROOT               = os.path.dirname(os.path.abspath(__file__))

# Path configuration
card_base          = os.path.join(ROOT, "..", "Code", "Cards")
img_base           = os.path.join(ROOT, "cards")
img_beta           = os.path.join(ROOT, "cards_beta")
img_missing        = os.path.join(ROOT, "cards_missing")

power_base         = os.path.join(ROOT, "..", "Code", "Powers")
power_img_base     = os.path.join(ROOT, "powers")
power_beta_base    = os.path.join(ROOT, "powers_beta")
power_missing_base = os.path.join(ROOT, "powers_missing")

missing_card_png   = os.path.join(ROOT, "missing.png")
missing_power_png  = os.path.join(ROOT, "missing_power.png")
PLACEHOLDERS_FILE  = os.path.join(ROOT, ".placeholders.json")

def to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

def normalize(name, is_power_type=False):
    """
    Standardizes names for matching.
    """
    snake = to_snake(name)
    if is_power_type:
        snake = re.sub(r'_power$', '', snake)
    return snake.replace("_", "")

def file_hash(path):
    if not os.path.exists(path): return ""
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()

# ── Placeholder Setup ─────────────────────────────────────────

MISSING_CARD_HASH  = file_hash(missing_card_png)
MISSING_POWER_HASH = file_hash(missing_power_png)

placeholders = {}
if os.path.exists(PLACEHOLDERS_FILE):
    with open(PLACEHOLDERS_FILE) as f:
        try:
            placeholders = json.load(f)
        except:
            placeholders = {}

def collect_real_images_flat(dirs, placeholder_hash, is_power_type):
    found = set()
    for base in dirs:
        if not os.path.exists(base): continue
        for root, _, files in os.walk(base):
            for file in files:
                if not file.lower().endswith(".png"): continue
                abs_path = os.path.join(root, file)
                rel_path = os.path.relpath(abs_path, ROOT)
                if rel_path in placeholders and file_hash(abs_path) == placeholder_hash:
                    continue
                
                stem = os.path.splitext(file)[0]
                found.add(normalize(stem, is_power_type))
    return found

# ── Process Cards ─────────────────────────────────────────────

cards = defaultdict(dict)
for root, dirs, files in os.walk(card_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    rel_path = os.path.relpath(root, card_base)
    # Force the folder name to lowercase
    folder = rel_path.split(os.sep)[0].lower() if rel_path != "." else ""
    for file in files:
        if file.endswith(".cs"):
            name = os.path.splitext(file)[0]
            snake = to_snake(name)
            cards[folder][normalize(name, False)] = snake

card_images_flat = collect_real_images_flat([img_base, img_beta], MISSING_CARD_HASH, False)

print("=== CHECKING CARDS ===")
for folder in sorted(cards.keys()):
    missing_norms = set(cards[folder].keys()) - card_images_flat
    
    if missing_norms:
        print(f"In folder '{folder if folder else 'root'}':")
        for norm in sorted(missing_norms):
            snake_name = cards[folder][norm]
            # Output directory is always lowercase
            dest_dir = os.path.join(img_missing, folder) if folder else img_missing
            os.makedirs(dest_dir, exist_ok=True)
            
            dest_path = os.path.join(dest_dir, f"{snake_name}.png")
            if not os.path.exists(dest_path):
                shutil.copy(missing_card_png, dest_path)
                print(f"  -> Created placeholder: {snake_name}")
            
            rel_dest = os.path.relpath(dest_path, ROOT)
            placeholders[rel_dest] = MISSING_CARD_HASH

# ── Process Powers ────────────────────────────────────────────

powers = defaultdict(dict)
for root, dirs, files in os.walk(power_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    rel_path = os.path.relpath(root, power_base)
    # Force the folder name to lowercase
    folder = rel_path.split(os.sep)[0].lower() if rel_path != "." else ""
    for file in files:
        if file.endswith(".cs"):
            name = os.path.splitext(file)[0]
            snake = to_snake(name)
            powers[folder][normalize(name, True)] = re.sub(r'_power$', '', snake)

power_images_flat = collect_real_images_flat([power_img_base, power_beta_base], MISSING_POWER_HASH, True)

print("\n=== CHECKING POWERS ===")
for folder in sorted(powers.keys()):
    missing_norms = set(powers[folder].keys()) - power_images_flat
    
    if missing_norms:
        print(f"In folder '{folder if folder else 'root'}':")
        for norm in sorted(missing_norms):
            snake_name = powers[folder][norm]
            # Output directory is always lowercase
            dest_dir = os.path.join(power_missing_base, folder) if folder else power_missing_base
            os.makedirs(dest_dir, exist_ok=True)
            
            dest_path = os.path.join(dest_dir, f"{snake_name}.png")
            if not os.path.exists(dest_path):
                shutil.copy(missing_power_png, dest_path)
                print(f"  -> Created placeholder: {snake_name}")
            
            rel_dest = os.path.relpath(dest_path, ROOT)
            placeholders[rel_dest] = MISSING_POWER_HASH

# ── Save and Cleanup ─────────────────────────────────────────

final_placeholders = {}
for rel_path, stored_hash in placeholders.items():
    abs_path = os.path.join(ROOT, rel_path)
    if os.path.exists(abs_path) and file_hash(abs_path) == stored_hash:
        final_placeholders[rel_path] = stored_hash

with open(PLACEHOLDERS_FILE, "w") as f:
    json.dump(final_placeholders, f, indent=2)

print("\nDone!")