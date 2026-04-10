import os
import re
import shutil
from collections import defaultdict
import msvcrt

card_base = r"C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall\Code\Cards"
img_base = r"C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall\Downfall\images\card_portraits"
power_base = r"C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall\Code\Powers"
power_img_base = r"C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall\image_gen\powers"
missing_card_png = r"C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall\image_gen\missing.png"
missing_power_png = r"C:\Users\lamal\Desktop\Programmieren\sts2mods\Downfall\image_gen\missing_power.png"

def to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

def normalize(name):
    return to_snake(name).replace("_", "")

# --- Cards ---
cards = defaultdict(dict)
images = defaultdict(set)

for root, dirs, files in os.walk(card_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    folder = normalize(os.path.relpath(root, card_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".cs"):
            snake = to_snake(os.path.splitext(file)[0])
            cards[folder][to_snake(snake)] = snake

for root, dirs, files in os.walk(img_base):
    folder = normalize(os.path.relpath(root, img_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".png"):
            images[folder].add(to_snake(os.path.splitext(file)[0]))

print("=== CARDS ===")
for folder in sorted(set(list(cards.keys()) + list(images.keys()))):
    missing_art = set(cards.get(folder, {}).keys()) - images.get(folder, set())
    missing_card = images.get(folder, set()) - set(cards.get(folder, {}).keys())
    if missing_art or missing_card:
        print(f"{folder}:")
        if missing_art:
            print(f"  no image: {sorted(missing_art)}")
            for norm_name in missing_art:
                snake_name = cards[folder][norm_name]
                dest_folder = os.path.join(img_base, folder)
                os.makedirs(dest_folder, exist_ok=True)
                dest = os.path.join(dest_folder, f"{snake_name}.png")
                shutil.copy(missing_card_png, dest)
                print(f"    -> copied to {dest}")
 

# --- Powers ---
powers = defaultdict(dict)
power_images = defaultdict(set)

for root, dirs, files in os.walk(power_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    folder = normalize(os.path.relpath(root, power_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".cs"):
            snake = to_snake(os.path.splitext(file)[0])
            snake = re.sub(r'_power$', '', snake)
            powers[folder][normalize(snake)] = snake

for root, dirs, files in os.walk(power_img_base):
    folder = normalize(os.path.relpath(root, power_img_base).split(os.sep)[0])
    for file in files:
        if file.endswith(".png"):
            power_images[folder].add(normalize(os.path.splitext(file)[0]))

print("\n=== POWERS ===")
for folder in sorted(set(list(powers.keys()) + list(power_images.keys()))):
    missing_art = set(powers.get(folder, {}).keys()) - power_images.get(folder, set())
    missing_card = power_images.get(folder, set()) - set(powers.get(folder, {}).keys())
    if missing_art or missing_card:
        print(f"{folder}:")
        if missing_art:
            print(f"  no image: {sorted(missing_art)}")
            for norm_name in missing_art:
                snake_name = powers[folder][norm_name]
                dest_folder = os.path.join(power_img_base, folder)
                os.makedirs(dest_folder, exist_ok=True)
                dest = os.path.join(dest_folder, f"{snake_name}.png")
                shutil.copy(missing_power_png, dest)
                print(f"    -> copied to {dest}")
                
               