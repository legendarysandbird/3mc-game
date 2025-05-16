extends ProgressBar

@export var health_pool: Health

func _ready() -> void:
	assert(is_instance_valid(health_pool))
	
	health_pool.health_pool_changed.connect(on_health_pool_changed)
	max_value = health_pool.max_health
	value = health_pool.health_pool
	
func on_health_pool_changed() -> void:
	value = health_pool.health_pool
