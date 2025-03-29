extends AnimatedSprite2D
@export var player: Node

func _ready() -> void:
	set_animation("still")

func animate() -> void:
	if !Input.is_anything_pressed():
		set_animation("still")
	
	if Input.is_action_pressed("player_left") or Input.is_action_pressed("player_right"):
		set_animation("walk")
		if player.velocity.x > 0:
			flip_h = false
		elif player.velocity.x < 0:
			flip_h = true
		play()
	else:
		stop()
