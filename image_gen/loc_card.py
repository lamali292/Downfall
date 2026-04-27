import os, json, re

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
PARENT     = os.path.join(SCRIPT_DIR, "..")

SOURCE_JSON = os.path.join(PARENT, "Downfall", "localization", "eng", "cards.json")

def to_snake_upper(name):
    return re.sub(r'(?<!^)(?=[A-Z])', '_', name).upper()

# Load source
with open(SOURCE_JSON, encoding="utf-8") as f:
    source = json.load(f)

# Build lookup: CARD_NAME -> {suffix: value}
# e.g. "ADRENAL_ARMOR" -> {"description": "...", "title": "..."}
source_map = {}
for key, value in source.items():
    # key format: DOWNFALL-CARD_NAME.suffix
    if "-" not in key or "." not in key: continue
    _, rest   = key.split("-", 1)
    card_name, suffix = rest.rsplit(".", 1)
    if card_name not in source_map:
        source_map[card_name] = {}
    source_map[card_name][suffix] = value

# Auto-discover characters
for proj_entry in sorted(os.listdir(PARENT)):
    proj_path = os.path.join(PARENT, proj_entry)
    if not os.path.isdir(proj_path): continue

    # Look for *Code subfolder
    code_dir = None
    char_id  = None
    for sub in os.listdir(proj_path):
        if sub.endswith("Code") and os.path.isdir(os.path.join(proj_path, sub)):
            code_dir = os.path.join(proj_path, sub)
            char_id  = sub[:-4].upper()  # e.g. "HEXAGHOST"
            break
    if not code_dir: continue

    cards_dir = os.path.join(code_dir, "Cards")
    if not os.path.exists(cards_dir): continue

    print(f"\n=== {char_id} ===")

    # Collect all card class names
    card_names = []
    for root, dirs, files in os.walk(cards_dir):
        dirs[:] = [d for d in dirs if d != "Abstract"]
        for file in files:
            if file.endswith(".cs"):
                name       = os.path.splitext(file)[0]
                snake_upper = to_snake_upper(name)
                card_names.append(snake_upper)

    # Build output json
    out = {}
    missing = []
    for card_name in sorted(card_names):
        if card_name not in source_map:
            missing.append(card_name)
            continue
        for suffix, value in source_map[card_name].items():
            out_key = f"{char_id}-{card_name}.{suffix}"
            out[out_key] = value

    if missing:
        print(f"  Not in source cards.json: {missing}")

    if not out:
        print(f"  No entries found, skipping")
        continue

    # Write output
    out_dir = os.path.join(PARENT, proj_entry, proj_entry, "localization", "eng")
    os.makedirs(out_dir, exist_ok=True)
    out_path = os.path.join(out_dir, "cards.json")
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(out, f, indent=2, ensure_ascii=False)
    print(f"  Wrote {len(out)} entries -> {out_path}")


# Collect all card names found across all characters
all_found = set()
for proj_entry in os.listdir(PARENT):
    proj_path = os.path.join(PARENT, proj_entry)
    if not os.path.isdir(proj_path): continue
    for sub in os.listdir(proj_path):
        if sub.endswith("Code") and os.path.isdir(os.path.join(proj_path, sub)):
            cards_dir = os.path.join(proj_path, sub, "Cards")
            if not os.path.exists(cards_dir): continue
            for root, dirs, files in os.walk(cards_dir):
                dirs[:] = [d for d in dirs if d != "Abstract"]
                for file in files:
                    if file.endswith(".cs"):
                        all_found.add(to_snake_upper(os.path.splitext(file)[0]))

# Write all source entries that were never matched to any character
unmatched = {}
for card_name, suffixes in source_map.items():
    if card_name not in all_found:
        for suffix, value in suffixes.items():
            unmatched[f"DOWNFALL-{card_name}.{suffix}"] = value

if unmatched:
    out_path = os.path.join(SCRIPT_DIR, "cards.json")
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(unmatched, f, indent=2, ensure_ascii=False)
    print(f"\nUnmatched: {len(unmatched) // 2} cards written to {out_path}")
    
    
print("\nDone!")