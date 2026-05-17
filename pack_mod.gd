extends SceneTree

func _init() -> void:
	var args: PackedStringArray = OS.get_cmdline_user_args()
	if args.size() < 2:
		printerr("Error: Missing arguments. Usage: godot -s script.gd -- <mod_folder> <output_path>")
		quit(1)
		return
		
	var mod_folder: String = args[0].trim_suffix("/").trim_prefix("res://")
	var output_path: String = args[1]
	
	var packer: PCKPacker = PCKPacker.new()
	# Godot 4 requires explicit alignment (16 is recommended for asset optimization)
	var err: int = packer.pck_start(output_path, 16)
	
	if err != OK:
		printerr("Failed to start PCK packer. Error code: ", err)
		quit(1)
		return
		
	print("Packing folder: res://" + mod_folder)
	_pack_folder_recursive(packer, "res://" + mod_folder)
	
	err = packer.flush(true)
	if err == OK:
		print("Successfully Packed: " + output_path)
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
		# Only skip current/parent references and the engine internal cache folder
		# This leaves .uid files intact
		if file_name == "." or file_name == ".." or file_name == ".godot":
			file_name = dir.get_next()
			continue
			
		var full_path: String = path + "/" + file_name
		
		if dir.current_is_dir():
			_pack_folder_recursive(packer, full_path)
		else:
			# 1. Pack the source file asset itself
			var err: int = packer.add_file(full_path, full_path)
			if err != OK:
				printerr("Failed to pack file: ", full_path)
			
			# 2. If it's a metadata link file, hunt down its compiled cache files
			if file_name.ends_with(".import"):
				_pack_imported_dependency(packer, full_path)
				
		file_name = dir.get_next()

func _pack_imported_dependency(packer: PCKPacker, import_file_path: String) -> void:
	var config: ConfigFile = ConfigFile.new()
	var err: int = config.load(import_file_path)
	if err != OK:
		return

	# 1. Direct targeted lookup for the primary remapped cache file
	if config.has_section("remap"):
		# Check standard path
		var standard_path = config.get_value("remap", "path", "")
		if standard_path is String and standard_path != "" and FileAccess.file_exists(standard_path):
			packer.add_file(standard_path, standard_path)
		
		# Check explicit desktop texture compression format path (s3tc_bptc)
		var s3tc_path = config.get_value("remap", "path.s3tc_bptc", "")
		if s3tc_path is String and s3tc_path != "" and FileAccess.file_exists(s3tc_path):
			packer.add_file(s3tc_path, s3tc_path)

	# 2. Extract explicit multi-file chunk arrays or dependency layouts safely
	if config.has_section("deps"):
		if config.has_section_key("deps", "dest_files"):
			var dest_files = config.get_value("deps", "dest_files")
			
			if dest_files is String:
				if dest_files != "" and FileAccess.file_exists(dest_files):
					packer.add_file(dest_files, dest_files)
			elif dest_files != null:
				for cache_path in dest_files:
					if cache_path is String and cache_path != "" and FileAccess.file_exists(cache_path):
						packer.add_file(cache_path, cache_path)