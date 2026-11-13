extends RefCounted

func load_bank(id: String):
	print("[DownfallAudio] Loading bank: " + id)
	var exists = FileAccess.file_exists(id)
	if not exists:
		print("[DownfallAudio] ERROR: Bank file not found at: " + id)
		return
	FmodServer.load_bank(id, FmodServer.FMOD_STUDIO_LOAD_BANK_NORMAL)
	FmodServer.update()
	print("[DownfallAudio] Successfully loaded bank: " + id)
	
	
func check_event(path: String) -> bool:
	return FmodServer.check_event_path(path)


func debug_all_events():
	var banks = FmodServer.get_all_banks()
	for bank in banks:
		print("[DownfallAudio] Bank: " + bank.get_path() + " events: " + str(bank.get_description_list().size()))
		
func debug_bank_methods():
	var descriptions = FmodServer.get_all_event_descriptions()
	for desc in descriptions:
		print("[DownfallAudio] Event: " + desc.get_path())

func get_all_event_paths() -> Array:
	# list all loaded banks and their events
	var result = []
	var banks = FmodServer.get_all_banks()
	for bank in banks:
		var events = bank.get_event_list()
		for e in events:
			result.append(e.get_path())
	return result
