extends Node
signal play_pressed
signal seed_set(new_seed: int)

@onready var button_play: Button = $PLAY
@onready var seed_edit: LineEdit = $SeedEdit

func _ready() -> void:
	for control: Control in [button_play, seed_edit]:
		assert(is_instance_valid(control))
	
	button_play.pressed.connect(on_play_button_pressed)

func on_play_button_pressed() -> void:
	var my_seed: int
	if seed_edit.text != "":
		my_seed = seed_edit.text.hash()
	else:
		my_seed = randi() % 1000000
	seed(my_seed)
	seed_set.emit(my_seed)
	
	play_pressed.emit()
