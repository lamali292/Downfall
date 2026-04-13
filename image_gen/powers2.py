from PIL import Image
import os, math, hashlib, shutil

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
PADDING         = 1          # px gap between packed rects (avoids bleeding)

ATLAS_FILENAME        = "power_atlas.png"
SPRITE_ATLAS_FILENAME = "power_sprite_atlas.png"

ATLAS_RES_PATH        = "res://Downfall/images/atlases/power_atlas.png"
SPRITE_ATLAS_RES_PATH = "res://Downfall/images/atlases/power_sprite_atlas.png"

SPRITE_ONLY_FOLDERS   = {"sts2"}
CUSTOM_SUFFIX         = "_power"
# ============================================================

OUT_TRES        = os.path.join(OUT_ATLASES, ATLAS_SPRITES)
OUT_TRES_SPRITE = os.path.join(OUT_ATLASES, SPRITE_SPRITES)


# ------------------------------------------------------------------
# Shelf bin packer
# Shelves are sorted by height; each new rect is placed on the first
# shelf that fits, or opens a new shelf at the bottom.
# ------------------------------------------------------------------
class ShelfPacker:
    def __init__(self, width: int):
        self.width   = width
        self.shelves = []   # each entry: [y, next_x, shelf_h]
        self.height  = 0    # current canvas height used

    def pack(self, w: int, h: int):
        """Return (x, y) for a rect of size w×h, expanding canvas as needed."""
        best = None
        best_waste = None
        for shelf in self.shelves:
            sy, sx, sh = shelf
            if h <= sh and sx + w <= self.width:
                waste = sh - h
                if best_waste is None or waste < best_waste:
                    best       = shelf
                    best_waste = waste

        if best is not None:
            x = best[1]
            best[1] += w + PADDING
            return x, best[0]

        # Open a new shelf
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
    """Return (trimmed_img, left, top, right_pad, bottom_pad) where pads are
    the transparent margins stripped from the original image."""
    bbox = img.getbbox()          # (left, top, right, bottom) of non-transparent pixels
    if bbox is None:
        bbox = (0, 0, img.width, img.height)
    trimmed = img.crop(bbox)
    left   = bbox[0]
    top    = bbox[1]
    right  = img.width  - bbox[2]
    bottom = img.height - bbox[3]
    return trimmed, left, top, right, bottom


def deterministic_uid(name: str, length=7) -> str:
    return hashlib.md5(name.encode()).hexdigest()[:length]


def write_tres(path, atlas_res_path, x, y, w, h, name: str,
               margin_l=0, margin_t=0, margin_r=0, margin_b=0):
    """Write an AtlasTexture .tres with optional margin for trimmed sprites."""
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
    with open(path, "w") as f:
        f.write(content)


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
# 1. Collect & resize all images
# ---------------------------------------------------------------
entries = []   # (stem, big, small, sprite, is_sprite_only)
for root, dirs, files in os.walk(INPUT_DIR):
    is_sprite_only = os.path.basename(root) in SPRITE_ONLY_FOLDERS
    for file in sorted(files):          # sorted → deterministic atlas layout
        if file.lower().endswith(".png"):
            in_path = os.path.join(root, file)
            stem    = os.path.splitext(file)[0]
            img     = Image.open(in_path).convert("RGBA")
            big     = img.resize((BIG_SIZE,   BIG_SIZE),   Image.LANCZOS) if not is_sprite_only else None
            small   = img.resize((ATLAS_SIZE, ATLAS_SIZE), Image.LANCZOS) if not is_sprite_only else None
            sprite  = img.resize((SPRITE_SIZE,SPRITE_SIZE),Image.LANCZOS)
            entries.append((stem, big, small, sprite, is_sprite_only))
            print("collected:", in_path, "→", stem, ("(sprite only)" if is_sprite_only else ""))

non_sprite_only = [(s, big, sm, sp) for s, big, sm, sp, iso in entries if not iso]
all_entries     = entries

# ---------------------------------------------------------------
# 2. Trim alpha from every image that will be packed
# ---------------------------------------------------------------
def trimmed_data(images_iter, base_size):
    """
    images_iter: iterable of (stem, img_at_base_size)
    Returns list of (stem, trimmed_img, ml, mt, mr, mb)
    """
    out = []
    for stem, img in images_iter:
        trimmed, ml, mt, mr, mb = trim_alpha(img)
        out.append((stem, trimmed, ml, mt, mr, mb))
    return out

