import json
import os
import re
from collections import OrderedDict
from pathlib import Path

with open(".github/configs/paratranz.json", encoding="utf-8") as f:
    config = json.load(f)


import para_branch
import para_http


def get_api_key(lang_code):
    env_name = f"PARATRANZ_API_KEY"
    token = os.environ.get(env_name)
    if not token:
        raise EnvironmentError(f"Environment variable {env_name} is not set.")
    return token


PAGE_SIZE = 1000  # the API's maximum for /strings


def fetch_json(url, token, **params):
    # Rate limiting / retry / shared cooldown live in para_http (shared with build/loc/*).
    r = para_http.client.request("GET", url, headers={"Authorization": token, "accept": "*/*"}, params=params)
    r.raise_for_status()
    return r.json()


def fetch_strings_by_file(project_id, token):
    """All strings of a project grouped by Paratranz file path, in a few paged requests
    (a project has ~4-5k strings -> ~5 requests, instead of one /translation request per file)."""
    by_file, page = {}, 1
    while True:
        d = fetch_json(f"https://paratranz.cn/api/projects/{project_id}/strings", token,
                       pageSize=PAGE_SIZE, page=page)
        for row in d["results"]:
            by_file.setdefault(row["file"]["name"], []).append(row)
        if page >= d["pageCount"]:
            return by_file
        page += 1


def save_translation(mod_name, lang_code, filename, translated_dict):
    if not translated_dict:
        return

    local_dir = para_branch.MOD_ROOT / mod_name / "localization" / lang_code
    local_dir.mkdir(parents=True, exist_ok=True)
    output_path = local_dir / filename
    source_path = para_branch.MOD_ROOT / mod_name / "localization" / "eng" / filename

    try:
        with open(source_path, "r", encoding="utf-8-sig") as f:
            source_content = f.read()
        source_json = json.loads(source_content, object_pairs_hook=OrderedDict)

        output_json = OrderedDict()
        for key, original_value in source_json.items():
            if key in translated_dict:
                output_json[key] = translated_dict[key]

        with open(output_path, "w", encoding="utf-8") as f:
            json.dump(output_json, f, ensure_ascii=False, indent=4)

    except (IOError, FileNotFoundError):
        with open(output_path, "w", encoding="utf-8") as f:
            json.dump(translated_dict, f, ensure_ascii=False, indent=4, sort_keys=True)


def process_file(paratranz_path, rows):
    parts = Path(paratranz_path).parts
    if "TM" in paratranz_path or len(parts) < 4 or parts[1] != "localization":
        print(f"  Skip (unexpected path): {paratranz_path}")
        return 0

    mod_name = para_branch.local_mod(parts[0])
    lang_code = parts[2]
    filename = parts[3]

    if mod_name is None or not para_branch.is_wanted(mod_name):
        return 0

    # Only strings that are translated (stage >= 1), like the former per-file /translation download.
    translated_dict = {r["key"]: re.sub(r'\\"', '"', r.get("translation") or "")
                       for r in rows if r["stage"] >= 1}

    save_translation(mod_name, lang_code, filename, translated_dict)
    print(f"  {paratranz_path} ({len(translated_dict)}/{len(rows)} translated)")
    return len(translated_dict)


def main(langs=None, workers=3):
    para_http.client.configure(workers=workers)
    grand_total = 0
    for lang_code, project_id in config["projects"].items():
        if langs and lang_code not in langs:
            continue
        project_id = int(project_id)
        token = get_api_key(lang_code)
        print(f"\n--- Downloading from project {project_id} ({lang_code}) ---")

        by_file = fetch_strings_by_file(project_id, token)
        print(f"  {len(by_file)} files, {sum(map(len, by_file.values()))} strings")

        project_total = sum(process_file(name, rows) for name, rows in sorted(by_file.items()))
        print(f"  {lang_code}: {project_total} strings across {len(by_file)} files")
        grand_total += project_total

    print(f"\nDone. {grand_total} strings written in total.")


if __name__ == "__main__":
    main()
