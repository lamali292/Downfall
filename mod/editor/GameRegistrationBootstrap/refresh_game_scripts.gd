@tool
extends EditorScript

# Diagnostic for game script binding. Open this file in the script editor and run it.
#
# GDScript rather than C# so the mod assembly stays free of GodotSharpEditor references.

const PROBE_PATHS := [
	"res://addons/mega_text/MegaLabel.cs",
	"res://addons/mega_text/MegaRichTextLabel.cs",
	"res://src/Core/Nodes/Ftue/NFtue.cs",
]


func _run() -> void:
	print("=== cached state ===")
	_probe(false)

	print("=== forcing cache-bypassing reload ===")
	_probe(true)


func _probe(replace: bool) -> void:
	for path in PROBE_PATHS:
		var script: Script

		if replace:
			script = ResourceLoader.load(path, "Script", ResourceLoader.CACHE_MODE_REPLACE)
		else:
			script = load(path)

		if script == null:
			print("  %s -> could not load" % path)
			continue

		var base_type: StringName = script.get_instance_base_type()
		var property_count: int = script.get_script_property_list().size()

		# A differing resource_path means the load was redirected and the bridge would have keyed the other string.
		var redirect := ""
		if script.resource_path != path:
			redirect = "  [!] loaded as '%s'" % script.resource_path

		print("  %s -> base '%s', %d properties%s" % [path, base_type, property_count, redirect])
