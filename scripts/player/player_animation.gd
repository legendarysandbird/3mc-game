extends AnimatedSprite2D
@export var player: Node
@onready var scale_x_cache: float = scale.x

func _ready() -> void:
	assert(is_instance_valid(player), "Player instance is not valid")
	play()
	

func animate() -> void:
	if Input.is_action_pressed("player_left") != Input.is_action_pressed("player_right"):
		set_animation("walk")
	else:
		set_animation("idle")
		return
	
	scale.x = scale_x_cache * (-1 if Input.is_action_pressed("player_left") else 1)

func _process(_delta: float) -> void:
		animate()
