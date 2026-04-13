from PIL import Image
import os
import math
import random
import string
import shutil

def random_uid(length=7):
    return ''.join(random.choices(string.ascii_lowercase + string.digits, k=length))

def write_tres(path, atlas_res_path, x, y, size):
    content = f'''[gd_resource type="AtlasTexture" load_steps=2 format=3 uid="uid://{random_uid()}"]
[ext_resource type="Texture2D" path="{atlas_res_path}" id="1"]
[resource]
atlas = ExtResource("1")
region = Rect2({x}, {y}, {size}, {size})
'''
    with open(path, "w") as f:
        f.write(content)

input_dir       = "powers"
OUT_POWERS      = "../Downfall/images/powers"
OUT_ATLASES     = "../Downfall/images/atlases"
OUT_TRES        = os.path.join(OUT_ATLASES, "power_atlas.sprites")
OUT_TRES_SPRITE = os.path.join(OUT_ATLASES, "power_sprite_atlas.sprites")

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

BIG_SIZE    = 256
ATLAS_SIZE  = 64
SPRITE_SIZE = 24

ATLAS_RES_PATH        = "res://Downfall/images/atlases/power_atlas.png"
SPRITE_ATLAS_RES_PATH = "res://Downfall/images/atlases/power_sprite_atlas.png"

# --- collect all images recursively ---
entries = []
for root, dirs, files in os.walk(input_dir):
    is_sts2 = os.path.basename(root) == "sts2"
    for file in files:
        if file.lower().endswith(".png"):
            in_path = os.path.join(root, file)
            stem = os.path.splitext(file)[0]
            img = Image.open(in_path).convert("RGBA")
            big    = img.resize((BIG_SIZE,   BIG_SIZE),   Image.LANCZOS) if not is_sts2 else None
            small  = img.resize((ATLAS_SIZE, ATLAS_SIZE), Image.LANCZOS) if not is_sts2 else None
            sprite = img.resize((SPRITE_SIZE, SPRITE_SIZE), Image.LANCZOS)
            entries.append((stem, big, small, sprite, is_sts2))
            print("processed:", in_path, "→", stem, ("(sprite only)" if is_sts2 else ""))

# --- build atlases ---
non_sts2    = [(stem, big, small, sprite) for stem, big, small, sprite, is_sts2 in entries if not is_sts2]
all_entries = entries

n_atlas  = len(non_sts2)
n_sprite = len(all_entries)

cols_atlas  = math.ceil(math.sqrt(n_atlas))
rows_atlas  = math.ceil(n_atlas  / cols_atlas)
cols_sprite = math.ceil(math.sqrt(n_sprite))
rows_sprite = math.ceil(n_sprite / cols_sprite)

atlas        = Image.new("RGBA", (cols_atlas  * ATLAS_SIZE,  rows_atlas  * ATLAS_SIZE),  (0, 0, 0, 0))
sprite_atlas = Image.new("RGBA", (cols_sprite * SPRITE_SIZE, rows_sprite * SPRITE_SIZE), (0, 0, 0, 0))

for i, (stem, big, small, sprite) in enumerate(non_sts2):
    col = i % cols_atlas
    row = i // cols_atlas
    ax = col * ATLAS_SIZE
    ay = row * ATLAS_SIZE
    atlas.paste(small, (ax, ay))
    write_tres(os.path.join(OUT_TRES, f"{stem}_power.tres"), ATLAS_RES_PATH, ax, ay, ATLAS_SIZE)
    big.save(os.path.join(OUT_POWERS, f"{stem}_power.png"))

for i, (stem, big, small, sprite, is_sts2) in enumerate(all_entries):
    col = i % cols_sprite
    row = i // cols_sprite
    sx = col * SPRITE_SIZE
    sy = row * SPRITE_SIZE
    sprite_atlas.paste(sprite, (sx, sy))
    filename = f"{stem}.tres" if is_sts2 else f"{stem}_power.tres"
    write_tres(os.path.join(OUT_TRES_SPRITE, filename), SPRITE_ATLAS_RES_PATH, sx, sy, SPRITE_SIZE)

atlas.save(os.path.join(OUT_ATLASES, "power_atlas.png"))
sprite_atlas.save(os.path.join(OUT_ATLASES, "power_sprite_atlas.png"))

print(f"\npower_atlas: {cols_atlas}x{rows_atlas} grid ({n_atlas} images)")
print(f"power_sprite_atlas: {cols_sprite}x{rows_sprite} grid ({n_sprite} images)")
print("Done!")