extends AnimatedSprite2D
@export var player: Node

func _ready() -> void:
	set_animation("still")

func animate() -> void:
	if !Input.is_anything_pressed():
		set_animation("still")
	
	if Input.is_action_pressed("player_left") != Input.is_action_pressed("player_right"): # Poor man's XOR
		set_animation("walk")
		if player.velocity.y != 0:
			stop()
		if player.velocity.x != 0:
			flip_h = player.velocity.x < 0
		play()
	else:
		stop()

func _process(_delta: float) -> void:
		animate()
