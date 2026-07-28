import os
import re
import stat
import sys

# Project root, resolved relative to this script (build/), not the cwd.
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
TARGET_DIRECTORY = os.path.dirname(SCRIPT_DIR)

# realpath resolves BOTH symlinks and Windows junctions, so containment against
# this is the check that actually holds. normcase because Windows paths are
# case-insensitive and realpath doesn't normalise casing.
REAL_ROOT = os.path.normcase(os.path.realpath(TARGET_DIRECTORY))

# [ \t]* not \s* — \s eats the newline and glues the next line on when uid= ends a line.
UID_PATTERN = re.compile(r'\buid="uid://[^"]*"[ \t]*')

DRY_RUN = "--apply" not in sys.argv


def is_inside_project(path):
    """True only if path resolves to somewhere physically under the project root."""
    real = os.path.normcase(os.path.realpath(path))
    return real == REAL_ROOT or real.startswith(REAL_ROOT + os.sep)


def is_reparse_point(path):
    """Catches symlinks AND junctions on Windows; os.path.islink misses junctions."""
    try:
        st = os.lstat(path)
    except OSError:
        return True  # unreadable: treat as unsafe
    return bool(getattr(st, "st_file_attributes", 0) & stat.FILE_ATTRIBUTE_REPARSE_POINT)


def nuke_uids_in_file(file_path):
    # newline='' on both ends, or Python rewrites Godot's LF files as CRLF
    # and every touched file shows up as a 100%-changed diff.
    with open(file_path, "r", encoding="utf-8", newline="") as f:
        content = f.read()

    if 'uid="uid://' not in content:
        return False

    if DRY_RUN:
        print(f"WOULD CLEAN: {file_path}")
        return True

    with open(file_path, "w", encoding="utf-8", newline="") as f:
        f.write(UID_PATTERN.sub("", content))
    print(f"CLEANED: {file_path}")
    return True


def main():
    print(f"Project root: {REAL_ROOT}")
    print("DRY RUN - pass --apply to actually write.\n" if DRY_RUN else "APPLYING CHANGES\n")

    target_extensions = (".tscn", ".tres")
    seen = changed = skipped = 0

    for root, dirs, files in os.walk(TARGET_DIRECTORY):
        if ".godot" in root.split(os.sep):
            dirs[:] = []
            continue

        # Prune anything that leaves the project, whatever the link mechanism.
        kept = []
        for d in dirs:
            full = os.path.join(root, d)
            if is_reparse_point(full) or not is_inside_project(full):
                print(f"SKIP (leaves project): {full} -> {os.path.realpath(full)}")
                skipped += 1
            else:
                kept.append(d)
        dirs[:] = kept

        for name in files:
            if not name.endswith(target_extensions):
                continue
            path = os.path.join(root, name)

            # Belt and braces: a file can be individually linked out too.
            if is_reparse_point(path) or not is_inside_project(path):
                print(f"SKIP (leaves project): {path}")
                skipped += 1
                continue

            seen += 1
            try:
                if nuke_uids_in_file(path):
                    changed += 1
            except Exception as e:
                print(f"ERROR {path}: {e}")

    verb = "would change" if DRY_RUN else "changed"
    print(f"\nScanned {seen} local files, {verb} {changed}, skipped {skipped} external paths.")


if __name__ == "__main__":
    main()