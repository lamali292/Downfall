from PIL import Image
import os, hashlib, json, string, sys

ROOT        = os.path.dirname(os.path.abspath(__file__))
PARENT      = os.path.join(ROOT, "..")
MAX_ATLAS   = 4096
PADDING     = 1
NORMAL_SIZE  = (250, 190)
ANCIENT_SIZE = (250, 351)
CACHE_FILE  = os.path.join(ROOT, ".cards_cache.json")
UID_CHARS   = string.ascii_lowercase + string.digits

INPUT_SUBDIRS = ["cards", "cards_beta", "cards_missing"]

def file_hash(path):
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""): h.update(chunk)
    return h.hexdigest()

def deterministic_uid(name, length=7):
    h = int(hashlib.md5(name.encode()).hexdigest(), 16)
    result = []
    for _ in range(length):
        result.append(UID_CHARS[h % len(UID_CHARS)])
        h //= len(UID_CHARS)
    return ''.join(result)

def flatten_alpha(img):
    bg = Image.new("RGB", img.size, (0, 0, 0))
    if img.mode == "RGBA": bg.paste(img, mask=img.split()[3])
    else: bg.paste(img.convert("RGB"))
    return bg

def write_if_changed(path, data: bytes):
    if os.path.exists(path):
        with open(path, "rb") as f:
            if f.read() == data: return False
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "wb") as f: f.write(data)
    return True

def save_image_if_changed(img, path):
    import io
    buf = io.BytesIO()
    img.save(buf, format="PNG")
    return write_if_changed(path, buf.getvalue())

def cols_for(w): return MAX_ATLAS // (w + PADDING)
def rows_for(h): return MAX_ATLAS // (h + PADDING)

