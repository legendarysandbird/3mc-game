class_name Director extends Node

@export var mob_types: Array[PackedScene]
@export var spawn_node: PathFollow2D
@export var max_mob_count: int

@onready var spawn_timer: Timer = $SpawnTimer
@onready var cur_mob_count: int = 0

signal enemy_death

func _ready() -> void:
	for node: Node in [spawn_timer, spawn_node]:
		assert(is_instance_valid(node))
	
	for enemy: PackedScene in mob_types:
		assert(is_instance_valid(enemy))
	
	assert(max_mob_count > 0)
	assert(cur_mob_count == 0)
	
	spawn_timer.timeout.connect(on_spawn_timer_timeout)
	
	#Give us one enemy to start with so we aren't waiting 5 seconds
	#Coordinates are where the debugging enemy usually is
	add_child(create_enemy_at(Vector2(765, 253)))
	cur_mob_count += 1
	
	spawn_timer.start()
	
func create_enemy() -> Enemy:
	spawn_node.set_progress_ratio(randf())
	return create_enemy_at(spawn_node.get_position())
	
func create_enemy_at(pos: Vector2) -> Enemy:
	var mob_scene: PackedScene = mob_types.pick_random()
	var mob: Enemy = mob_scene.instantiate()
	
	mob.position = pos
	
	mob.death.connect(on_enemy_death)
	return mob
	
func on_spawn_timer_timeout() -> void:
	if cur_mob_count < max_mob_count:
		add_child(create_enemy())
		cur_mob_count += 1
	
#Event bus for communicating enemy death with the score counter
func on_enemy_death() -> void:
	enemy_death.emit()
	cur_mob_count -= 1
	
