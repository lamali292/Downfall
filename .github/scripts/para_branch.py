"""Per-branch Paratranz scoping, shared by the upload/download scripts.

A branch may ship .github/configs/paratranz-branch.json to say which of its mod folders sync
and under which folder name they appear on Paratranz:

    { "only_mods": ["SlimeBoss"], "aliases": { "SlimeBoss": "SlimeBossBeta" } }

Use case: the Slimeboss branch reworks SlimeBoss's strings while the old SlimeBoss is still
live, so its strings must land in a separate SlimeBossBeta folder instead of overwriting the
live translations. Branches without the file behave as before (every mod, no renaming).
"""
import json
import os
from pathlib import Path

_PATH = Path(".github/configs/paratranz-branch.json")
_config = json.loads(_PATH.read_text(encoding="utf-8-sig")) if _PATH.is_file() else {}

_aliases = _config.get("aliases", {})
_only = {m.lower() for m in _config.get("only_mods", [])}
# PARA_ONLY_MODS narrows a run further (comma-separated, e.g. the Collector branch's sync).
_only_env = {m.strip().lower() for m in os.environ.get("PARA_ONLY_MODS", "").split(",") if m.strip()}


def remote_mod(local_mod):
    """Folder name on Paratranz for a local mod directory."""
    return _aliases.get(local_mod, local_mod)


def local_mod(remote_name):
    """Local mod directory for a Paratranz folder, or None if this branch must not touch it.

    A folder that is the *unaliased* name of an aliased mod (the live SlimeBoss) is not ours.
    """
    for local, remote in _aliases.items():
        if remote_name == remote:
            return local
    return None if remote_name in _aliases else remote_name


def is_wanted(local_mod_name):
    name = local_mod_name.lower()
    if _only and name not in _only:
        return False
    if _only_env and name not in _only_env:
        return False
    return (Path(local_mod_name) / "localization" / "eng").is_dir()
