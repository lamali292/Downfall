@tool
class_name DevToolsPlugin extends EditorPlugin

enum MenuItem{
    CLEAR_NUGET_CACHE = 0, 
}

var _menu_button: MenuButton
var _result_dialog: AcceptDialog


func _enter_tree() -> void :
    _menu_button = MenuButton.new()
    _menu_button.text = "Dev Tools"
    _menu_button.tooltip_text = "Developer tools and utilities"

    var popup: PopupMenu = _menu_button.get_popup()
    popup.add_item("Clear NuGet Cache", MenuItem.CLEAR_NUGET_CACHE)
    popup.id_pressed.connect(_on_menu_item_pressed)

    add_control_to_container(EditorPlugin.CONTAINER_TOOLBAR, _menu_button)

    _result_dialog = AcceptDialog.new()
    _result_dialog.title = "NuGet Cache Clear"
    add_child(_result_dialog)


func _exit_tree() -> void :
    remove_control_from_container(EditorPlugin.CONTAINER_TOOLBAR, _menu_button)
    _menu_button.queue_free()

    _result_dialog.queue_free()


func _on_menu_item_pressed(id: int) -> void :
    match id:
        MenuItem.CLEAR_NUGET_CACHE:
            _clear_nuget_cache()


func _get_dotnet_path() -> String:

    var editor_settings: EditorSettings = EditorInterface.get_editor_settings()
    if editor_settings.has_setting("dotnet/dotnet_cli_path"):
        var dotnet_path: String = editor_settings.get_setting("dotnet/dotnet_cli_path")
        if not dotnet_path.is_empty() and FileAccess.file_exists(dotnet_path):
            return dotnet_path


    var output: Array = []
    var find_cmd: String = "where" if OS.get_name() == "Windows" else "which"
    var exit_code: int = OS.execute(find_cmd, ["dotnet"], output)
    if exit_code == 0 and output.size() > 0:
        var path: String = str(output[0]).strip_edges().split("\n")[0]
        if FileAccess.file_exists(path):
            return path

    return ""


func _clear_nuget_cache() -> void :
    var dotnet_path: String = _get_dotnet_path()

    if dotnet_path.is_empty():
        _result_dialog.dialog_text = "Could not find dotnet CLI.\n\nEnsure dotnet is in PATH or set in Editor Settings."
        _result_dialog.popup_centered()
        return

    var output: Array = []

    var temp_dir: String = OS.get_cache_dir()
    var exit_code: int
    if OS.get_name() == "Windows":
        var cmd: String = "cd /d \"%s\" && \"%s\" nuget locals all --clear" % [temp_dir, dotnet_path]
        exit_code = OS.execute("cmd", ["/c", cmd], output, true)
    else:
        var cmd: String = "cd '%s' && '%s' nuget locals all --clear" % [temp_dir, dotnet_path]
        exit_code = OS.execute("bash", ["-c", cmd], output, true)

    var result_text: String = ""
    for line in output:
        result_text += str(line)

    if exit_code == 0:
        _result_dialog.dialog_text = "NuGet cache cleared successfully.\n\n" + result_text
    else:
        _result_dialog.dialog_text = "Failed (exit code: %d).\n\nPath: %s\n\n%s" % [exit_code, dotnet_path, result_text]

    _result_dialog.popup_centered()
