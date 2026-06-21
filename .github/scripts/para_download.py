import json
import os
import re
from collections import OrderedDict
from pathlib import Path

import requests

with open(".github/configs/paratranz.json", encoding="utf-8") as f:
    config = json.load(f)


def get_api_key(lang_code):
    env_name = f"PARATRANZ_API_KEY_{lang_code.upper()}"
    token = os.environ.get(env_name)
    if not token:
        raise EnvironmentError(f"Environment variable {env_name} is not set.")
    return token


def fetch_json(url, token):
    headers = {"Authorization": token, "accept": "*/*"}
    r = requests.get(url, headers=headers)
    r.raise_for_status()
    return r.json()


def get_translations(project_id, file_id, token):
    url = f"https://paratranz.cn/api/projects/{project_id}/files/{file_id}/translation"
    translations = fetch_json(url, token)

    keys, values = [], []
    for item in translations:
        keys.append(item["key"])
        translation = item.get("translation", "")
        original = item.get("original", "")
        if item["stage"] in [0, -1]:
            values.append(original)
        else:
            values.append(translation)

    return keys, values


def save_translation(mod_name, lang_code, filename, translated_dict):
    local_dir = Path(mod_name) / "localization" / lang_code
    local_dir.mkdir(parents=True, exist_ok=True)
    output_path = local_dir / filename
    source_path = Path(mod_name) / "localization" / "eng" / filename

    try:
        with open(source_path, "r", encoding="utf-8") as f:
            source_content = f.read()
        source_json = json.loads(source_content, object_pairs_hook=OrderedDict)

        for key, original_value in source_json.items():
            if key in translated_dict:
                translated_value = translated_dict[key]

                original_str = json.dumps(original_value, ensure_ascii=False)
                translated_str = json.dumps(translated_value, ensure_ascii=False)

                key_pattern = re.escape(json.dumps(key, ensure_ascii=False))
                value_pattern = re.escape(original_str)

                pattern = re.compile(f"({key_pattern}\\s*:\\s*){value_pattern}")
                safe_replacement = translated_str.replace("\\", "\\\\")
                replacement = f"\\1{safe_replacement}"

                source_content, _ = pattern.subn(replacement, source_content, count=1)

        with open(output_path, "w", encoding="utf-8") as f:
            f.write(source_content)

    except (IOError, FileNotFoundError):
        with open(output_path, "w", encoding="utf-8") as f:
            json.dump(translated_dict, f, ensure_ascii=False, indent=4, sort_keys=True)


def process_file(project_id, file_id, paratranz_path, token):
    keys, values = get_translations(project_id, file_id, token)

    parts = Path(paratranz_path).parts
    if len(parts) < 4 or parts[1] != "localization":
        print(f"  Skip (unexpected path structure): {paratranz_path}")
        return

    mod_name = parts[0]
    lang_code = parts[2]
    filename = parts[3]

    translated_dict = {}
    for k, v in zip(keys, values):
        v = re.sub(r'\\"', '"', v)
        translated_dict[k] = v

    save_translation(mod_name, lang_code, filename, translated_dict)
    print(f"  {paratranz_path}")


def main():
    for lang_code, project_id in config["projects"].items():
        project_id = int(project_id)
        token = get_api_key(lang_code)
        print(f"\n--- Downloading from project {project_id} ({lang_code}) ---")

        files_url = f"https://paratranz.cn/api/projects/{project_id}/files/"
        files = fetch_json(files_url, token)

        for f in files:
            if "TM" in f["name"]:
                continue
            process_file(project_id, f["id"], f["name"], token)

    print("\nDone.")


if __name__ == "__main__":
    main()
