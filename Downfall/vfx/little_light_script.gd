extends CPUParticles2D
var random: = randi_range(0, 5)

func _ready():
	set_pre_process_time(random)
