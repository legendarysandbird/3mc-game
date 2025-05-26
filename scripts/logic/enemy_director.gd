class_name Director extends Node

@export var mob_array: Array[PackedScene]

@onready var spawn_timer: Timer = $SpawnTimer
@export var spawn_node: PathFollow2D

signal enemy_death

func _ready() -> void:
	for node: Node in [spawn_timer, spawn_node]:
		assert(is_instance_valid(node))
	
	for enemy: PackedScene in mob_array:
		assert(is_instance_valid(enemy))
	
	spawn_timer.timeout.connect(on_spawn_timer_timeout)
	
	#Give us one enemy to start with so we aren't waiting 5 seconds
	on_spawn_timer_timeout()

func on_spawn_timer_timeout() -> void:
	var mob_scene: PackedScene = mob_array.pick_random()
	var mob: Enemy = mob_scene.instantiate()
	
	spawn_node.set_progress_ratio(randf())
	print(spawn_node.progress_ratio)
	mob.position = spawn_node.get_position()
	mob.death.connect(on_enemy_death)
	add_child(mob)

#Event bus for communicating enemy death with the score counter
func on_enemy_death() -> void:
	enemy_death.emit()
