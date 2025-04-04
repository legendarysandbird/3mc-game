extends AnimatedSprite2D
@export var player: Node

func _ready() -> void:
	play()

func animate() -> void:
	if !player.is_on_floor():
		set_animation("idle")
		if !flip_h:
			flip_h = Input.is_action_pressed("player_left")
		else:
			flip_h = Input.is_action_pressed("player_left")
		return
	
	if player.velocity.x == 0 and !Input.is_anything_pressed():
		set_animation("idle")
		return
	
	if Input.is_action_pressed("player_left") != Input.is_action_pressed("player_right"):
		set_animation("walk")
	else:
		set_animation("idle")
		return
	
	flip_h = Input.is_action_pressed("player_left")


func _process(_delta: float) -> void:
		animate()
