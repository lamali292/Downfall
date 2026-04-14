import os, re, shutil, json, hashlib
from collections import defaultdict

ROOT              = os.path.dirname(os.path.abspath(__file__))
card_base         = os.path.join(ROOT, "..", "Code", "Cards")
img_base          = os.path.join(ROOT, "cards")
img_beta          = os.path.join(ROOT, "cards_beta")
img_missing       = os.path.join(ROOT, "cards_missing")
power_base        = os.path.join(ROOT, "..", "Code", "Powers")
power_img_base    = os.path.join(ROOT, "powers")
missing_card_png  = os.path.join(ROOT, "missing.png")
missing_power_png = os.path.join(ROOT, "missing_power.png")
PLACEHOLDERS_FILE = os.path.join(ROOT, ".placeholders.json")

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

MISSING_CARD_HASH  = file_hash(missing_card_png)
MISSING_POWER_HASH = file_hash(missing_power_png)

placeholders = {}
if os.path.exists(PLACEHOLDERS_FILE):
    with open(PLACEHOLDERS_FILE) as f:
        placeholders = json.load(f)

def collect_card_images(*dirs):
    """Collect all real (non-placeholder) card image names from multiple dirs."""
    found = defaultdict(set)
    for base in dirs:
        if not os.path.exists(base):
            continue
        for root, dirs_, files in os.walk(base):
            folder = normalize(os.path.relpath(root, base).split(os.sep)[0])
            for file in files:
                if file.endswith(".png"):
                    path = os.path.join(root, file)
                    rel  = os.path.relpath(path, ROOT)
                    if not (rel in placeholders and file_hash(path) == MISSING_CARD_HASH):
                        found[folder].add(to_snake(os.path.splitext(file)[0]))
    return found

# ── Cards ─────────────────────────────────────────────────────

cards = defaultdict(dict)
for root, dirs, files in os.walk(card_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    folder = normalize(os.path.relpath(root, card_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".cs"):
            snake = to_snake(os.path.splitext(file)[0])
            cards[folder][to_snake(snake)] = snake

# Real art = anything in cards/ or cards_beta/ that isn't a placeholder
images = collect_card_images(img_base, img_beta)

print("=== CARDS ===")
for folder in sorted(set(list(cards.keys()) + list(images.keys()))):
    missing_art = set(cards.get(folder, {}).keys()) - images.get(folder, set())
    if missing_art:
        print(f"{folder}:")
        for norm_name in sorted(missing_art):
            snake_name  = cards[folder][norm_name]
            dest_folder = os.path.join(img_missing, folder)
            os.makedirs(dest_folder, exist_ok=True)
            dest = os.path.join(dest_folder, f"{snake_name}.png")
            if not os.path.exists(dest):
                shutil.copy(missing_card_png, dest)
                print(f"  -> placeholder: {dest}")
            rel = os.path.relpath(dest, ROOT)
            placeholders[rel] = MISSING_CARD_HASH

# ── Powers ────────────────────────────────────────────────────

powers, power_images = defaultdict(dict), defaultdict(set)
for root, dirs, files in os.walk(power_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    folder = normalize(os.path.relpath(root, power_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".cs"):
            snake = to_snake(os.path.splitext(file)[0])
            snake = re.sub(r'_power$', '', snake)
            powers[folder][normalize(snake)] = snake

for root, dirs, files in os.walk(power_img_base):
    folder = normalize(os.path.relpath(root, power_img_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".png"):
            path = os.path.join(root, file)
            rel  = os.path.relpath(path, ROOT)
            if not (rel in placeholders and file_hash(path) == MISSING_POWER_HASH):
                power_images[folder].add(normalize(os.path.splitext(file)[0]))

print("\n=== POWERS ===")
for folder in sorted(set(list(powers.keys()) + list(power_images.keys()))):
    missing_art = set(powers.get(folder, {}).keys()) - power_images.get(folder, set())
    if missing_art:
        print(f"{folder}:")
        for norm_name in sorted(missing_art):
            snake_name  = powers[folder][norm_name]
            dest_folder = os.path.join(power_img_base, folder)
            os.makedirs(dest_folder, exist_ok=True)
            dest = os.path.join(dest_folder, f"{snake_name}.png")
            if not os.path.exists(dest):
                shutil.copy(missing_power_png, dest)
                print(f"  -> placeholder: {dest}")
            rel = os.path.relpath(dest, ROOT)
            placeholders[rel] = MISSING_POWER_HASH

# ── Save placeholders ─────────────────────────────────────────

placeholders = {
    k: v for k, v in placeholders.items()
    if os.path.exists(os.path.join(ROOT, k)) and file_hash(os.path.join(ROOT, k)) == v
}

with open(PLACEHOLDERS_FILE, "w") as f:
    json.dump(placeholders, f, indent=2)