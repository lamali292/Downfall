from PIL import Image
import os, math, hashlib, shutil, json

# ============================================================
# CONFIG
# ============================================================
INPUT_DIR       = "powers"
OUT_POWERS      = "../Downfall/images/powers"
OUT_ATLASES     = "../Downfall/images/atlases"
ATLAS_SPRITES   = "power_atlas.sprites"
SPRITE_SPRITES  = "power_sprite_atlas.sprites"

BIG_SIZE        = 256
ATLAS_SIZE      = 64
SPRITE_SIZE     = 24
PADDING         = 1

ATLAS_FILENAME        = "power_atlas.png"
SPRITE_ATLAS_FILENAME = "power_sprite_atlas.png"

ATLAS_RES_PATH        = "res://Downfall/images/atlases/power_atlas.png"
SPRITE_ATLAS_RES_PATH = "res://Downfall/images/atlases/power_sprite_atlas.png"

SPRITE_ONLY_FOLDERS   = {"sts2"}
CUSTOM_SUFFIX         = "_power"
CACHE_FILE            = ".powers_cache.json"
# ============================================================

OUT_TRES        = os.path.join(OUT_ATLASES, ATLAS_SPRITES)
OUT_TRES_SPRITE = os.path.join(OUT_ATLASES, SPRITE_SPRITES)


# ---------------------------------------------------------------
# Cache helpers
# ---------------------------------------------------------------
def file_hash(path: str) -> str:
    h = hashlib.md5()
    with open(path, "rb") as f:
        for chunk in iter(lambda: f.read(65536), b""):
            h.update(chunk)
    return h.hexdigest()

def load_cache() -> dict:
    if os.path.exists(CACHE_FILE):
        with open(CACHE_FILE) as f:
            return json.load(f)
    return {}

def save_cache(cache: dict):
    with open(CACHE_FILE, "w") as f:
        json.dump(cache, f, indent=2)

def write_if_changed(path: str, data: bytes):
    """Write file only if content has changed."""
    if os.path.exists(path):
        with open(path, "rb") as f:
            if f.read() == data:
                return False
    with open(path, "wb") as f:
        f.write(data)
    return True

def save_image_if_changed(img: Image.Image, path: str) -> bool:
    """Save PIL image only if it differs from what's on disk."""
    import io
    buf = io.BytesIO()
    img.save(buf, format="PNG")
    return write_if_changed(path, buf.getvalue())


# ---------------------------------------------------------------
# Check if any input changed since last run
# ---------------------------------------------------------------
def collect_input_hashes() -> dict:
    hashes = {}
    for root, dirs, files in os.walk(INPUT_DIR):
        for file in sorted(files):
            if file.lower().endswith(".png"):
                path = os.path.join(root, file)
                hashes[path] = file_hash(path)
    return hashes

cache = load_cache()
current_hashes = collect_input_hashes()

outputs_exist = (
    os.path.exists(os.path.join(OUT_ATLASES, ATLAS_FILENAME)) and
    os.path.exists(os.path.join(OUT_ATLASES, SPRITE_ATLAS_FILENAME))
)

if outputs_exist and cache.get("input_hashes") == current_hashes:
    print("Nothing changed, skipping.")
    exit(0)


# ---------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------
class ShelfPacker:
    def __init__(self, width: int):
        self.width   = width
        self.shelves = []
        self.height  = 0

    def pack(self, w: int, h: int):
        best, best_waste = None, None
        for shelf in self.shelves:
            sy, sx, sh = shelf
            if h <= sh and sx + w <= self.width:
                waste = sh - h
                if best_waste is None or waste < best_waste:
                    best, best_waste = shelf, waste
        if best is not None:
            x = best[1]
            best[1] += w + PADDING
            return x, best[0]
        y = self.height
        self.shelves.append([y, w + PADDING, h])
        self.height = y + h + PADDING
        return 0, y

    def canvas_size(self):
        return self.width, self.height


def next_power_of_two(n):
    p = 1
    while p < n:
        p <<= 1
    return p


def trim_alpha(img: Image.Image):
    bbox = img.getbbox() or (0, 0, img.width, img.height)
    trimmed = img.crop(bbox)
    return trimmed, bbox[0], bbox[1], img.width - bbox[2], img.height - bbox[3]


def deterministic_uid(name: str, length=7) -> str:
    return hashlib.md5(name.encode()).hexdigest()[:length]


def write_tres(path, atlas_res_path, x, y, w, h, name,
               margin_l=0, margin_t=0, margin_r=0, margin_b=0):
    margin_line = ""
    if any((margin_l, margin_t, margin_r, margin_b)):
        margin_line = f"margin = Rect2({margin_l}, {margin_t}, {margin_r}, {margin_b})\n"
    content = (
        f'[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(name)}"]\n'
        f'[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]\n'
        f'[resource]\n'
        f'atlas = ExtResource("1")\n'
        f'region = Rect2({x}, {y}, {w}, {h})\n'
        f'{margin_line}'
    )
    write_if_changed(path, content.encode())


def clean_dir(folder, extensions):
    if not os.path.exists(folder):
        os.makedirs(folder)
        return
    for f in os.listdir(folder):
        if any(f.endswith(ext) for ext in extensions):
            os.remove(os.path.join(folder, f))

