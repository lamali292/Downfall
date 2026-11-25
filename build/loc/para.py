#!/usr/bin/env python3
"""Local Paratranz uploader: eng source and existing translations, all languages in parallel.

  python build/loc/para.py source                 # eng files -> every language project
  python build/loc/para.py translations           # local <lang>/ files -> their project
  python build/loc/para.py all                    # source first, then translations
  python build/loc/para.py all --lang deu,fra     # only some languages
  python build/loc/para.py all --mod Champ,Snecko # only some mod folders
  python build/loc/para.py all --dry-run          # list what would be uploaded
  python build/loc/para.py download [--mod X]     # what download.yml does: Paratranz -> local files
  python build/loc/para.py prune [--yes]          # delete Paratranz files that no longer exist locally
  python build/loc/para.py restore <rev> [--mod X] [--lang xx]
                                                  # bring localization back from a git rev, then upload it

Same targets as .github/scripts/para_upload*.py (and the same paratranz-branch.json scoping),
but runs from your machine: no GitHub dispatch, one pass for source + translations, and
requests are parallel across languages. Run from the repo root.
API key: environment variable PARATRANZ_API_KEY.

The repo is the source of truth: translations are uploaded with force, so local files overwrite
whatever Paratranz has (use --no-force to keep existing Paratranz translations). The nightly download.yml
still runs: it only fills translations for files/keys that exist locally, so Paratranz edits by
translators flow in, and anything you deleted locally stays deleted once pruned.

Delete / restore: delete localization locally, `para.py all` (updates source) and `para.py prune`
(removes remote files whose local eng file is gone; asks unless --yes; only mods with a local
eng folder, or those named with --mod). Deleting a whole mod folder
needs `prune --mod X` (the nightly download already ignores mods without a local eng dir). To get it back, `restore <rev>`
checks the localization folders out of <rev> (a commit from before the delete) for every language and
uploads source + translations, force-overwriting. Paratranz drops translations of removed strings, so
git history is the backup.

Order matters: a translation file can only be uploaded once its source file exists on the
project, so `all` uploads source first. Re-adding a language later = `all --lang xx`.
"""
import argparse
import hashlib
import json
import os
import sys
import threading
import time
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path

import requests

ROOT = Path(__file__).resolve().parents[2]
os.chdir(ROOT)  # the shared scripts use repo-root-relative paths
sys.path.insert(0, str(ROOT / ".github" / "scripts"))
import para_branch  # noqa: E402
import para_http  # noqa: E402

API = "https://paratranz.cn/api"
EXCLUDED_MODS = {"gremlins"}
CONFIG = json.loads((ROOT / ".github/configs/paratranz.json").read_text(encoding="utf-8-sig"))["projects"]


def api_key():
    key = os.environ.get("PARATRANZ_API_KEY")
    if not key:
        sys.exit("PARATRANZ_API_KEY not set (set it as an environment variable)")
    return key


def request(method, path, **kwargs):
    # Start-rate limit, concurrency cap, retry and shared 429 cooldown live in para_http.
    return para_http.client.request(method, f"{API}{path}", headers={"Authorization": KEY}, **kwargs)


def parallel(fn, items):
    """Run fn over items concurrently (results in order). Real concurrency is limited by para_http."""
    with ThreadPoolExecutor(max_workers=16) as pool:
        return list(pool.map(fn, items))


def mods(only):
    wanted = {m.lower() for m in only} if only else None
    return sorted(
        d for d in para_branch.MOD_ROOT.iterdir()
        if d.is_dir() and d.name.lower() not in EXCLUDED_MODS and para_branch.is_wanted(d.name)
        and (wanted is None or d.name.lower() in wanted))


def remote_listing(pid):
    r = request("GET", f"/projects/{pid}/files")
    r.raise_for_status()
    return r.json()


def remote_files(pid):
    return {f["name"]: f["id"] for f in remote_listing(pid)}


def unchanged(path, remote_hash):
    """Paratranz stores the MD5 of the uploaded file. Compare with the local bytes, raw or with
    line endings/BOM normalised (it hashed some files in normalised form)."""
    b = path.read_bytes()
    nobom = b.removeprefix(b"\xef\xbb\xbf")
    return remote_hash in {hashlib.md5(x).hexdigest()
                           for x in (b, nobom, b.replace(b"\r\n", b"\n"), nobom.replace(b"\r\n", b"\n"))}


