from PIL import Image
import numpy as np
from scipy.ndimage import gaussian_filter
import os, math, hashlib, json, io

# ============================================================
# CONFIG
# ============================================================
INPUT_DIR       = "relics"
OUT_RELICS      = "../Downfall/images/relics"
OUT_ATLASES     = "../Downfall/images/atlases"
ATLAS_SPRITES   = "relic_atlas.sprites"

IMG_SIZE        = 93
INSET           = 4
REGION_SIZE     = 85
PADDING         = 1

ATLAS_FILENAME         = "relic_atlas.png"
OUTLINE_ATLAS_FILENAME = "relic_outline_atlas.png"

ATLAS_RES_PATH         = "res://Downfall/images/atlases/relic_atlas.png"
OUTLINE_ATLAS_RES_PATH = "res://Downfall/images/atlases/relic_outline_atlas.png"

CROP_BOX        = (56, 56, 200, 200)
OUTLINE_RADIUS  = 10
OUTLINE_SIGMA   = 0.5
CACHE_FILE      = ".relics_cache.json"
# ============================================================

OUT_TRES = os.path.join(OUT_ATLASES, ATLAS_SPRITES)


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

def write_if_changed(path: str, data: bytes) -> bool:
    if os.path.exists(path):
        with open(path, "rb") as f:
            if f.read() == data:
                return False
    with open(path, "wb") as f:
        f.write(data)
    return True

def save_image_if_changed(img: Image.Image, path: str) -> bool:
    buf = io.BytesIO()
    img.save(buf, format="PNG")
    return write_if_changed(path, buf.getvalue())


# ---------------------------------------------------------------
# UID helpers
# ---------------------------------------------------------------
def read_import_uid(import_path: str) -> str:
    if not os.path.exists(import_path):
        return ""
    with open(import_path) as f:
        for line in f:
            if line.startswith("uid="):
                return line.strip().split('"')[1]  # e.g. uid://i38f3wjmpkc3
    return ""

def deterministic_uid(name: str, length=7) -> str:
    return hashlib.md5(name.encode()).hexdigest()[:length]


# ---------------------------------------------------------------
# Early exit if nothing changed
# ---------------------------------------------------------------
def collect_input_hashes() -> dict:
    return {
        f: file_hash(os.path.join(INPUT_DIR, f))
        for f in sorted(os.listdir(INPUT_DIR))
        if f.lower().endswith(".png")
    }

cache = load_cache()
current_hashes = collect_input_hashes()

outputs_exist = (
    os.path.exists(os.path.join(OUT_ATLASES, ATLAS_FILENAME)) and
    os.path.exists(os.path.join(OUT_ATLASES, OUTLINE_ATLAS_FILENAME))
)

if outputs_exist and cache.get("input_hashes") == current_hashes:
    print("Relics: nothing changed, skipping.")
    exit(0)

changed = {k for k, v in current_hashes.items()
           if cache.get("input_hashes", {}).get(k) != v}
print(f"Changed/new relics: {changed or 'none (atlas layout changed)'}")


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


def clean_dir(folder, extensions):
    if not os.path.exists(folder):
        os.makedirs(folder); return
    for f in os.listdir(folder):
        if any(f.endswith(ext) for ext in extensions):
            os.remove(os.path.join(folder, f))

clean_dir(OUT_RELICS, [".png", ".import"])
clean_dir(OUT_TRES,   [".tres"])
os.makedirs(OUT_ATLASES, exist_ok=True)


def write_tres(path, atlas_res_path, atlas_uid, x, y, w, h, name):
    uid_part = f' uid="{atlas_uid}"' if atlas_uid else ""
    content = (
        f'[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(name)}"]\n'
        f'[ext_resource type="Texture2D"{uid_part} path="{atlas_res_path}" id="1"]\n'
        f'[resource]\n'
        f'atlas = ExtResource("1")\n'
        f'region = Rect2({x}, {y}, {w}, {h})\n'
    )
    write_if_changed(path, content.encode())


