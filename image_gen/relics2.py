from PIL import Image
import numpy as np
from scipy.ndimage import gaussian_filter
import os, math, hashlib, json, io, string

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PARENT     = os.path.join(SCRIPT_DIR, "..")

IMG_SIZE       = 93
INSET          = 4
REGION_SIZE    = 85
PADDING        = 1
CROP_BOX       = (56, 56, 200, 200)
OUTLINE_RADIUS = 10
OUTLINE_SIGMA  = 0.5
CACHE_FILE     = os.path.join(SCRIPT_DIR, ".relics_cache.json")
UID_CHARS      = string.ascii_lowercase + string.digits

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

def write_if_changed(path, data: bytes):
    if os.path.exists(path):
        with open(path, "rb") as f:
            if f.read() == data: return False
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "wb") as f: f.write(data)
    return True

def save_image_if_changed(img, path):
    buf = io.BytesIO()
    img.save(buf, format="PNG")
    return write_if_changed(path, buf.getvalue())

def next_power_of_two(n):
    p = 1
    while p < n: p <<= 1
    return p

def write_tres(path, atlas_res_path, x, y, w, h, name):
    content = (
        f'[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(name)}"]\n'
        f'[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]\n'
        f'[resource]\natlas = ExtResource("1")\nregion = Rect2({x}, {y}, {w}, {h})\n'
    )
    write_if_changed(path, content.encode())

class ShelfPacker:
    def __init__(self, width):
        self.width = width; self.shelves = []; self.height = 0
    def pack(self, w, h):
        best, best_waste = None, None
        for shelf in self.shelves:
            sy, sx, sh = shelf
            if h <= sh and sx + w <= self.width:
                waste = sh - h
                if best_waste is None or waste < best_waste:
                    best, best_waste = shelf, waste
        if best:
            x = best[1]; best[1] += w + PADDING; return x, best[0]
        y = self.height; self.shelves.append([y, w + PADDING, h]); self.height = y + h + PADDING
        return 0, y
    def canvas_size(self): return self.width, self.height

def process_image(path):
    img          = Image.open(path).convert("RGBA")
    img_cropped  = img.crop(CROP_BOX)
    img_upscaled = img_cropped.resize((256, 256), Image.LANCZOS)
    alpha        = np.array(img_upscaled.split()[3])
    radius       = OUTLINE_RADIUS
    size         = radius * 2 + 1
    y, x         = np.ogrid[-radius:radius+1, -radius:radius+1]
    kernel       = (x*x + y*y) <= radius*radius
    padded       = np.pad(alpha, radius)
    dilated      = np.zeros_like(alpha)
    for dy in range(size):
        for dx in range(size):
            if kernel[dy, dx]:
                dilated = np.maximum(dilated, padded[dy:dy+alpha.shape[0], dx:dx+alpha.shape[1]])
    outline_alpha            = gaussian_filter(dilated.astype(np.float32), sigma=OUTLINE_SIGMA)
    outline_alpha            = np.clip(outline_alpha, 0, 255).astype(np.uint8)
    outline                  = np.zeros((*alpha.shape, 4), dtype=np.uint8)
    outline[..., :3]         = 255
    outline[..., 3]          = outline_alpha
    black_outline            = np.zeros((*alpha.shape, 4), dtype=np.uint8)
    black_outline[..., 3]    = (outline_alpha * 0.5).astype(np.uint8)
    big                      = Image.alpha_composite(Image.fromarray(black_outline, "RGBA"), img_upscaled)
    outline_ds               = Image.fromarray(outline, "RGBA").resize((IMG_SIZE, IMG_SIZE), Image.LANCZOS)
    image_ds                 = img_cropped.resize((IMG_SIZE, IMG_SIZE), Image.LANCZOS)
    return big, outline_ds, image_ds

cache = json.load(open(CACHE_FILE)) if os.path.exists(CACHE_FILE) else {}

# Auto-discover characters from relics/ subfolders
char_ids = set()
relics_root = os.path.join(SCRIPT_DIR, "relics")
if os.path.exists(relics_root):
    for entry in os.listdir(relics_root):
        if os.path.isdir(os.path.join(relics_root, entry)):
            char_ids.add(entry.lower())

