extends SceneTree

func _init():
    var args = OS.get_cmdline_user_args()
    if args.size() < 2:
        quit(1)
        return
    
    var mod_folder = args[0]
    var output_path = args[1]
    
    var packer = PCKPacker.new()
    packer.pck_start(output_path)
    _add_dir(packer, "res://" + mod_folder)
    # also pack the imported versions of all assets
    _add_imported(packer, "res://" + mod_folder)
    packer.flush(true)
    print("Packed: " + output_path)
    quit(0)

func _add_dir(packer: PCKPacker, path: String):
    var dir = DirAccess.open(path)
    if dir == null: return
    dir.list_dir_begin()
    var file = dir.get_next()
    while file != "":
        var full = path + "/" + file
        if dir.current_is_dir() and not file.begins_with("."):
            _add_dir(packer, full)
        else:
            packer.add_file(full, full)
        file = dir.get_next()

func _add_imported(packer: PCKPacker, path: String):
    var dir = DirAccess.open(path)
    if dir == null: return
    dir.list_dir_begin()
    var file = dir.get_next()
    while file != "":
        var full = path + "/" + file
        if dir.current_is_dir() and not file.begins_with("."):
            _add_imported(packer, full)
        elif file.ends_with(".import"):
            # read the import file to find the .ctex path
            var f = FileAccess.open(full, FileAccess.READ)
            if f:
                var content = f.get_as_text()
                f.close()
                for line in content.split("\n"):
                    if line.begins_with("path=") or line.begins_with("path."):
                        var ctex = line.split("=")[1].strip_edges().trim_prefix('"').trim_suffix('"')
                        if ResourceLoader.exists(ctex):
                            packer.add_file(ctex, ctex)
        file = dir.get_next()