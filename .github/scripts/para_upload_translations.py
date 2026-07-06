import asyncio
import json
import os
from pathlib import Path

import paratranz_client

CONFIG_PATH = Path(".github/configs/paratranz.json")
with open(CONFIG_PATH, encoding="utf-8") as f:
    config = json.load(f)

# Only these are handled — matches the localization sync script
CATEGORY_FILES = {"cards.json", "powers.json", "relics.json"}

# This script only ever uploads German translations.
LANG_CODE = "jpn"

# force=False: only fills in strings that haven't been manually edited by a
# human translator on Paratranz. Set to True to overwrite everything.
FORCE = False


def get_api_key(lang_code):
    env_name = f"PARATRANZ_API_KEY_{lang_code.upper()}"
    token = os.environ.get(env_name)
    if not token:
        raise EnvironmentError(f"Environment variable {env_name} is not set.")
    return token


async def upload_translation_for_file(api_instance, project_id, file_path, local_file, existing_files_dict, force):
    existing_file = existing_files_dict.get(file_path)

    if not existing_file:
        print(f"  SKIP (no matching source file on Paratranz yet): {file_path}")
        return

    max_retries = 3
    for attempt in range(max_retries):
        try:
            if force:
                # Only pass force when True — passing force=False triggers a
                # bool-serialization bug in this SDK's generated query params.
                await api_instance.update_file_translation(
                    project_id, file_id=existing_file.id, file=str(local_file), force=force
                )
            else:
                await api_instance.update_file_translation(
                    project_id, file_id=existing_file.id, file=str(local_file)
                )
            print(f"  Uploaded translation: {file_path}")
            break
        except Exception as e:
            if attempt < max_retries - 1:
                wait_time = 2 ** attempt
                print(f"  Retry {attempt + 1}/{max_retries} for {file_path}: {e}")
                await asyncio.sleep(wait_time)
            else:
                print(f"  Failed: {file_path} - {e}")


async def main():
    if LANG_CODE not in config["projects"]:
        raise RuntimeError(f"'{LANG_CODE}' not found in {CONFIG_PATH}")

    project_id = int(config["projects"][LANG_CODE])
    token = get_api_key(LANG_CODE)

    repos = sorted(
        d for d in Path(".").iterdir()
        if d.is_dir() and (d / "localization" / "eng").is_dir()
    )

    if not repos:
        print("No mod directories with localization/eng/ found.")
        return

    print(f"Found {len(repos)} mod(s) to check.")
    print(f"\n--- Uploading {LANG_CODE} translations to project {project_id} ---")

    configuration = paratranz_client.Configuration(host="https://paratranz.cn/api")
    configuration.api_key["Token"] = token

    async with paratranz_client.ApiClient(configuration) as api_client:
        api_instance = paratranz_client.FilesApi(api_client)
        try:
            existing_files = await api_instance.get_files(project_id)
            existing_dict = {f.name: f for f in existing_files}
        except Exception as e:
            print(f"  Warning: cannot list existing files: {e}")
            existing_dict = {}

        sem = asyncio.Semaphore(1)

        async def upload_with_limit(file_path, local_file):
            async with sem:
                await upload_translation_for_file(
                    api_instance, project_id, file_path, local_file, existing_dict, FORCE
                )

        tasks = []
        for repo in repos:
            lang_dir = repo / "localization" / LANG_CODE
            if not lang_dir.is_dir():
                continue

            for json_file in sorted(lang_dir.glob("*.json")):
                if json_file.name.lower() not in CATEGORY_FILES:
                    continue  # only cards.json / powers.json / relics.json

                # Must match the path used when the SOURCE file was created
                # (see para_upload.py): {repo}/localization/{lang}/{filename}
                file_path = f"{repo.name}/localization/{LANG_CODE}/{json_file.name}"
                tasks.append(upload_with_limit(file_path, json_file))

        if not tasks:
            print(f"  No local translation files found for '{LANG_CODE}'.")

        await asyncio.gather(*tasks)

    print("\nDone.")


if __name__ == "__main__":
    asyncio.run(main())