from PIL import Image
import os, math, hashlib, json, string, sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PARENT     = os.path.join(SCRIPT_DIR, "..")

INPUT_SUBDIRS       = ["powers", "powers_beta", "powers_missing"]
SPRITE_ONLY_FOLDERS = {"sts2"}
CUSTOM_SUFFIX       = "_power"
CACHE_FILE          = os.path.join(SCRIPT_DIR, ".powers_cache.json")

BIG_SIZE    = 256
ATLAS_SIZE  = 64
SPRITE_SIZE = 24
PADDING     = 1
UID_CHARS   = string.ascii_lowercase + string.digits

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
    import io
    buf = io.BytesIO()
    img.save(buf, format="PNG")
    return write_if_changed(path, buf.getvalue())

def next_power_of_two(n):
    p = 1
    while p < n: p <<= 1
    return p

def trim_alpha(img):
    bbox = img.getbbox() or (0, 0, img.width, img.height)
    return img.crop(bbox), bbox[0], bbox[1], img.width - bbox[2], img.height - bbox[3]

def write_tres(path, atlas_res_path, x, y, w, h, name, ml=0, mt=0, mr=0, mb=0):
    margin_line = f"margin = Rect2({ml}, {mt}, {mr}, {mb})\n" if any((ml,mt,mr,mb)) else ""
    content = (
        f'[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{deterministic_uid(name)}"]\n'
        f'[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]\n'
        f'[resource]\natlas = ExtResource("1")\nregion = Rect2({x}, {y}, {w}, {h})\n{margin_line}'
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

def pack_all(data):
    total_area = sum((d[1].width + PADDING) * (d[1].height + PADDING) for d in data)
    est_side   = max(next_power_of_two(int(math.sqrt(total_area) * 1.2)), 64)
    order      = sorted(range(len(data)), key=lambda i: -data[i][1].height)
    for _ in range(4):
        packer = ShelfPacker(est_side)
        placements = [None] * len(data)
        for i in order: placements[i] = packer.pack(data[i][1].width, data[i][1].height)
        _, ch = packer.canvas_size()
        if ch <= est_side * 2: break
        est_side <<= 1
    return packer, placements

cache = json.load(open(CACHE_FILE)) if os.path.exists(CACHE_FILE) else {}

# Auto-discover characters from powers/ subfolders
char_ids = set()
for sub in INPUT_SUBDIRS:
    d = os.path.join(SCRIPT_DIR, sub)
    if not os.path.exists(d): continue
    for entry in os.listdir(d):
        if os.path.isdir(os.path.join(d, entry)):
            char_ids.add(entry.lower())

for char_id in sorted(char_ids):
    # Special case: downfall powers go into DownfallLib
    if char_id == "downfall":
        char_proj = "Downfall"
    else:
        char_proj = next((e for e in os.listdir(PARENT)
                          if e.lower() == char_id
                          and os.path.isdir(os.path.join(PARENT, e))
                          and not e.endswith("Code")), None)
    if not char_proj:
        print(f"No project folder for {char_id}, skipping"); continue

    out_atlases     = os.path.join(PARENT, char_proj, char_proj, "images", "atlases")
    out_powers      = os.path.join(PARENT, char_proj, char_proj, "images", "powers")
    out_tres        = os.path.join(out_atlases, "power_atlas.sprites")
    out_tres_sprite = os.path.join(out_atlases, "power_sprite_atlas.sprites")
    res_base        = f"res://{char_proj}/images/atlases"
    atlas_res_path  = f"{res_base}/power_atlas.png"
    sprite_res_path = f"{res_base}/power_sprite_atlas.png"

    os.makedirs(out_atlases, exist_ok=True)
    os.makedirs(out_powers, exist_ok=True)
    os.makedirs(out_tres, exist_ok=True)
    os.makedirs(out_tres_sprite, exist_ok=True)

    print(f"\n=== {char_id} -> {char_proj} ===")

    seen    = set()
    entries = []

    for sub in INPUT_SUBDIRS:
        d = os.path.join(SCRIPT_DIR, sub, char_id)
        if not os.path.exists(d): continue
        for file in sorted(os.listdir(d)):
            if not file.lower().endswith(".png") or file in seen: continue
            seen.add(file)
            path = os.path.join(d, file)
            stem = os.path.splitext(file)[0]
            is_sprite_only = False
            img    = Image.open(path).convert("RGBA")
            big    = img.resize((BIG_SIZE,    BIG_SIZE),    Image.LANCZOS) if not is_sprite_only else None
            small  = img.resize((ATLAS_SIZE,  ATLAS_SIZE),  Image.LANCZOS) if not is_sprite_only else None
            sprite = img.resize((SPRITE_SIZE, SPRITE_SIZE), Image.LANCZOS)
            entries.append((stem, big, small, sprite, is_sprite_only))

    if not entries:
        print(f"  no images found"); continue

    non_sprite = [(s, big, sm, sp) for s, big, sm, sp, iso in entries if not iso]

    atlas_data  = [(s, *trim_alpha(sm)) for s, _, sm, _ in non_sprite]
    sprite_data = [(s, *trim_alpha(sp)) for s, _, _, sp, _ in entries]

    atlas_packer,  atlas_pl  = pack_all(atlas_data)
    sprite_packer, sprite_pl = pack_all(sprite_data)

    aw = next_power_of_two(atlas_packer.canvas_size()[0])
    ah = atlas_packer.canvas_size()[1]
    sw = next_power_of_two(sprite_packer.canvas_size()[0])
    sh = sprite_packer.canvas_size()[1]

    atlas        = Image.new("RGBA", (aw, ah), (0,0,0,0))
    sprite_atlas = Image.new("RGBA", (sw, sh), (0,0,0,0))

    for i, (stem, trimmed, ml, mt, mr, mb) in enumerate(atlas_data):
        ax, ay = atlas_pl[i]
        atlas.paste(trimmed, (ax, ay))
        tw, th = trimmed.size
        tres_name = f"{stem}{CUSTOM_SUFFIX}"
        write_tres(os.path.join(out_tres, f"{tres_name}.tres"),
                   atlas_res_path, ax, ay, tw, th, f"{char_id}_{stem}_atlas", ml, mt, mr, mb)
        big_path = os.path.join(out_powers, f"{tres_name}.png")
        if save_image_if_changed(non_sprite[i][1], big_path):
            print(f"  updated big: {tres_name}")

    for i, (stem, trimmed, ml, mt, mr, mb) in enumerate(sprite_data):
        sx, sy = sprite_pl[i]
        sprite_atlas.paste(trimmed, (sx, sy))
        tw, th = trimmed.size
        tres_name = f"{stem}{CUSTOM_SUFFIX}"
        write_tres(os.path.join(out_tres_sprite, f"{tres_name}.tres"),
                   sprite_res_path, sx, sy, tw, th, f"{char_id}_{stem}_sprite", ml, mt, mr, mb)

    save_image_if_changed(atlas,        os.path.join(out_atlases, "power_atlas.png"))
    save_image_if_changed(sprite_atlas, os.path.join(out_atlases, "power_sprite_atlas.png"))
    print(f"  {len(atlas_data)} powers packed")

with open(CACHE_FILE, "w") as f: json.dump(cache, f, indent=2)
print("\nDone!")