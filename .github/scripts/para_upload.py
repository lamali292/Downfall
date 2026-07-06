import asyncio
import json
import os
from pathlib import Path

import paratranz_client
from pydantic import ValidationError

CONFIG_PATH = Path(".github/configs/paratranz.json")
with open(CONFIG_PATH, encoding="utf-8") as f:
    config = json.load(f)


def get_api_key(lang_code):
    env_name = f"PARATRANZ_API_KEY_{lang_code.upper()}"
    token = os.environ.get(env_name)
    if not token:
        raise EnvironmentError(f"Environment variable {env_name} is not set.")
    return token


async def upload_file(api_client, project_id, file_path, local_file, existing_files_dict):
    api_instance = paratranz_client.FilesApi(api_client)
    existing_file = existing_files_dict.get(file_path)

    max_retries = 3
    for attempt in range(max_retries):
        try:
            if existing_file:
                await api_instance.update_file(project_id, file_id=existing_file.id, file=str(local_file))
                print(f"  Updated: {file_path}")
            else:
                path = str(Path(file_path).parent).replace("\\", "/")
                if path:
                    path += "/"
                await api_instance.create_file(project_id, file=str(local_file), path=path)
                print(f"  Created: {file_path}")
            break
        except ValidationError:
            print(f"  OK: {file_path}")
            break
        except Exception as e:
            if attempt < max_retries - 1:
                wait_time = 2 ** attempt
                print(f"  Retry {attempt + 1}/{max_retries} for {file_path}: {e}")
                await asyncio.sleep(wait_time)
            else:
                print(f"  Failed: {file_path} - {e}")


async def main():
    repos = sorted(d for d in Path(".").iterdir() if d.is_dir() and (d / "localization" / "eng").is_dir())

    if not repos:
        print("No mod directories with localization/eng/ found.")
        return

    print(f"Found {len(repos)} mod(s) to sync.")

    for lang_code, project_id in config["projects"].items():
        project_id = int(project_id)
        token = get_api_key(lang_code)
        print(f"\n--- Uploading to project {project_id} ({lang_code}) ---")

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
                    await upload_file(api_client, project_id, file_path, local_file, existing_dict)

            tasks = []
            for repo in repos:
                eng_dir = repo / "localization" / "eng"
                for json_file in sorted(eng_dir.glob("*.json")):
                    file_path = f"{repo.name}/localization/{lang_code}/{json_file.name}"
                    tasks.append(upload_with_limit(file_path, json_file))

            await asyncio.gather(*tasks)

    print("\nDone.")


if __name__ == "__main__":
    asyncio.run(main())