def process_image(path, crop_box=CROP_BOX, radius=OUTLINE_RADIUS, sigma=OUTLINE_SIGMA):
    img          = Image.open(path).convert("RGBA")
    img_cropped  = img.crop(crop_box)
    img_upscaled = img_cropped.resize((256, 256), Image.LANCZOS)
    alpha        = np.array(img_upscaled.split()[3])
    size         = radius * 2 + 1
    y, x         = np.ogrid[-radius:radius+1, -radius:radius+1]
    kernel       = (x*x + y*y) <= radius*radius
    padded       = np.pad(alpha, radius)
    dilated      = np.zeros_like(alpha)
    for dy in range(size):
        for dx in range(size):
            if kernel[dy, dx]:
                dilated = np.maximum(dilated, padded[dy:dy+alpha.shape[0], dx:dx+alpha.shape[1]])
    outline_alpha        = gaussian_filter(dilated.astype(np.float32), sigma=sigma)
    outline_alpha        = np.clip(outline_alpha, 0, 255).astype(np.uint8)
    outline              = np.zeros((*alpha.shape, 4), dtype=np.uint8)
    outline[..., :3]     = 255
    outline[..., 3]      = outline_alpha
    black_outline        = np.zeros((*alpha.shape, 4), dtype=np.uint8)
    black_outline[...,3] = (outline_alpha * 0.5).astype(np.uint8)
    big                  = Image.alpha_composite(Image.fromarray(black_outline, "RGBA"), img_upscaled)
    outline_ds           = Image.fromarray(outline, "RGBA").resize((IMG_SIZE, IMG_SIZE), Image.LANCZOS)
    image_ds             = img_cropped.resize((IMG_SIZE, IMG_SIZE), Image.LANCZOS)
    return big, outline_ds, image_ds


# ---------------------------------------------------------------
# 1. Collect & process
# ---------------------------------------------------------------
entries = []
for file in sorted(os.listdir(INPUT_DIR)):
    if file.lower().endswith(".png"):
        stem    = os.path.splitext(file)[0]
        in_path = os.path.join(INPUT_DIR, file)
        big, outline_ds, image_ds = process_image(in_path)
        entries.append((stem, big, outline_ds, image_ds, file in changed))
        print("processed:", file, "->", stem, ("(changed)" if file in changed else ""))

n = len(entries)

# ---------------------------------------------------------------
# 2. Bin-pack
# ---------------------------------------------------------------
est_width = next_power_of_two(int(math.sqrt(n * (IMG_SIZE + PADDING) ** 2) * 1.2))
est_width = max(est_width, IMG_SIZE + PADDING)

packer     = ShelfPacker(est_width)
placements = [packer.pack(IMG_SIZE, IMG_SIZE) for _ in entries]

cw, ch = packer.canvas_size()
aw = next_power_of_two(cw)
ah = ch

print(f"\nPacking: width={aw}, canvas={cw}x{ah}, {n} images")

# ---------------------------------------------------------------
# 3. Render
# ---------------------------------------------------------------
atlas         = Image.new("RGBA", (aw, ah), (0, 0, 0, 0))
outline_atlas = Image.new("RGBA", (aw, ah), (0, 0, 0, 0))

# Read atlas UIDs after saving (import files may not exist yet on first run)
atlas_import_path         = os.path.join(OUT_ATLASES, ATLAS_FILENAME + ".import")
outline_atlas_import_path = os.path.join(OUT_ATLASES, OUTLINE_ATLAS_FILENAME + ".import")

for i, (stem, big, outline_ds, image_ds, is_changed) in enumerate(entries):
    x, y = placements[i]
    atlas.paste(image_ds,   (x, y))
    outline_atlas.paste(outline_ds, (x, y))

    big_path = os.path.join(OUT_RELICS, f"{stem}.png")
    if is_changed:
        if save_image_if_changed(big, big_path):
            print("updated big:", stem)

save_image_if_changed(atlas,         os.path.join(OUT_ATLASES, ATLAS_FILENAME))
save_image_if_changed(outline_atlas, os.path.join(OUT_ATLASES, OUTLINE_ATLAS_FILENAME))

# Read UIDs after atlas files are written (Godot must have imported them already)
atlas_uid         = read_import_uid(atlas_import_path)
outline_atlas_uid = read_import_uid(outline_atlas_import_path)

for i, (stem, big, outline_ds, image_ds, is_changed) in enumerate(entries):
    x, y = placements[i]
    write_tres(os.path.join(OUT_TRES, f"{stem}.tres"),
               ATLAS_RES_PATH, atlas_uid, x + INSET, y + INSET, REGION_SIZE, REGION_SIZE, stem)
    write_tres(os.path.join(OUT_TRES, f"{stem}_outline.tres"),
               OUTLINE_ATLAS_RES_PATH, outline_atlas_uid, x + INSET, y + INSET, REGION_SIZE, REGION_SIZE, f"{stem}_outline")

# ---------------------------------------------------------------
# 4. Save cache
# ---------------------------------------------------------------
cache["input_hashes"] = current_hashes
save_cache(cache)

print(f"\n{ATLAS_FILENAME}:         {aw}x{ah}px, {n} images")
print(f"{OUTLINE_ATLAS_FILENAME}: {aw}x{ah}px, {n} images")
print("Done!")