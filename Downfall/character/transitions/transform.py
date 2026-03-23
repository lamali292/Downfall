import shutil
from pathlib import Path

# --- Config ---
INPUT_DIR = Path(".")        # folder to search in
OUTPUT_DIR = Path(".")       # folder to write to (can be same)

INPUT_NAME = "ironclad"
OUTPUT_NAMES = ["automaton", "champ", "collector", "gremlins", "guardian", "hexaghost", "slime_boss", "snecko"]
# --------------

def copy_renamed_pngs(input_dir: Path, output_dir: Path, input_name: str, output_names: list[str]):
    matches = [f for f in input_dir.rglob("*.png") if input_name in f.stem]

    if not matches:
        print(f"No PNGs found containing '{input_name}'")
        return

    output_dir.mkdir(parents=True, exist_ok=True)

    for src in matches:
        for out_name in output_names:
            new_filename = src.name.replace(input_name, out_name)
            dest = output_dir / new_filename
            shutil.copy2(src, dest)
            print(f"  {src.name}  ->  {dest.name}")

    print(f"\nDone. {len(matches) * len(output_names)} files written to '{output_dir}'")


copy_renamed_pngs(INPUT_DIR, OUTPUT_DIR, INPUT_NAME, OUTPUT_NAMES)