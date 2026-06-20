extends SceneTree

# Raw source formats Godot compiles into .ctex / other cache files.
# These are never loaded at runtime only their compiled counterparts are.
const SKIP_EXTENSIONS: Array[String] = [
	".png", ".jpg", ".jpeg", ".webp", ".bmp", ".svg", ".tga",
	".ogg", ".mp3", ".wav",
]

const REMAP_PATH_KEYS: Array[String] = [
	"path.s3tc_bptc",   
	"path.etc2_astc",  
	"path",            
]

func _init() -> void:
	var args: PackedStringArray = OS.get_cmdline_user_args()
	if args.size() < 2:
		printerr("Error: Missing arguments. Usage: godot -s script.gd -- <mod_folder> <output_path>")
		quit(1)
		return

	var mod_folder: String = args[0].trim_suffix("/").trim_prefix("res://")
	var output_path: String = args[1]

	var packer: PCKPacker = PCKPacker.new()
	var err: int = packer.pck_start(output_path, 16)

	if err != OK:
		printerr("Failed to start PCK packer. Error code: ", err)
		quit(1)
		return

	print("Packing folder: res://" + mod_folder)
	_pack_folder_recursive(packer, "res://" + mod_folder)

	err = packer.flush(true)
	if err == OK:
		print("Successfully packed: " + output_path)
		quit(0)
	else:
		printerr("Failed to flush PCK file. Error code: ", err)
		quit(1)


func _pack_folder_recursive(packer: PCKPacker, path: String) -> void:
	var dir: DirAccess = DirAccess.open(path)
	if dir == null:
		return

	dir.list_dir_begin()
	var file_name: String = dir.get_next()

	while file_name != "":
		# Skip filesystem noise and the editor's internal cache folder.
		if file_name == "." or file_name == ".." or file_name == ".godot":
			file_name = dir.get_next()
			continue

		var full_path: String = path + "/" + file_name

		if dir.current_is_dir():
			_pack_folder_recursive(packer, full_path)
		else:
			# Skip raw source images Godot loads only the compiled .ctex
			var is_raw_image: bool = SKIP_EXTENSIONS.any(
				func(ext: String) -> bool: return file_name.ends_with(ext)
			)
			if not is_raw_image:
				var err: int = packer.add_file(full_path, full_path)
				if err != OK:
					printerr("Failed to pack file: ", full_path)

			# For import metadata files, also pack the compiled cache asset.
			if file_name.ends_with(".import"):
				_pack_imported_dependency(packer, full_path)

		file_name = dir.get_next()


func _pack_imported_dependency(packer: PCKPacker, import_file_path: String) -> void:
	var config: ConfigFile = ConfigFile.new()
	if config.load(import_file_path) != OK:
		return

	if not config.has_section("remap"):
		return

	for key in REMAP_PATH_KEYS:
		var cache_path = config.get_value("remap", key, "")
		if cache_path is String and cache_path != "" and FileAccess.file_exists(cache_path):
			var err: int = packer.add_file(cache_path, cache_path)
			if err != OK:
				printerr("Failed to pack imported cache: ", cache_path)
			break