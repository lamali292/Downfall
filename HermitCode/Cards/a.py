import re
import shutil
from pathlib import Path

# Configure these
SOURCE_DIR = Path(".")
OUTPUT_DIR = Path(".")  # Set to a different path if you want a separate output folder

RARITY_PATTERN = re.compile(r'CardRarity\.(\w+)')

def find_rarity(file_path: Path) -> str | None:
    """Look for CardRarity.X only in the constructor (line containing ': base(')."""
    with open(file_path, encoding="utf-8") as f:
        for line in f:
            if ": base(" in line:
                match = RARITY_PATTERN.search(line)
                if match:
                    return match.group(1)
    return None

def main():
    cs_files = list(SOURCE_DIR.rglob("*.cs"))
    print(f"Found {len(cs_files)} .cs files")

    moved = 0
    skipped = 0

    for file in cs_files:
        rarity = find_rarity(file)
        if not rarity:
            print(f"  [skip] No rarity found: {file.name}")
            skipped += 1
            continue

        dest_dir = OUTPUT_DIR / rarity
        dest_dir.mkdir(parents=True, exist_ok=True)
        dest = dest_dir / file.name

        shutil.move(str(file), dest)
        print(f"  [move] {file.name} -> {rarity}/")
        moved += 1

    print(f"\nDone. Moved: {moved}, Skipped: {skipped}")

if __name__ == "__main__":
    main()