@tool
extends EditorScript




const SCENES_DIR: = "res://scenes"
const SPRITES_PATTERN: = ".sprites/"


func _deterministic_uid(path: String) -> int:
    return hash(path) & 9223372036854775807


func _run() -> void :
    print("[MigrateUIDs] Starting scene UID migration...")

    var scenes: = _find_scenes_with_sprites(SCENES_DIR)
    print("[MigrateUIDs] Found %d scenes with sprite references" % scenes.size())

    var total_updates: = 0
    for scene_path in scenes:
        var updates: = _migrate_scene(scene_path)
        total_updates += updates

    print("[MigrateUIDs] Done. Updated %d UIDs across %d scenes." % [total_updates, scenes.size()])


func _find_scenes_with_sprites(dir_path: String) -> Array[String]:
    var result: Array[String] = []


    var abs_path: = ProjectSettings.globalize_path(dir_path)
    var dir: = DirAccess.open(abs_path)
    if dir == null:
        printerr("[MigrateUIDs] Failed to open directory: " + dir_path + " (abs: " + abs_path + ")")
        return result

    dir.list_dir_begin()
    var file_name: = dir.get_next()
    while file_name != "":
        var full_path: = dir_path.path_join(file_name)
        var full_abs_path: = abs_path.path_join(file_name)
        if dir.current_is_dir():
            if not file_name.begins_with("."):
                result.append_array(_find_scenes_with_sprites(full_path))
        elif file_name.ends_with(".tscn"):

            var content: = FileAccess.get_file_as_string(full_abs_path)
            if content.contains(SPRITES_PATTERN):
                result.append(full_abs_path)
        file_name = dir.get_next()

    return result


func _migrate_scene(scene_path: String) -> int:
    var content: = FileAccess.get_file_as_string(scene_path)
    var original_content: = content
    var updates: = 0


    var regex: = RegEx.new()
    regex.compile("uid=\"(uid://[^\"]+)\" path=\"(res://[^\"]*\\.sprites/[^\"]+\\.tres)\"")

    var matches: = regex.search_all(content)
    for m in matches:
        var old_uid: = m.get_string(1)
        var sprite_path: = m.get_string(2)


        var new_uid_int: = _deterministic_uid(sprite_path)
        var new_uid: = ResourceUID.id_to_text(new_uid_int)

        if old_uid != new_uid:
            content = content.replace("uid=\"%s\"" % old_uid, "uid=\"%s\"" % new_uid)
            print("[MigrateUIDs] %s: %s -> %s" % [sprite_path.get_file(), old_uid, new_uid])
            updates += 1


    if content != original_content:
        var file: = FileAccess.open(scene_path, FileAccess.WRITE)
        if file:
            file.store_string(content)
            print("[MigrateUIDs] Updated: " + scene_path)
        else:
            printerr("[MigrateUIDs] Failed to write: " + scene_path)

    return updates