clean_dir(OUT_POWERS,      [".png", ".import"])
clean_dir(OUT_TRES,        [".tres"])
clean_dir(OUT_TRES_SPRITE, [".tres"])
os.makedirs(OUT_ATLASES, exist_ok=True)

# ---------------------------------------------------------------
# 1. Collect & resize
# ---------------------------------------------------------------
entries = []
for root, dirs, files in os.walk(INPUT_DIR):
    is_sprite_only = os.path.basename(root) in SPRITE_ONLY_FOLDERS
    for file in sorted(files):
        if file.lower().endswith(".png"):
            in_path = os.path.join(root, file)
            stem    = os.path.splitext(file)[0]
            img     = Image.open(in_path).convert("RGBA")
            big     = img.resize((BIG_SIZE,    BIG_SIZE),    Image.LANCZOS) if not is_sprite_only else None
            small   = img.resize((ATLAS_SIZE,  ATLAS_SIZE),  Image.LANCZOS) if not is_sprite_only else None
            sprite  = img.resize((SPRITE_SIZE, SPRITE_SIZE), Image.LANCZOS)
            entries.append((stem, big, small, sprite, is_sprite_only))
            print("collected:", in_path, "->", stem, ("(sprite only)" if is_sprite_only else ""))

non_sprite_only = [(s, big, sm, sp) for s, big, sm, sp, iso in entries if not iso]

# ---------------------------------------------------------------
# 2. Trim alpha
# ---------------------------------------------------------------
def trimmed_data(images_iter):
    return [(stem, *trim_alpha(img)) for stem, img in images_iter]

atlas_data  = trimmed_data((s, sm) for s, _, sm, _ in non_sprite_only)
sprite_data = trimmed_data((s, sp) for s, _, _, sp, _ in entries)

# ---------------------------------------------------------------
# 3. Pack
# ---------------------------------------------------------------
def pack_all(data, label):
    total_area = sum((d[1].width + PADDING) * (d[1].height + PADDING) for d in data)
    est_side   = max(next_power_of_two(int(math.sqrt(total_area) * 1.2)), 64)
    order      = sorted(range(len(data)), key=lambda i: -data[i][1].height)
    for attempt in range(4):
        packer     = ShelfPacker(est_side)
        placements = [None] * len(data)
        for i in order:
            placements[i] = packer.pack(data[i][1].width, data[i][1].height)
        cw, ch = packer.canvas_size()
        print(f"  [{label}] attempt {attempt+1}: width={est_side}, canvas={cw}x{ch}")
        if ch <= est_side * 2:
            break
        est_side <<= 1
    return packer, placements

print("\nPacking atlas textures...")
atlas_packer,  atlas_placements  = pack_all(atlas_data,  "atlas")
print("Packing sprite atlas textures...")
sprite_packer, sprite_placements = pack_all(sprite_data, "sprite")

# ---------------------------------------------------------------
# 4. Render & write — skip unchanged files
# ---------------------------------------------------------------
aw = next_power_of_two(atlas_packer.canvas_size()[0])
ah = atlas_packer.canvas_size()[1]
sw = next_power_of_two(sprite_packer.canvas_size()[0])
sh = sprite_packer.canvas_size()[1]

atlas        = Image.new("RGBA", (aw, ah), (0, 0, 0, 0))
sprite_atlas = Image.new("RGBA", (sw, sh), (0, 0, 0, 0))

for i, (stem, trimmed, ml, mt, mr, mb) in enumerate(atlas_data):
    ax, ay = atlas_placements[i]
    atlas.paste(trimmed, (ax, ay))
    tw, th    = trimmed.size
    tres_name = f"{stem}{CUSTOM_SUFFIX}"
    write_tres(os.path.join(OUT_TRES, f"{tres_name}.tres"),
               ATLAS_RES_PATH, ax, ay, tw, th, f"{stem}_atlas", ml, mt, mr, mb)
    big_path = os.path.join(OUT_POWERS, f"{tres_name}.png")
    if save_image_if_changed(non_sprite_only[i][1], big_path):
        print("updated big:", tres_name)

for i, (stem, trimmed, ml, mt, mr, mb) in enumerate(sprite_data):
    sx, sy   = sprite_placements[i]
    sprite_atlas.paste(trimmed, (sx, sy))
    tw, th   = trimmed.size
    orig_iso = entries[i][4]
    filename = f"{stem}.tres" if orig_iso else f"{stem}{CUSTOM_SUFFIX}.tres"
    write_tres(os.path.join(OUT_TRES_SPRITE, filename),
               SPRITE_ATLAS_RES_PATH, sx, sy, tw, th, f"{stem}_sprite", ml, mt, mr, mb)

save_image_if_changed(atlas,        os.path.join(OUT_ATLASES, ATLAS_FILENAME))
save_image_if_changed(sprite_atlas, os.path.join(OUT_ATLASES, SPRITE_ATLAS_FILENAME))

# ---------------------------------------------------------------
# 5. Save cache
# ---------------------------------------------------------------
cache["input_hashes"] = current_hashes
save_cache(cache)

aw_used, ah_used = atlas_packer.canvas_size()
sw_used, sh_used = sprite_packer.canvas_size()
print(f"\n{ATLAS_FILENAME}: canvas {aw}x{ah} (content {aw_used}x{ah_used}), {len(atlas_data)} images")
print(f"{SPRITE_ATLAS_FILENAME}: canvas {sw}x{sh} (content {sw_used}x{sh_used}), {len(sprite_data)} images")
print("Done!")