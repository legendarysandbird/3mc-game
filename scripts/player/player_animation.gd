extends AnimatedSprite2D
@export var player: Node

func _ready() -> void:
	assert(is_instance_valid(player), "Player instance is not valid")
	play()

func animate() -> void:
	if Input.is_action_pressed("player_left") != Input.is_action_pressed("player_right"):
		set_animation("walk")
	else:
		set_animation("idle")
		return
	
	flip_h = Input.is_action_pressed("player_left")


func _process(_delta: float) -> void:
		animate()
