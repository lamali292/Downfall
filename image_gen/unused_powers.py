import os, re

ROOT = os.path.dirname(os.path.abspath(__file__))

power_base      = os.path.join(ROOT, "..", "Code", "Powers")
power_img_base  = os.path.join(ROOT, "powers")
power_beta_base = os.path.join(ROOT, "powers_beta")

SKIP_FOLDERS = {"sts2"}

def to_snake(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).lower()

def normalize(name):
    return re.sub(r'_power$', '', to_snake(name)).replace("_", "")

cs_names = set()
for root, dirs, files in os.walk(power_base):
    dirs[:] = [d for d in dirs if d != "Abstract"]
    for file in files:
        if file.endswith(".cs"):
            cs_names.add(normalize(os.path.splitext(file)[0]))

for base in [power_img_base, power_beta_base]:
    if not os.path.exists(base):
        continue
    for root, _, files in os.walk(base):
        char = os.path.basename(root)
        if char in SKIP_FOLDERS:
            continue
        for file in sorted(files):
            if not file.lower().endswith(".png"):
                continue
            stem = os.path.splitext(file)[0]
            if normalize(stem) not in cs_names:
                print(f"[{char}] {stem}")