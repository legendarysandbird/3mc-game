extends Node2D
@export var player_animation: Node 

func _ready() -> void:
	pass # Replace with function body.

func _process(_delta: float) -> void:
	look_at(get_global_mouse_position())