atlas_data  = trimmed_data(((s, sm) for s, _, sm, _ in non_sprite_only), ATLAS_SIZE)
sprite_data = trimmed_data(((s, sp) for s, _, _, sp, _ in all_entries),  SPRITE_SIZE)

# ---------------------------------------------------------------
# 3. Estimate a good atlas width, then bin-pack
# ---------------------------------------------------------------
def pack_all(data, label):
    """
    data: list of (stem, trimmed_img, ml, mt, mr, mb)
    Returns (packer, placements) where placements[i] = (x, y)
    """
    total_area = sum((d[1].width + PADDING) * (d[1].height + PADDING) for d in data)
    # Start with a power-of-two width close to sqrt(area)*1.2
    est_side = next_power_of_two(int(math.sqrt(total_area) * 1.2))
    est_side = max(est_side, 64)

    # Sort by height descending for better shelf utilisation
    order = sorted(range(len(data)), key=lambda i: -data[i][1].height)

    for attempt in range(4):               # try up to 4 width doublings
        packer     = ShelfPacker(est_side)
        placements = [None] * len(data)
        for i in order:
            w = data[i][1].width
            h = data[i][1].height
            placements[i] = packer.pack(w, h)
        cw, ch = packer.canvas_size()
        print(f"  [{label}] attempt {attempt+1}: width={est_side}, canvas={cw}×{ch}")
        if ch <= est_side * 2:             # acceptable aspect ratio
            break
        est_side <<= 1

    return packer, placements

print("\nPacking atlas textures…")
atlas_packer,  atlas_placements  = pack_all(atlas_data,  "atlas")
print("Packing sprite atlas textures…")
sprite_packer, sprite_placements = pack_all(sprite_data, "sprite")

# ---------------------------------------------------------------
# 4. Render atlas images
# ---------------------------------------------------------------
aw = next_power_of_two(atlas_packer.canvas_size()[0])
ah = atlas_packer.canvas_size()[1]   # exact — no wasted rows
sw = next_power_of_two(sprite_packer.canvas_size()[0])
sh = sprite_packer.canvas_size()[1]  # exact — no wasted rows

atlas        = Image.new("RGBA", (aw, ah), (0, 0, 0, 0))
sprite_atlas = Image.new("RGBA", (sw, sh), (0, 0, 0, 0))

# --- main atlas + big images ---
for i, (stem, trimmed, ml, mt, mr, mb) in enumerate(atlas_data):
    ax, ay = atlas_placements[i]
    atlas.paste(trimmed, (ax, ay))
    tw, th = trimmed.size
    tres_name = f"{stem}{CUSTOM_SUFFIX}"
    write_tres(
        os.path.join(OUT_TRES, f"{tres_name}.tres"),
        ATLAS_RES_PATH,
        ax, ay, tw, th, f"{stem}_atlas",
        ml, mt, mr, mb
    )
    # big image: save original (untrimmed) for direct use
    non_sprite_only[i][1].save(os.path.join(OUT_POWERS, f"{stem}{CUSTOM_SUFFIX}.png"))

# --- sprite atlas ---
for i, (stem, trimmed, ml, mt, mr, mb) in enumerate(sprite_data):
    sx, sy = sprite_placements[i]
    sprite_atlas.paste(trimmed, (sx, sy))
    tw, th = trimmed.size
    orig_iso = all_entries[i][4]
    filename = f"{stem}.tres" if orig_iso else f"{stem}{CUSTOM_SUFFIX}.tres"
    write_tres(
        os.path.join(OUT_TRES_SPRITE, filename),
        SPRITE_ATLAS_RES_PATH,
        sx, sy, tw, th, f"{stem}_sprite",
        ml, mt, mr, mb
    )

atlas.save(       os.path.join(OUT_ATLASES, ATLAS_FILENAME))
sprite_atlas.save(os.path.join(OUT_ATLASES, SPRITE_ATLAS_FILENAME))

aw_used, ah_used = atlas_packer.canvas_size()
sw_used, sh_used = sprite_packer.canvas_size()
print(f"\n{ATLAS_FILENAME}:        canvas {aw}×{ah}  (content {aw_used}×{ah_used}), {len(atlas_data)} images")
print(f"{SPRITE_ATLAS_FILENAME}: canvas {sw}×{sh}  (content {sw_used}×{sh_used}), {len(sprite_data)} images")
print("Done!")