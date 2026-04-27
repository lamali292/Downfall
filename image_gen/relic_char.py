import os, shutil, re

SCRIPT_DIR  = os.path.dirname(os.path.abspath(__file__))
PARENT      = os.path.join(SCRIPT_DIR, "..")
RELICS_DIR  = os.path.join(SCRIPT_DIR, "relics")

def to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

# Build map: snake_stem -> char_id
relic_to_char = {}
for proj_entry in os.listdir(PARENT):
    proj_path = os.path.join(PARENT, proj_entry)
    if not os.path.isdir(proj_path): continue
    # Look for a *Code subfolder inside each project folder
    for sub_entry in os.listdir(proj_path):
        if not sub_entry.endswith("Code"): continue
        char_id    = sub_entry[:-4].lower()
        relics_dir = os.path.join(proj_path, sub_entry, "Relics")
        if not os.path.exists(relics_dir): continue
        for root, _, files in os.walk(relics_dir):
            for file in files:
                if file.endswith(".cs"):
                    stem  = os.path.splitext(file)[0]
                    snake = to_snake(stem)
                    relic_to_char[snake] = char_id
                    print(f"  found: {snake} -> {char_id}")

print(f"\nFound {len(relic_to_char)} relics across {len(set(relic_to_char.values()))} characters\n")

# Move flat PNGs into subfolders
moved   = 0
skipped = 0
unknown = []

for file in sorted(os.listdir(RELICS_DIR)):
    if not file.lower().endswith(".png"): continue
    src  = os.path.join(RELICS_DIR, file)
    if os.path.isdir(src): continue  # skip existing subfolders
    stem = os.path.splitext(file)[0]

    char_id = relic_to_char.get(stem)
    if not char_id:
        unknown.append(file)
        skipped += 1
        continue

    dest_dir = os.path.join(RELICS_DIR, char_id)
    os.makedirs(dest_dir, exist_ok=True)
    dest = os.path.join(dest_dir, file)
    if os.path.exists(dest):
        print(f"  already exists, skipping: {file}")
        skipped += 1
        continue

    shutil.move(src, dest)
    print(f"  moved: {file} -> {char_id}/")
    moved += 1

print(f"\nMoved: {moved}, Skipped: {skipped}")
if unknown:
    print(f"Unknown (no matching Code folder): {unknown}")