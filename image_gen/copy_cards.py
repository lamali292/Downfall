import os, shutil, hashlib, json

SRC        = os.path.join(os.path.dirname(__file__), "cards")
DEST       = os.path.join(os.path.dirname(__file__), "..", "Downfall", "images", "card_portraits")
CACHE_FILE = os.path.join(os.path.dirname(__file__), ".cards_cache.json")

def file_hash(path):
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()

def collect_hashes():
    hashes = {}
    for root, dirs, files in os.walk(SRC):
        for file in sorted(files):
            if file.lower().endswith(".png"):
                path = os.path.join(root, file)
                hashes[path] = file_hash(path)
    return hashes

cache   = json.load(open(CACHE_FILE)) if os.path.exists(CACHE_FILE) else {}
current = collect_hashes()

if cache.get("input_hashes") == current:
    print("Nothing changed, skipping.")
    exit(0)

for root, dirs, files in os.walk(SRC):
    rel_folder  = os.path.relpath(root, SRC)
    dest_folder = os.path.join(DEST, rel_folder)
    os.makedirs(dest_folder, exist_ok=True)
    for file in files:
        if not file.lower().endswith(".png"):
            continue
        shutil.copy2(os.path.join(root, file), os.path.join(dest_folder, file))
        print(f"copied: {os.path.join(rel_folder, file)}")

cache["input_hashes"] = current
with open(CACHE_FILE, "w") as f:
    json.dump(cache, f, indent=2)

print("Done.")