def upload_source(lang, pid, mod_dirs, dry):
    listing = remote_listing(pid)
    existing = {f["name"]: f["id"] for f in listing}
    hashes = {f["name"]: f.get("hash") for f in listing}
    n = {"created": 0, "updated": 0, "unchanged": 0, "failed": 0}

    def one(f_mod):
        mod, f = f_mod
        name = f"{para_branch.remote_mod(mod.name)}/localization/{lang}/{f.name}"
        if not ALL_FILES and name in hashes and unchanged(f, hashes[name]):
            return "unchanged"
        if dry:
            print(f"[{lang}] source  {name}")
            return None
        with open(f, "rb") as fh:
            if name in existing:
                r = request("POST", f"/projects/{pid}/files/{existing[name]}", files={"file": (f.name, fh)})
                key = "updated"
            else:
                folder = name.rsplit("/", 1)[0] + "/"
                r = request("POST", f"/projects/{pid}/files", files={"file": (f.name, fh)}, data={"path": folder})
                key = "created"
        if r.ok:
            print(f"[{lang}] {key:<8} {name}", flush=True)
            return key
        print(f"[{lang}] FAILED {name}: {r.status_code} {r.text[:200]}", flush=True)
        return "failed"

    jobs = [(mod, f) for mod in mod_dirs for f in sorted((mod / "localization" / "eng").glob("*.json"))]
    print(f"[{lang}] checking {len(jobs)} source files against Paratranz hashes ...", flush=True)
    for key in parallel(one, jobs):
        if key:
            n[key] += 1
    return (f"[{lang}] source: {n['created']} created, {n['updated']} updated, "
            f"{n['unchanged']} unchanged (skipped), {n['failed']} failed")


def upload_translations(lang, pid, mod_dirs, dry):
    existing = {} if dry else remote_files(pid)
    n = {"uploaded": 0, "skipped": 0, "failed": 0}

    def one(f_mod):
        mod, f = f_mod
        name = f"{para_branch.remote_mod(mod.name)}/localization/{lang}/{f.name}"
        if dry:
            print(f"[{lang}] transl. {name}")
            return None
        if name not in existing:
            print(f"[{lang}] SKIP (no source on Paratranz yet): {name}")
            return "skipped"
        with open(f, "rb") as fh:
            r = request("POST", f"/projects/{pid}/files/{existing[name]}/translation",
                        files={"file": (f.name, fh)}, data={"force": "true"} if FORCE else None)
        if r.ok:
            print(f"[{lang}] uploaded {name}", flush=True)
            return "uploaded"
        print(f"[{lang}] FAILED {name}: {r.status_code} {r.text[:200]}", flush=True)
        return "failed"

    jobs = [(mod, f) for mod in mod_dirs for f in sorted((mod / "localization" / lang).glob("*.json"))]
    print(f"[{lang}] uploading {len(jobs)} translation files ...", flush=True)
    for key in parallel(one, jobs):
        if key:
            n[key] += 1
    return f"[{lang}] translations: {n['uploaded']} uploaded, {n['skipped']} skipped, {n['failed']} failed"


def prune(langs, mod_dirs, only, dry, yes):
    """Delete remote files under the selected mods whose local file no longer exists."""
    # A file is kept while its eng source exists locally (translation files are legitimately
    # missing locally for untranslated files, so they must not count as deleted).
    local = set()
    for mod in mod_dirs:
        for f in (mod / "localization" / "eng").glob("*.json"):
            local.add((para_branch.remote_mod(mod.name), f.name))
    # Default scope: mods that still have a local eng folder (file-level deletions). --mod X also
    # prunes X after its whole folder was deleted. Mods of other branches (Collector, SlimeBossBeta,
    # Gremlins) are never touched unless named explicitly.
    scope = {para_branch.remote_mod(m) for m in only} if only else {para_branch.remote_mod(m.name) for m in mod_dirs}
    doomed = []
    for lang in langs:
        pid = int(CONFIG[lang])
        for name, fid in remote_files(pid).items():
            top = name.split("/", 1)[0]
            parts = name.split("/")
            if "TM" in name or len(parts) != 4 or parts[1] != "localization" or (top, parts[3]) in local:
                continue
            if top not in scope:
                continue
            doomed.append((lang, pid, fid, name))
    for lang, _, _, name in doomed:
        print(f"[{lang}] would delete {name}")
    if dry or not doomed:
        print(f"{len(doomed)} remote file(s) to prune")
        return
    if not yes and input(f"Delete {len(doomed)} remote file(s)? [y/N] ").lower() != "y":
        return
    def delete(d):
        lang, pid, fid, name = d
        r = request("DELETE", f"/projects/{pid}/files/{fid}")
        print(f"[{lang}] {'deleted' if r.ok else 'FAILED ' + str(r.status_code)} {name}")

    parallel(delete, doomed)


