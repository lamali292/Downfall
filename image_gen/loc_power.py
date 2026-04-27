import os, json, re

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PARENT     = os.path.join(SCRIPT_DIR, "..")

SOURCE_JSON = os.path.join(PARENT, "Downfall", "localization", "eng", "powers.json")

def to_snake_upper(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).upper()

with open(SOURCE_JSON, encoding="utf-8") as f:
    source = json.load(f)

source_map = {}
for key, value in source.items():
    if "-" not in key or "." not in key: continue
    _, rest            = key.split("-", 1)
    power_name, suffix = rest.rsplit(".", 1)
    if power_name not in source_map:
        source_map[power_name] = {}
    source_map[power_name][suffix] = value

all_found = set()

for proj_entry in sorted(os.listdir(PARENT)):
    proj_path = os.path.join(PARENT, proj_entry)
    if not os.path.isdir(proj_path): continue

    code_dir = None
    char_id  = None
    for sub in os.listdir(proj_path):
        if sub.endswith("Code") and os.path.isdir(os.path.join(proj_path, sub)):
            code_dir = os.path.join(proj_path, sub)
            char_id  = sub[:-4].upper()
            break
    if not code_dir: continue

    powers_dir = os.path.join(code_dir, "Powers")
    if not os.path.exists(powers_dir): continue

    print(f"\n=== {char_id} ===")

    power_names = []
    for root, dirs, files in os.walk(powers_dir):
        dirs[:] = [d for d in dirs if d != "Abstract"]
        for file in files:
            if file.endswith(".cs"):
                name = os.path.splitext(file)[0]
                power_names.append(to_snake_upper(name))
                all_found.add(to_snake_upper(name))

    out = {}
    missing = []
    for power_name in sorted(power_names):
        if power_name not in source_map:
            missing.append(power_name)
            continue
        for suffix, value in source_map[power_name].items():
            out[f"{char_id}-{power_name}.{suffix}"] = value

    if missing:
        print(f"  Not in source powers.json: {missing}")
    if not out:
        print(f"  No entries found, skipping"); continue

    out_dir = os.path.join(PARENT, proj_entry, proj_entry, "localization", "eng")
    os.makedirs(out_dir, exist_ok=True)
    out_path = os.path.join(out_dir, "powers.json")
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(out, f, indent=2, ensure_ascii=False)
    print(f"  Wrote {len(out)} entries -> {out_path}")

# Unmatched
unmatched = {}
for power_name, suffixes in source_map.items():
    if power_name not in all_found:
        for suffix, value in suffixes.items():
            unmatched[f"DOWNFALL-{power_name}.{suffix}"] = value

if unmatched:
    out_path = os.path.join(SCRIPT_DIR, "powers.json")
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(unmatched, f, indent=2, ensure_ascii=False)
    print(f"\nUnmatched: {len(unmatched) // 2} powers written to {out_path}")

print("\nDone!")