def slot_to_pos(slot, w, h):
    cols = cols_for(w)
    rows = rows_for(h)
    spa  = cols * rows
    idx  = slot // spa
    loc  = slot % spa
    return idx, (loc % cols) * (w + PADDING), (loc // cols) * (h + PADDING)

def pack(group_id, entries, out_dir, res_base, atlas_base, sprites_folder, card_size, group_cache, force):
    w, h = card_size
    if force: group_cache = {}
    entry_map = {stem: (img, hsh) for stem, img, hsh in entries}
    next_slot = max((v["slot"] for v in group_cache.values()), default=-1) + 1
    slot_map = {}
    dirty = set()
    for stem, cached in group_cache.items():
        if stem in entry_map:
            slot_map[stem] = cached["slot"]
            if cached["hash"] != entry_map[stem][1]: dirty.add(stem)
    for stem in entry_map:
        if stem not in slot_map:
            slot_map[stem] = next_slot; next_slot += 1; dirty.add(stem)
    removed = set(group_cache) - set(entry_map)
    if not dirty and not removed and not force:
        print(f"  {group_id}: no changes"); return group_cache

    max_slot = max(slot_map.values(), default=0)
    spa = cols_for(w) * rows_for(h)
    atlas_count = (max_slot // spa) + 1
    canvases = [Image.new("RGB", (MAX_ATLAS, MAX_ATLAS), (0,0,0)) for _ in range(atlas_count)]
    for stem, slot in slot_map.items():
        img, _ = entry_map[stem]
        idx, x, y = slot_to_pos(slot, w, h)
        canvases[idx].paste(img, (x, y))

    atlas_res_paths = []
    for idx, canvas in enumerate(canvases):
        slots_on_atlas = [(slot_to_pos(slot, w, h)[1], slot_to_pos(slot, w, h)[2])
                  for stem, slot in slot_map.items() if slot_to_pos(slot, w, h)[0] == idx]
        used_w = max((x + w for x, _ in slots_on_atlas), default=w)
        used_h = max((y + h for _, y in slots_on_atlas), default=h)
        cropped = canvas.crop((0, 0, used_w, used_h))
        fname = f"{atlas_base}_{idx}.png"
        atlas_res_paths.append(f"{res_base}/{fname}")
        if save_image_if_changed(cropped, os.path.join(out_dir, fname)):
            print(f"  wrote: {fname}")

    tres_dir = os.path.join(out_dir, f"{sprites_folder}.sprites")
    os.makedirs(tres_dir, exist_ok=True)
    tres_written = 0
    for stem, slot in slot_map.items():
        idx, x, y = slot_to_pos(slot, w, h)
        cached = group_cache.get(stem, {})
        if stem not in dirty and cached.get("atlas_idx") == idx: continue
        content = (
            f'[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(f"{group_id}_{stem}")}"]\n'
            f'[ext_resource type="Texture2D" path="{atlas_res_paths[idx]}" id="1"]\n'
            f'[resource]\natlas = ExtResource("1")\nregion = Rect2({x}, {y}, {w}, {h})\n'
        )
        write_if_changed(os.path.join(tres_dir, f"{stem}.tres"), content.encode())
        tres_written += 1

    for stem in removed:
        p = os.path.join(tres_dir, f"{stem}.tres")
        if os.path.exists(p): os.remove(p); print(f"  removed: {stem}.tres")

    print(f"  {group_id}: {len(entry_map)} cards, {atlas_count} page(s), {tres_written} .tres updated")
    new_cache = {}
    for stem, slot in slot_map.items():
        idx, _, _ = slot_to_pos(slot, w, h)
        new_cache[stem] = {"hash": entry_map[stem][1], "slot": slot, "atlas_idx": idx}
    return new_cache

force = "--repack" in sys.argv
cache = json.load(open(CACHE_FILE)) if os.path.exists(CACHE_FILE) else {}

# Auto-discover characters from image_gen/cards/ subfolders
char_ids = set()
for sub in INPUT_SUBDIRS:
    d = os.path.join(ROOT, sub)
    if not os.path.exists(d): continue
    for entry in os.listdir(d):
        if os.path.isdir(os.path.join(d, entry)):
            char_ids.add(entry.lower())

for char_id in sorted(char_ids):
    char_proj = next((e for e in os.listdir(PARENT)
                      if e.lower() == char_id and os.path.isdir(os.path.join(PARENT, e))
                      and not e.endswith("Code")), None)
    if not char_proj:
        print(f"No project folder found for {char_id}, skipping"); continue

    out_dir  = os.path.join(PARENT, char_proj, char_proj, "images", "atlases")
    res_base = f"res://{char_proj}/images/atlases"
    os.makedirs(out_dir, exist_ok=True)

    print(f"\n=== {char_id} -> {char_proj}/images/atlases ===")

    seen = set()
    normal_entries  = []
    ancient_entries = []

    for sub in INPUT_SUBDIRS:
        d = os.path.join(ROOT, sub, char_id)
        if not os.path.exists(d): continue
        for file in sorted(os.listdir(d)):
            if not file.lower().endswith(".png") or file in seen: continue
            seen.add(file)
            path = os.path.join(d, file)
            stem = os.path.splitext(file)[0]
            img  = Image.open(path).convert("RGBA")
            hsh  = file_hash(path)
            if img.height > img.width:
                ancient_entries.append((stem, flatten_alpha(img.resize(ANCIENT_SIZE, Image.LANCZOS)), hsh))
            else:
                normal_entries.append((stem, flatten_alpha(img.resize(NORMAL_SIZE, Image.LANCZOS)), hsh))

    char_cache = cache.get(char_id, {})
    char_cache["normal"] = pack(
        f"{char_id}_normal", normal_entries, out_dir, res_base,
        f"card_atlas", "card_atlas",
        NORMAL_SIZE, char_cache.get("normal", {}), force
    )
    char_cache["ancient"] = pack(
        f"{char_id}_ancient", ancient_entries, out_dir, res_base,
        f"card_ancient_atlas", "card_atlas",
        ANCIENT_SIZE, char_cache.get("ancient", {}), force
    )
    cache[char_id] = char_cache

with open(CACHE_FILE, "w") as f: json.dump(cache, f, indent=2)
print("\nDone!")