for char_id in sorted(char_ids):
    char_proj = next((e for e in os.listdir(PARENT)
                      if e.lower() == char_id
                      and os.path.isdir(os.path.join(PARENT, e))
                      and not e.endswith("Code")), None)
    if not char_proj:
        print(f"No project folder for {char_id}, skipping"); continue

    input_dir   = os.path.join(relics_root, char_id)
    out_atlases = os.path.join(PARENT, char_proj, char_proj, "images", "atlases")
    out_relics  = os.path.join(PARENT, char_proj, char_proj, "images", "relics")
    out_tres    = os.path.join(out_atlases, "relic_atlas.sprites")
    res_base    = f"res://{char_proj}/images/atlases"
    atlas_res   = f"{res_base}/relic_atlas.png"
    outline_res = f"{res_base}/relic_outline_atlas.png"

    os.makedirs(out_atlases, exist_ok=True)
    os.makedirs(out_relics,  exist_ok=True)
    os.makedirs(out_tres,    exist_ok=True)

    print(f"\n=== {char_id} -> {char_proj} ===")

    current_hashes = {f: file_hash(os.path.join(input_dir, f))
                      for f in sorted(os.listdir(input_dir))
                      if f.lower().endswith(".png")}

    char_cache = cache.get(char_id, {})
    outputs_exist = (os.path.exists(os.path.join(out_atlases, "relic_atlas.png")) and
                     os.path.exists(os.path.join(out_atlases, "relic_outline_atlas.png")))
    if outputs_exist and char_cache.get("input_hashes") == current_hashes:
        print(f"  nothing changed, skipping"); continue

    changed = {k for k, v in current_hashes.items()
               if char_cache.get("input_hashes", {}).get(k) != v}

    entries = []
    for file in sorted(os.listdir(input_dir)):
        if not file.lower().endswith(".png"): continue
        stem = os.path.splitext(file)[0]
        big, outline_ds, image_ds = process_image(os.path.join(input_dir, file))
        entries.append((stem, big, outline_ds, image_ds, file in changed))
        print(f"  processed: {file}" + (" (changed)" if file in changed else ""))

    if not entries:
        print(f"  no images"); continue

    n = len(entries)
    est_width = max(next_power_of_two(int(math.sqrt(n * (IMG_SIZE + PADDING) ** 2) * 1.2)), IMG_SIZE + PADDING)
    packer     = ShelfPacker(est_width)
    placements = [packer.pack(IMG_SIZE, IMG_SIZE) for _ in entries]
    cw, ch     = packer.canvas_size()
    aw, ah     = next_power_of_two(cw), ch

    atlas         = Image.new("RGBA", (aw, ah), (0,0,0,0))
    outline_atlas = Image.new("RGBA", (aw, ah), (0,0,0,0))

    for i, (stem, big, outline_ds, image_ds, is_changed) in enumerate(entries):
        x, y = placements[i]
        atlas.paste(image_ds,   (x, y))
        outline_atlas.paste(outline_ds, (x, y))
        if is_changed:
            if save_image_if_changed(big, os.path.join(out_relics, f"{stem}.png")):
                print(f"  updated big: {stem}")

    save_image_if_changed(atlas,         os.path.join(out_atlases, "relic_atlas.png"))
    save_image_if_changed(outline_atlas, os.path.join(out_atlases, "relic_outline_atlas.png"))

    for i, (stem, _, _, _, _) in enumerate(entries):
        x, y = placements[i]
        write_tres(os.path.join(out_tres, f"{stem}.tres"),
                   atlas_res, x + INSET, y + INSET, REGION_SIZE, REGION_SIZE, f"{char_id}_{stem}")
        write_tres(os.path.join(out_tres, f"{stem}_outline.tres"),
                   outline_res, x + INSET, y + INSET, REGION_SIZE, REGION_SIZE, f"{char_id}_{stem}_outline")

    char_cache["input_hashes"] = current_hashes
    cache[char_id] = char_cache

with open(CACHE_FILE, "w") as f: json.dump(cache, f, indent=2)
print("\nDone!")