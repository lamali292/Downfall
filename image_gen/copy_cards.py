from PIL import Image
import os, hashlib, json, string, sys

# ============================================================
# CONFIG
# ============================================================
INPUT_DIRS      = ["cards", "cards_beta", "cards_missing"]  # priority: first wins
OUT_ATLASES     = "../Downfall/images/atlases"
SPRITES_DIR     = "card_atlas.sprites"

NORMAL_SIZE     = (250, 190)
ANCIENT_SIZE    = (250, 351)
MAX_ATLAS       = 4096
PADDING         = 1

NORMAL_BASE     = "card_atlas"
ANCIENT_BASE    = "card_ancient_atlas"
ATLAS_RES_BASE  = "res://Downfall/images/atlases"
CACHE_FILE      = ".cards_cache.json"
# ============================================================

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
OUT_TRES   = os.path.join(OUT_ATLASES, SPRITES_DIR)
UID_CHARS  = string.ascii_lowercase + string.digits

# ── Helpers ───────────────────────────────────────────────────

def file_hash(path):
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()

def load_cache():
    return json.load(open(CACHE_FILE)) if os.path.exists(CACHE_FILE) else {}

def save_cache(cache):
    with open(CACHE_FILE, "w") as f:
        json.dump(cache, f, indent=2)

def write_if_changed(path, data: bytes):
    if os.path.exists(path):
        with open(path, "rb") as f:
            if f.read() == data:
                return False
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "wb") as f:
        f.write(data)
    return True

def save_image_if_changed(img, path):
    import io
    buf = io.BytesIO()
    img.save(buf, format="PNG")
    return write_if_changed(path, buf.getvalue())

def deterministic_uid(name, length=7):
    h = int(hashlib.md5(name.encode()).hexdigest(), 16)
    result = []
    for _ in range(length):
        result.append(UID_CHARS[h % len(UID_CHARS)])
        h //= len(UID_CHARS)
    return ''.join(result)

def flatten_alpha(img):
    bg = Image.new("RGB", img.size, (0, 0, 0))
    if img.mode == "RGBA":
        bg.paste(img, mask=img.split()[3])
    else:
        bg.paste(img.convert("RGB"))
    return bg

def is_ancient(img):
    return img.height > img.width

def write_tres(path, atlas_res_path, x, y, w, h, uid_name):
    content = (
        f'[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(uid_name)}"]\n'
        f'[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]\n'
        f'[resource]\n'
        f'atlas = ExtResource("1")\n'
        f'region = Rect2({x}, {y}, {w}, {h})\n'
    )
    write_if_changed(path, content.encode())

def tres_name_for(char_id, stem):
    return f"{char_id}_{stem}" if char_id and char_id != "." else stem

# ── Fixed-size grid positioning ───────────────────────────────

def cols_for(card_w):
    return MAX_ATLAS // (card_w + PADDING)

def rows_per_atlas(card_h):
    return MAX_ATLAS // (card_h + PADDING)

def slot_to_atlas_and_xy(slot, card_w, card_h):
    cols = cols_for(card_w)
    rows = rows_per_atlas(card_h)
    slots_per_atlas = cols * rows
    atlas_idx = slot // slots_per_atlas
    local_slot = slot % slots_per_atlas
    col = local_slot % cols
    row = local_slot // cols
    x = col * (card_w + PADDING)
    y = row * (card_h + PADDING)
    return atlas_idx, x, y

# ── Pack a group ──────────────────────────────────────────────

