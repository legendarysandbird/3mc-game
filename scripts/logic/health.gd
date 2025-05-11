class_name Health extends Node
@export var health: int
@onready var maxHealth: int = health

signal death

func change_health(value: int) -> void:
	health = clamp(health + value, 0, maxHealth)
	if health == 0:
		death.emit()
