extends AnimatedSprite2D

@export var enemy: CharacterBody2D

func _ready() -> void:
	assert(is_instance_valid(enemy))
	play()

func _process(_delta: float) -> void:
	var animation_name: String = "aggro" if enemy.velocity != Vector2.ZERO else "idle"
	set_animation(animation_name)
