from PIL import Image
import numpy as np
from scipy.ndimage import gaussian_filter
import os
import math
import random
import string

def process_image(path, crop_box=(56, 56, 200, 200), radius=10, sigma=0.5):
    img = Image.open(path).convert("RGBA")
    img_cropped = img.crop(crop_box)
    img_upscaled = img_cropped.resize((256, 256), Image.NEAREST)

    alpha = np.array(img_upscaled.split()[3])

    size = radius * 2 + 1
    y, x = np.ogrid[-radius:radius+1, -radius:radius+1]
    kernel = (x*x + y*y) <= radius*radius

    padded = np.pad(alpha, radius)
    dilated = np.zeros_like(alpha)
    for dy in range(size):
        for dx in range(size):
            if kernel[dy, dx]:
                dilated = np.maximum(dilated, padded[dy:dy+alpha.shape[0], dx:dx+alpha.shape[1]])

    outline_alpha = gaussian_filter(dilated.astype(np.float32), sigma=sigma)
    outline_alpha = np.clip(outline_alpha, 0, 255).astype(np.uint8)

    outline = np.zeros((alpha.shape[0], alpha.shape[1], 4), dtype=np.uint8)
    outline[..., 0] = 255
    outline[..., 1] = 255
    outline[..., 2] = 255
    outline[..., 3] = outline_alpha
    image_outline = Image.fromarray(outline, "RGBA")

    black_outline = np.zeros((alpha.shape[0], alpha.shape[1], 4), dtype=np.uint8)
    black_outline[..., 3] = (outline_alpha * 0.5).astype(np.uint8)
    black_result = Image.fromarray(black_outline, "RGBA")

    big = Image.alpha_composite(black_result, img_upscaled)
    outline_downscaled = image_outline.resize((93, 93), Image.NEAREST)
    image_downscaled = img_cropped.resize((93, 93), Image.NEAREST)

    return big, outline_downscaled, image_downscaled


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


input_dir = "old"
os.makedirs("big", exist_ok=True)
os.makedirs("atlas", exist_ok=True)
os.makedirs("tres", exist_ok=True)

PADDING = 4
IMG_SIZE = 93
ATLAS_RES_PATH = "res://Watcher/images/relics/atlas/watcher_atlas.png"
OUTLINE_ATLAS_RES_PATH = "res://Watcher/images/relics/atlas/watcher_outline_atlas.png"

# --- collect all images first ---
entries = []

for file in os.listdir(input_dir):
    if file.lower().endswith(".png"):
        in_path = os.path.join(input_dir, file)
        stem = os.path.splitext(file)[0]

        big, outline_downscaled, image_downscaled = process_image(in_path)
        entries.append((stem, big, outline_downscaled, image_downscaled))
        print("processed:", file, "→", stem)

# --- build atlases ---
n = len(entries)
cols = math.ceil(math.sqrt(n))
rows = math.ceil(n / cols)

atlas_w = PADDING + cols * (IMG_SIZE + PADDING)
atlas_h = PADDING + rows * (IMG_SIZE + PADDING)

atlas = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))
outline_atlas = Image.new("RGBA", (atlas_w, atlas_h), (0, 0, 0, 0))

PADDING = 0        # no padding in atlas
INSET = 4          # tres region inset
REGION_SIZE = 85   # tres region size (93 - 4*2 = 85)

for i, (stem, big, outline_downscaled, image_downscaled) in enumerate(entries):
    col = i % cols
    row = i // cols
    x = col * IMG_SIZE
    y = row * IMG_SIZE

    atlas.paste(image_downscaled, (x, y))
    outline_atlas.paste(outline_downscaled, (x, y))

    big.save(os.path.join("big", f"{stem}.png"))

    write_tres(os.path.join("tres", f"{stem}.tres"), ATLAS_RES_PATH, x + INSET, y + INSET, REGION_SIZE)
    write_tres(os.path.join("tres", f"{stem}_outline.tres"), OUTLINE_ATLAS_RES_PATH, x + INSET, y + INSET, REGION_SIZE)

atlas.save(os.path.join("atlas", "watcher_atlas.png"))
outline_atlas.save(os.path.join("atlas", "watcher_outline_atlas.png"))

print(f"\nAtlases saved: {atlas_w}x{atlas_h}px, {n} images ({cols}x{rows} grid)")
print("Done!")