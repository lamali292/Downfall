@tool
extends EditorPlugin

# This plugin must be listed first in project.godot -> [editor_plugins] -> enabled.
#
# Force the mod dll to load, which runs GameScriptRegistration's [ModuleInitializer] and
# registers sts2.dll's [ScriptPath] entries with ScriptManagerBridge.
#
# Written in GDScript on purpose. An EditorPlugin subclass in C# would reference GodotSharpEditor,
# and the game's ModManager calls Module.GetTypes() on it at load time, which
# fails hard because GodotSharpEditor.dll does not ship with the game.

const BOOTSTRAP_PATH := "res://editor/GameRegistrationBootstrap/EditorBootstrap.cs"


func _enter_tree() -> void:
	_force_mod_assembly_load()

func _force_mod_assembly_load() -> void:
	var bootstrap: Script = load(BOOTSTRAP_PATH)

	if bootstrap == null:
		push_warning("[Game Script Registration] Could not load %s - build the C# project." % BOOTSTRAP_PATH)
		return

	# Attaching the script to an object is what forces the managed type, and therefore the assembly, to be resolved.
	var probe := RefCounted.new()
	probe.set_script(bootstrap)

	if bootstrap.get_instance_base_type() == &"":
		push_warning("[Game Script Registration] EditorBootstrap is not bound to its type yet - C# script "
			+ "registration has not run at plugin load time.")
