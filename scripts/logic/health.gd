class_name Health extends Node
@export var health_pool: int
@onready var max_health: int = health_pool

signal death
signal health_pool_changed

func change_health(value: int) -> void:
	health_pool = clamp(health_pool + value, 0, max_health)
	if health_pool == 0:
		death.emit()
	
	health_pool_changed.emit()