def pack_group(group_id, entries, atlas_base, type_prefix, card_size, group_cache, force_repack):
    """
    entries:     list of (stem, resized_image, src_hash)
                 stem is already the final .tres name (includes char prefix)
    group_cache: dict stem -> {hash, slot, atlas_idx}
    Returns updated group_cache.
    """
    card_w, card_h = card_size

    if force_repack:
        group_cache = {}

    entry_map = {stem: (resized, src_hash) for stem, resized, src_hash in entries}

    # Assign slots — keep existing positions, append new at end
    next_slot = max((v["slot"] for v in group_cache.values()), default=-1) + 1
    slot_map = {}
    dirty_stems = set()

    for stem, cached in group_cache.items():
        if stem in entry_map:
            slot_map[stem] = cached["slot"]
            _, src_hash = entry_map[stem]
            if cached["hash"] != src_hash:
                dirty_stems.add(stem)

    for stem in entry_map:
        if stem not in slot_map:
            slot_map[stem] = next_slot
            next_slot += 1
            dirty_stems.add(stem)

    removed = set(group_cache) - set(entry_map)

    if not dirty_stems and not removed and not force_repack:
        print(f"  {group_id}: nothing changed, skipping.")
        return group_cache

    # Build canvases
    max_slot = max(slot_map.values(), default=0)
    cols = cols_for(card_w)
    rows = rows_per_atlas(card_h)
    slots_per_atlas = cols * rows
    atlas_count = (max_slot // slots_per_atlas) + 1
    canvases = [Image.new("RGB", (MAX_ATLAS, MAX_ATLAS), (0, 0, 0)) for _ in range(atlas_count)]

    for stem, slot in slot_map.items():
        resized, _ = entry_map[stem]
        atlas_idx, x, y = slot_to_atlas_and_xy(slot, card_w, card_h)
        canvases[atlas_idx].paste(resized, (x, y))

    # Write atlas PNGs cropped to used height
    atlas_res_paths = []
    for idx, canvas in enumerate(canvases):
        used_h = 0
        for stem, slot in slot_map.items():
            aidx, _, y = slot_to_atlas_and_xy(slot, card_w, card_h)
            if aidx == idx:
                used_h = max(used_h, y + card_h)
        cropped = canvas.crop((0, 0, MAX_ATLAS, used_h))
        filename = f"{atlas_base}_{group_id}_{idx}.png"
        res_path = f"{ATLAS_RES_BASE}/{filename}"
        atlas_res_paths.append(res_path)
        changed = save_image_if_changed(cropped, os.path.join(OUT_ATLASES, filename))
        if changed:
            print(f"  wrote atlas: {filename}")

    # Write .tres only for dirty or atlas-page-changed stems
    tres_written = 0
    for stem, slot in slot_map.items():
        atlas_idx, x, y = slot_to_atlas_and_xy(slot, card_w, card_h)
        cached = group_cache.get(stem, {})
        if stem not in dirty_stems and cached.get("atlas_idx") == atlas_idx:
            continue
        write_tres(
            os.path.join(OUT_TRES, f"{stem}.tres"),
            atlas_res_paths[atlas_idx],
            x, y, card_w, card_h,
            f"{type_prefix}_{stem}"
        )
        tres_written += 1

    # Remove .tres for deleted cards
    for stem in removed:
        tres_path = os.path.join(OUT_TRES, f"{stem}.tres")
        if os.path.exists(tres_path):
            os.remove(tres_path)
            print(f"  removed: {stem}.tres")

    print(f"  {group_id}: {len(entry_map)} cards, {atlas_count} atlas page(s), {tres_written} .tres updated")

    # Build updated cache
    new_cache = {}
    for stem, slot in slot_map.items():
        atlas_idx, _, _ = slot_to_atlas_and_xy(slot, card_w, card_h)
        _, src_hash = entry_map[stem]
        new_cache[stem] = {"hash": src_hash, "slot": slot, "atlas_idx": atlas_idx}

    return new_cache

# ── Main ──────────────────────────────────────────────────────

force_repack = "--repack" in sys.argv

cache = load_cache()
os.makedirs(OUT_TRES, exist_ok=True)
os.makedirs(OUT_ATLASES, exist_ok=True)

# Collect all cards
seen = set()
normal_groups   = {}  # char_id -> [(stem, resized, hash)]
ancient_entries = []  # [(stem, resized, hash)] — shared atlas, stem includes char prefix

for input_dir in INPUT_DIRS:
    full = os.path.join(SCRIPT_DIR, input_dir)
    if not os.path.exists(full):
        continue
    for root, _, files in os.walk(full):
        rel_folder = os.path.relpath(root, full)
        char_id = rel_folder.lower() if rel_folder != "." else "_"
        for file in sorted(files):
            if not file.lower().endswith(".png"):
                continue
            stem = os.path.splitext(file)[0]
            key = (rel_folder, stem)
            if key in seen:
                continue
            seen.add(key)

            path = os.path.join(root, file)
            img = Image.open(path).convert("RGBA")
            src_hash = file_hash(path)

            if is_ancient(img):
                resized = flatten_alpha(img.resize(ANCIENT_SIZE, Image.LANCZOS))
                ancient_entries.append((tres_name_for(char_id, stem), resized, src_hash))
                print(f"ancient [{input_dir}]: {os.path.relpath(path)}")
            else:
                resized = flatten_alpha(img.resize(NORMAL_SIZE, Image.LANCZOS))
                normal_groups.setdefault(char_id, []).append((stem, resized, src_hash))
                print(f"normal  [{input_dir}]: {os.path.relpath(path)}")

print()

# Pack normal cards — per character
normal_cache = cache.get("normal", {})
for char_id, entries in normal_groups.items():
    prefixed = [(tres_name_for(char_id, stem), resized, src_hash) for stem, resized, src_hash in entries]
    normal_cache[char_id] = pack_group(
        char_id, prefixed, NORMAL_BASE, "card",
        NORMAL_SIZE, normal_cache.get(char_id, {}), force_repack
    )

# Pack ancient cards — single shared atlas
ancient_cache = cache.get("ancient", {})
cache["ancient"] = {"shared": pack_group(
    "shared", ancient_entries, ANCIENT_BASE, "ancient",
    ANCIENT_SIZE, ancient_cache.get("shared", {}), force_repack
)}

cache["normal"] = normal_cache
save_cache(cache)

total_normal = sum(len(v) for v in normal_groups.values())
print(f"\nNormal:  {total_normal} cards across {len(normal_groups)} character(s)")
print(f"Ancient: {len(ancient_entries)} cards in shared atlas")
print("Done.")