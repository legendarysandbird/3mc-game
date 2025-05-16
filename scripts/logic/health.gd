class_name Health extends Node
@export var max_health: int
@onready var health_pool: int = max_health

signal death
signal health_pool_changed

func _ready() -> void:
	assert(max_health > 0)

func change_health(value: int) -> void:
	health_pool = clamp(health_pool + value, 0, max_health)
	if health_pool == 0:
		death.emit()
	
	health_pool_changed.emit()
