extends Node
@export var mob_array: Array[PackedScene]

@onready var spawn_timer: Timer = $SpawnTimer
@export var spawn_node: PathFollow2D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	for node: Node in [spawn_timer, spawn_node]:
		assert(is_instance_valid(node))
	
	for enemy: PackedScene in mob_array:
		assert(is_instance_valid(enemy))
	
	spawn_timer.timeout.connect(on_spawn_timer_timeout)

func on_spawn_timer_timeout() -> void:
	var mob_scene: PackedScene = mob_array.pick_random()
	var mob: Enemy = mob_scene.instantiate()
	
	spawn_node.set_progress_ratio(randf())
	mob.position = spawn_node.get_position()
	
	get_parent().add_child(mob)
