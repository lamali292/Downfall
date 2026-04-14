from PIL import Image
import os, hashlib, json, string

# ============================================================
# CONFIG
# ============================================================
INPUT_DIRS      = ["cards", "cards_beta", "cards_missing"]  # priority: first wins
OUT_PORTRAITS   = "../Downfall/images/card_portraits"
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

OUT_TRES  = os.path.join(OUT_ATLASES, SPRITES_DIR)
UID_CHARS = string.ascii_lowercase + string.digits

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

def clean_dir(folder, extensions):
    os.makedirs(folder, exist_ok=True)
    for f in os.listdir(folder):
        if any(f.endswith(ext) for ext in extensions):
            os.remove(os.path.join(folder, f))

# ── Cache check ───────────────────────────────────────────────

def collect_input_hashes():
    hashes = {}
    for input_dir in INPUT_DIRS:
        if not os.path.exists(input_dir):
            continue
        for root, dirs, files in os.walk(input_dir):
            for file in sorted(files):
                if file.lower().endswith(".png"):
                    path = os.path.join(root, file)
                    hashes[path] = file_hash(path)
    return hashes

cache          = load_cache()
current_hashes = collect_input_hashes()

existing = [
    f for f in (os.listdir(OUT_ATLASES) if os.path.exists(OUT_ATLASES) else [])
    if (f.startswith(NORMAL_BASE) or f.startswith(ANCIENT_BASE)) and f.endswith(".png")
]

if existing and cache.get("input_hashes") == current_hashes:
    print("Nothing changed, skipping.")
    exit(0)

# ── Prepare output dirs ───────────────────────────────────────

clean_dir(OUT_TRES,      [".tres"])
clean_dir(OUT_PORTRAITS, [".png", ".import"])
os.makedirs(OUT_ATLASES, exist_ok=True)

for f in os.listdir(OUT_ATLASES):
    if (f.startswith(NORMAL_BASE) or f.startswith(ANCIENT_BASE)) and f.endswith(".png"):
        os.remove(os.path.join(OUT_ATLASES, f))

# ── Collect — first source wins per (rel_folder, stem) ───────

seen            = set()   # (rel_folder, stem)
normal_entries  = []      # (stem, rel_folder, resized_rgb)
ancient_entries = []

for input_dir in INPUT_DIRS:
    if not os.path.exists(input_dir):
        continue
    for root, dirs, files in os.walk(input_dir):
        rel_folder = os.path.relpath(root, input_dir)
        for file in sorted(files):
            if not file.lower().endswith(".png"):
                continue
            stem = os.path.splitext(file)[0]
            key  = (rel_folder, stem)
            if key in seen:
                continue  # higher-priority source already has this
            seen.add(key)

            path = os.path.join(root, file)
            img  = Image.open(path).convert("RGBA")
            src  = os.path.relpath(path)

            # Copy full-size portrait
            dest_folder = os.path.join(OUT_PORTRAITS, rel_folder)
            os.makedirs(dest_folder, exist_ok=True)
            save_image_if_changed(flatten_alpha(img), os.path.join(dest_folder, f"{stem}.png"))

            if is_ancient(img):
                resized = flatten_alpha(img.resize(ANCIENT_SIZE, Image.LANCZOS))
                ancient_entries.append((stem, rel_folder, resized))
                print(f"ancient [{input_dir}]: {src}")
            else:
                resized = flatten_alpha(img.resize(NORMAL_SIZE, Image.LANCZOS))
                normal_entries.append((stem, rel_folder, resized))
                print(f"normal  [{input_dir}]: {src}")

# ── Packer ────────────────────────────────────────────────────

class StripPacker:
    def __init__(self, max_size):
        self.max_size = max_size
        self.x = 0
        self.y = 0
        self.row_h = 0

    def try_pack(self, w, h):
        if self.x + w <= self.max_size:
            pos = (self.x, self.y)
            self.x += w + PADDING
            self.row_h = max(self.row_h, h)
            return pos
        new_y = self.y + self.row_h + PADDING
        if new_y + h > self.max_size:
            return None
        self.y, self.x, self.row_h = new_y, w + PADDING, h
        return (0, new_y)

    def canvas_height(self):
        return self.y + self.row_h

def pack_entries(entries, atlas_base):
    atlases    = []
    placements = []

    def new_atlas():
        packer = StripPacker(MAX_ATLAS)
        canvas = Image.new("RGB", (MAX_ATLAS, MAX_ATLAS), (0, 0, 0))
        atlases.append((packer, canvas))
        return len(atlases) - 1

    new_atlas()

    for stem, rel_folder, resized in entries:
        w, h   = resized.size
        placed = False
        for idx, (packer, canvas) in enumerate(atlases):
            pos = packer.try_pack(w, h)
            if pos:
                canvas.paste(resized, pos)
                placements.append((idx, pos[0], pos[1]))
                placed = True
                break
        if not placed:
            idx = new_atlas()
            packer, canvas = atlases[idx]
            pos = packer.try_pack(w, h)
            canvas.paste(resized, pos)
            placements.append((idx, pos[0], pos[1]))

    atlas_res_paths = []
    for idx, (packer, canvas) in enumerate(atlases):
        used_h   = packer.canvas_height()
        cropped  = canvas.crop((0, 0, MAX_ATLAS, used_h))
        filename = f"{atlas_base}_{idx}.png"
        res_path = f"{ATLAS_RES_BASE}/{filename}"
        atlas_res_paths.append(res_path)
        if save_image_if_changed(cropped, os.path.join(OUT_ATLASES, filename)):
            print(f"wrote: {filename} ({MAX_ATLAS}x{used_h})")

    for i, (stem, rel_folder, resized) in enumerate(entries):
        atlas_idx, x, y = placements[i]
        w, h      = resized.size
        tres_name = stem if rel_folder == "." else f"{rel_folder.replace(os.sep, '_')}_{stem}"
        write_tres(
            os.path.join(OUT_TRES, f"{tres_name}.tres"),
            atlas_res_paths[atlas_idx],
            x, y, w, h,
            f"card_{tres_name}"
        )

    return len(atlases)

# ── Run ───────────────────────────────────────────────────────

n_normal  = pack_entries(normal_entries,  NORMAL_BASE)
n_ancient = pack_entries(ancient_entries, ANCIENT_BASE)

cache["input_hashes"] = current_hashes
save_cache(cache)

print(f"\nNormal:  {len(normal_entries)} cards in {n_normal} atlas page(s)")
print(f"Ancient: {len(ancient_entries)} cards in {n_ancient} atlas page(s)")
print("Done.")