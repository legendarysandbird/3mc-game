extends Node

@onready var scene_temp_level: PackedScene = preload('uid://chvfto1w6w3um')
@onready var main_menu: Control = $"Main Menu"

@onready var game_info: Dictionary = {"current_seed": 0}

func _ready() -> void:
	for object: Object in [scene_temp_level, main_menu]:
		assert(is_instance_valid(object))

	main_menu.seed_set.connect(on_main_menu_seed_set)
	main_menu.play_pressed.connect(on_main_menu_play_pressed)
	
func on_main_menu_play_pressed() -> void:
	var temp_level: Node = scene_temp_level.instantiate()
	get_tree().root.add_child(temp_level)
	main_menu.queue_free()

func on_main_menu_seed_set(new_seed: int) -> void:
	game_info["current_seed"] = new_seed
