




































@tool
extends EditorPlugin

const ATLAS_BASE_PATH: = "res://images/atlases/"
const ATLAS_NAMES: = [
    "relic_atlas", 
    "relic_outline_atlas", 
    "power_atlas", 
    "card_atlas", 
    "ui_atlas", 
    "potion_atlas", 
    "potion_outline_atlas", 
    "era_atlas", 
    "compressed"
]

func _enter_tree() -> void :
    print("[AtlasGenerator] Plugin loaded, generating atlas sprites...")
    _generate_all_atlases()
    print("[AtlasGenerator] Done.")


func _generate_all_atlases() -> void :
    for atlas_name in ATLAS_NAMES:
        _generate_atlas(atlas_name)


func _generate_atlas(atlas_name: String) -> void :
    var tpsheet_path: = ATLAS_BASE_PATH + atlas_name + ".tpsheet"
    var sprites_dir: = ATLAS_BASE_PATH + atlas_name + ".sprites"


    if not _needs_regeneration(tpsheet_path, sprites_dir):
        return


    var tpsheet: = _load_tpsheet(tpsheet_path)
    if tpsheet == null:
        printerr("[AtlasGenerator] Failed to load: " + tpsheet_path)
        return

    print("[AtlasGenerator] Generating sprites for: " + atlas_name)


    _ensure_directory(sprites_dir)


    var textures: Array = tpsheet.get("textures", [])
    for texture_data in textures:
        var image_name: String = texture_data.get("image", "")
        var image_path: = ATLAS_BASE_PATH + image_name


        var atlas_texture: = ResourceLoader.load(image_path, "Texture2D")
        if atlas_texture == null:
            printerr("[AtlasGenerator] Failed to load atlas image: " + image_path)
            continue


        var sprites: Array = texture_data.get("sprites", [])
        for sprite in sprites:
            _create_sprite_tres(sprites_dir, sprite, atlas_texture, image_path)


    _write_marker_file(tpsheet_path, sprites_dir)


func _needs_regeneration(tpsheet_path: String, sprites_dir: String) -> bool:
    var marker_path: = sprites_dir + "/.generated"


    if not FileAccess.file_exists(marker_path):
        return true


    var tpsheet_time: = FileAccess.get_modified_time(tpsheet_path)
    var marker_file: = FileAccess.open(marker_path, FileAccess.READ)
    if marker_file == null:
        return true

    var marker_content: = marker_file.get_as_text().strip_edges()
    var parts: = marker_content.split("\n")
    var stored_time: = parts[0].to_int() if parts.size() > 0 else 0
    var stored_count: = parts[1].to_int() if parts.size() > 1 else 0


    if tpsheet_time != stored_time:
        return true


    if stored_count > 0:
        var actual_count: = _count_tres_files_recursive(sprites_dir)
        if actual_count < stored_count:
            return true

    return false


func _count_tres_files_recursive(dir_path: String) -> int:
    var count: = 0
    var dir: = DirAccess.open(dir_path)
    if dir == null:
        return 0

    dir.list_dir_begin()
    var file_name: = dir.get_next()
    while file_name != "":
        if not file_name.begins_with("."):
            var full_path: = dir_path.path_join(file_name)
            if dir.current_is_dir():
                count += _count_tres_files_recursive(full_path)
            elif file_name.ends_with(".tres"):
                count += 1
        file_name = dir.get_next()

    return count


func _write_marker_file(tpsheet_path: String, sprites_dir: String) -> void :
    var marker_path: = sprites_dir + "/.generated"
    var tpsheet_time: = FileAccess.get_modified_time(tpsheet_path)
    var file_count: = _count_tres_files_recursive(sprites_dir)

    var file: = FileAccess.open(marker_path, FileAccess.WRITE)
    if file:
        file.store_string("%d\n%d" % [tpsheet_time, file_count])


func _load_tpsheet(path: String) -> Variant:
    var file: = FileAccess.open(path, FileAccess.READ)
    if file == null:
        return null

    var json_text: = file.get_as_text()
    var json: = JSON.parse_string(json_text)
    return json


func _ensure_directory(path: String) -> void :
    var dir: = DirAccess.open("res://")
    if dir and not dir.dir_exists(path):
        dir.make_dir_recursive(path)


func _deterministic_uid(path: String) -> int:





    return hash(path) & 9223372036854775807


func _create_sprite_tres(sprites_dir: String, sprite: Dictionary, atlas_texture: Texture2D, image_path: String) -> void :
    var filename: String = sprite.get("filename", "")
    if filename.is_empty():
        return


    var sprite_name: = filename.get_basename()
    var tres_path: = sprites_dir + "/" + sprite_name + ".tres"


    var tres_dir: = tres_path.get_base_dir()
    _ensure_directory(tres_dir)


    var uid: = _deterministic_uid(tres_path)
    var uid_text: = ResourceUID.id_to_text(uid)


    var region: Dictionary = sprite.get("region", {})
    var margin: Dictionary = sprite.get("margin", {})


    var content: = "[gd_resource type=\"AtlasTexture\" load_steps=2 format=3 uid=\"%s\"]\n\n" % uid_text
    content += "[ext_resource type=\"Texture2D\" path=\"%s\" id=\"1\"]\n\n" % image_path
    content += "[resource]\n"
    content += "atlas = ExtResource(\"1\")\n"
    content += "region = Rect2(%d, %d, %d, %d)\n" % [region.get("x", 0), region.get("y", 0), region.get("w", 0), region.get("h", 0)]

    var margin_rect: = Rect2(margin.get("x", 0), margin.get("y", 0), margin.get("w", 0), margin.get("h", 0))
    if margin_rect != Rect2(0, 0, 0, 0):
        content += "margin = Rect2(%d, %d, %d, %d)\n" % [int(margin_rect.position.x), int(margin_rect.position.y), int(margin_rect.size.x), int(margin_rect.size.y)]

    var file: = FileAccess.open(tres_path, FileAccess.WRITE)
    if file:
        file.store_string(content)
    else:
        printerr("[AtlasGenerator] Failed to save: " + tres_path)
