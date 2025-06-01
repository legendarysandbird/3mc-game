class_name Director extends Node

@export var mob_types: Array[PackedScene]

@onready var spawn_timer: Timer = $SpawnTimer
@export var spawn_node: PathFollow2D

signal enemy_death

func _ready() -> void:
	for node: Node in [spawn_timer, spawn_node]:
		assert(is_instance_valid(node))
	
	for enemy: PackedScene in mob_types:
		assert(is_instance_valid(enemy))
	
	spawn_timer.timeout.connect(on_spawn_timer_timeout)
	
	#Give us one enemy to start with so we aren't waiting 5 seconds
	#Coordinates are where the debugging enemy usually is
	add_child(create_enemy(Vector2(765, 253)))

func create_enemy(pos: Vector2 = Vector2(0,0)) -> Enemy:
	var mob_scene: PackedScene = mob_types.pick_random()
	var mob: Enemy = mob_scene.instantiate()
	
	if pos:
		mob.position = pos
	else:
		spawn_node.set_progress_ratio(randf())
		mob.position = spawn_node.get_position()
	
	mob.death.connect(on_enemy_death)
	return mob
	
func on_spawn_timer_timeout() -> void:
	add_child(create_enemy())

#Event bus for communicating enemy death with the score counter
func on_enemy_death() -> void:
	enemy_death.emit()
