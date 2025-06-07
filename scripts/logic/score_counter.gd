extends Label

@export var director: Director

@onready var score: int = 0

func _ready() -> void:
	assert(is_instance_valid(director))
	director.enemy_death.connect(on_enemy_death)
	
	text = str(score)

func on_enemy_death() -> void:
	score += 1
	text = str(score)