def restore(rev, langs, only_mods):
    import subprocess
    mod_root = para_branch.MOD_ROOT
    names = only_mods or sorted(
        {p.split("/")[1] for p in subprocess.check_output(
            ["git", "ls-tree", "-r", "--name-only", rev, "--", str(mod_root)], text=True).splitlines()
         if "/localization/eng/" in p})
    for m in names:
        for lang in ["eng", *langs]:
            path = f"{mod_root.as_posix()}/{m}/localization/{lang}"
            if subprocess.run(["git", "cat-file", "-e", f"{rev}:{path}"], capture_output=True).returncode == 0:
                subprocess.check_call(["git", "checkout", rev, "--", path])
                print(f"restored {path}")


def run_phase(fn, langs, mod_dirs, dry, jobs):
    # Parallel across languages (separate projects); sequential inside one project.
    with ThreadPoolExecutor(max_workers=jobs) as pool:
        futures = [pool.submit(fn, lang, int(CONFIG[lang]), mod_dirs, dry) for lang in langs]
        for fut in futures:
            print(fut.result())


def main():
    global KEY, FORCE, ALL_FILES
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("command", choices=["source", "translations", "all", "prune", "restore", "download"])
    ap.add_argument("rev", nargs="?", help="git rev to restore from (restore only)")
    ap.add_argument("--yes", action="store_true", help="prune without asking")
    ap.add_argument("--lang", help="comma-separated language codes (default: all in paratranz.json)")
    ap.add_argument("--mod", help="comma-separated mod folders (default: all)")
    ap.add_argument("--jobs", type=int, default=4, help="languages processed in parallel")
    ap.add_argument("--workers", type=int, default=3, help="max concurrent Paratranz requests")
    ap.add_argument("--interval", type=float, default=0.25,
                    help="min seconds between request starts, across all threads (raise it if you still get 429s)")
    ap.add_argument("--all-files", action="store_true",
                    help="source: re-upload every eng file, even if its hash already matches Paratranz")
    ap.add_argument("--no-force",action="store_true",
                    help="keep translations that already exist on Paratranz (default: local files overwrite them)")
    ap.add_argument("--dry-run", action="store_true")
    a = ap.parse_args()

    para_http.client.configure(workers=a.workers, interval=a.interval)
    FORCE = not a.no_force
    ALL_FILES = a.all_files
    langs = a.lang.split(",") if a.lang else list(CONFIG)
    bad = [l for l in langs if l not in CONFIG]
    if bad:
        sys.exit(f"unknown language(s): {', '.join(bad)}")
    only = a.mod.split(",") if a.mod else None
    if a.command == "restore":
        if not a.rev:
            sys.exit("restore needs a git rev, e.g. `restore HEAD~3`")
        restore(a.rev, langs, only)
    mod_dirs = mods(only) if a.command != "download" else []
    if not mod_dirs and a.command not in ("prune", "download"):
        sys.exit("no mod folders with localization/eng/ matched")
    if a.command == "download":
        # Run the exact script download.yml runs, just locally (writes localization files; review with git diff).
        KEY = api_key()
        os.environ["PARATRANZ_API_KEY"] = KEY
        if only:
            para_branch._only_env = {m.lower() for m in only}
        import para_download
        para_http.client.configure(workers=a.workers, interval=a.interval)
        para_download.main(langs=set(langs) if a.lang else None, workers=a.workers)
        return
    KEY = "" if a.dry_run and not os.environ.get("PARATRANZ_API_KEY") else api_key()

    if a.command == "prune":
        prune(langs, mod_dirs, only, a.dry_run, a.yes)
        return
    if a.command in ("source", "all", "restore"):
        run_phase(upload_source, langs, mod_dirs, a.dry_run, a.jobs)
    if a.command in ("translations", "all", "restore"):
        run_phase(upload_translations, langs, mod_dirs, a.dry_run, a.jobs)


if __name__ == "__main__":
    main()
