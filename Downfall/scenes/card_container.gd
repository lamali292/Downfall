extends Node2D

func _ready():
	# Set camera to center on the card and zoom in
	var camera = $Camera2D
	var card = $Card
	
	camera.global_position = card.global_position
	
	# Make sure viewport is transparent
	get_viewport().transparent_bg = true
	
	await get_tree().process_frame
	await get_tree().process_frame
	
	var image = get_viewport().get_texture().get_image()
	image.decompress()
	image.save_png("user://output.png")
	print("Saved to: ", OS.get_user_data_dir(), "/output.png")
	get_tree().quit()
