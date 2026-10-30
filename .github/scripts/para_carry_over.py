"""One-off: seed this branch's translations from the live (old) ones, keeping only strings whose
English source is unchanged.

The Slimeboss branch reworks SlimeBoss's strings. Its SlimeBossBeta folder on Paratranz starts
with nothing translated; running this first lets us upload the translations that are still
valid (same English text as the live SlimeBoss) and leave the changed strings untranslated.

    python .github/scripts/para_carry_over.py --mod SlimeBoss --old-ref develop
"""
import argparse
import json
import subprocess
from collections import OrderedDict
from pathlib import Path


def git_show(ref, path):
    out = subprocess.run(["git", "show", f"{ref}:{path}"], capture_output=True, check=True).stdout
    return json.loads(out.decode("utf-8-sig"), object_pairs_hook=OrderedDict)


def git_ls(ref, path):
    out = subprocess.run(["git", "ls-tree", "-r", "--name-only", ref, path], capture_output=True, text=True, check=True).stdout
    return [line for line in out.splitlines() if line.endswith(".json")]


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--mod", required=True)
    ap.add_argument("--old-ref", required=True, help="git ref holding the live translations")
    args = ap.parse_args()

    loc = Path(args.mod) / "localization"
    totals = {"kept": 0, "changed": 0, "gone": 0}
    for old_path in git_ls(args.old_ref, f"{args.mod}/localization"):
        parts = Path(old_path).parts
        lang, filename = parts[2], parts[3]
        if lang == "eng":
            continue
        new_eng_path = loc / "eng" / filename
        if not new_eng_path.is_file():
            continue
        old_eng = git_show(args.old_ref, f"{args.mod}/localization/eng/{filename}")
        old_tr = git_show(args.old_ref, old_path)
        new_eng = json.loads(new_eng_path.read_text(encoding="utf-8-sig"), object_pairs_hook=OrderedDict)

        kept = OrderedDict()
        for key, text in new_eng.items():
            if key not in old_tr:
                continue
            if old_eng.get(key) == text:
                kept[key] = old_tr[key]
            else:
                totals["changed"] += 1
        totals["gone"] += sum(1 for k in old_tr if k not in new_eng)
        totals["kept"] += len(kept)

        out_path = loc / lang / filename
        if kept:
            out_path.parent.mkdir(parents=True, exist_ok=True)
            out_path.write_text(json.dumps(kept, ensure_ascii=False, indent=4), encoding="utf-8")
        elif out_path.exists():
            out_path.unlink()

    print(f"kept {totals['kept']} translations, {totals['changed']} dropped (English changed), "
          f"{totals['gone']} dropped (string removed)")


if __name__ == "__main__":
    main()
