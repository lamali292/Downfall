extends SceneTree

func _init():
	var args = OS.get_cmdline_user_args()
	if args.size() < 2:
		printerr("Error: Missing arguments. Usage: godot -s script.gd -- <mod_folder> <output_path>")
		quit(1)
		return
		
	var mod_folder = args[0].trim_suffix("/").trim_prefix("res://")
	var output_path = args[1]
	
	var packer = PCKPacker.new()
	var err = packer.pck_start(output_path)
	
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

func _pack_folder_recursive(packer: PCKPacker, path: String):
	var dir = DirAccess.open(path)
	if dir == null: 
		return
		
	dir.list_dir_begin()
	var file_name = dir.get_next()
	
	while file_name != "":
		# Ignore hidden system directories/files
		if file_name.begins_with("."):
			file_name = dir.get_next()
			continue
			
		var full_path = path + "/" + file_name
		
		if dir.current_is_dir():
			_pack_folder_recursive(packer, full_path)
		else:
			# 1. Pack the source file itself (e.g., scene, script, raw png)
			var err = packer.add_file(full_path, full_path)
			if err != OK:
				printerr("Failed to pack file: ", full_path)
			
			# 2. If it's a resource metadata file, find its compiled cache dependency
			if file_name.ends_with(".import"):
				_pack_imported_dependency(packer, full_path)
				
		file_name = dir.get_next()

func _pack_imported_dependency(packer: PCKPacker, import_file_path: String):
	var config = ConfigFile.new()
	var err = config.load(import_file_path)
	if err != OK:
		return

	# Godot 4 stores the actual imported file path under [remap] -> "path"
	# Or for textures under specific format keys like "path.s3tc_bptc"
	if config.has_section("remap"):
		for key in config.get_section_keys("remap"):
			if key == "path" or key.begins_with("path."):
				var cache_path = config.get_value("remap", key, "")
				if cache_path != "" and FileAccess.file_exists(cache_path):
					packer.add_file(cache_path, cache_path)