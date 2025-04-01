extends AnimatedSprite2D
@export var player: Node

func _ready() -> void:
	set_animation("idle")

func animate() -> void:
	if !Input.is_anything_pressed():
		set_animation("idle")
		play()
	
	if Input.is_action_pressed("player_left") != Input.is_action_pressed("player_right"): # Poor man's XOR
		if player.velocity.y != 0:
			set_animation("idle")
			flip_h = player.velocity.x < 0
		elif player.velocity.x != 0:
			set_animation("walk")
			flip_h = player.velocity.x < 0
		play()
	else:
		set_animation("idle")
		play()

func _process(_delta: float) -> void:
		animate()
