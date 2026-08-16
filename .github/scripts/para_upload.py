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
    env_name = f"PARATRANZ_API_KEY"
    token = os.environ.get(env_name)
    if not token:
        raise EnvironmentError(f"Environment variable {env_name} is not set.")
    return token


async def upload_file(api_client, project_id, file_path, local_file, existing_files_dict, index, total, counts):
    api_instance = paratranz_client.FilesApi(api_client)
    existing_file = existing_files_dict.get(file_path)

    max_retries = 3
    for attempt in range(max_retries):
        try:
            if existing_file:
                await api_instance.update_file(project_id, file_id=existing_file.id, file=str(local_file))
                print(f"  [{index}/{total}] Updated: {file_path}")
                counts["updated"] += 1
            else:
                path = str(Path(file_path).parent).replace("\\", "/")
                if path:
                    path += "/"
                await api_instance.create_file(project_id, file=str(local_file), path=path)
                print(f"  [{index}/{total}] Created: {file_path}")
                counts["created"] += 1
            break
        except ValidationError:
            print(f"  [{index}/{total}] OK (no change): {file_path}")
            counts["ok"] += 1
            break
        except Exception as e:
            if attempt < max_retries - 1:
                wait_time = 2 ** attempt
                print(f"  [{index}/{total}] Retry {attempt + 1}/{max_retries} for {file_path}: {e}")
                await asyncio.sleep(wait_time)
            else:
                print(f"  [{index}/{total}] Failed: {file_path} - {e}")
                counts["failed"] += 1


async def main():
    repos = sorted(d for d in Path(".").iterdir() if d.is_dir() and (d / "localization" / "eng").is_dir())

    if not repos:
        print("No mod directories with localization/eng/ found.")
        return

    print(f"Found {len(repos)} mod(s) to sync: {', '.join(r.name for r in repos)}")

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
                print(f"  {len(existing_dict)} file(s) already on the project")
            except Exception as e:
                print(f"  Warning: cannot list existing files: {e}")
                existing_dict = {}

            # Collect the work first so we know the total for progress output.
            jobs = []
            for repo in repos:
                eng_dir = repo / "localization" / "eng"
                for json_file in sorted(eng_dir.glob("*.json")):
                    file_path = f"{repo.name}/localization/{lang_code}/{json_file.name}"
                    jobs.append((file_path, json_file))

            total = len(jobs)
            print(f"  {total} file(s) to upload")

            counts = {"created": 0, "updated": 0, "ok": 0, "failed": 0}
            sem = asyncio.Semaphore(1)
            progress = {"n": 0}

            async def upload_with_limit(file_path, local_file):
                async with sem:
                    progress["n"] += 1
                    await upload_file(api_client, project_id, file_path, local_file,
                                      existing_dict, progress["n"], total, counts)

            await asyncio.gather(*(upload_with_limit(fp, lf) for fp, lf in jobs))

            print(f"  {lang_code} summary: {counts['created']} created, "
                  f"{counts['updated']} updated, {counts['ok']} unchanged, "
                  f"{counts['failed']} failed")

    print("\nDone.")


if __name__ == "__main__":
    asyncio.run(main())