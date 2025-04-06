class_name Enemy extends CharacterBody2D

@export var speed: float = 100.0
@export var detection_distance: float = 50.0

@onready var player: Player = get_player_node()

var calculated_velocity: float

func _ready() -> void:
	add_to_group(Groups.ENEMY)

func _process(delta: float) -> void:
	calculated_velocity = calculate_velocity()

func _physics_process(delta: float) -> void:
	velocity.x = calculated_velocity * delta
	move_and_slide()

func calculate_velocity() -> float:
	if not can_detect_player():
		return 0.0

	var direction: float = 1.0 if global_position.x > player.global_position.x else -1.0
	return direction * speed

func can_detect_player() -> bool:
	var squared_distance: float = global_position.distance_squared_to(player.global_position)
	return squared_distance <= detection_distance ** 2

func get_player_node() -> Player:
	var player: Player = get_tree().get_first_node_in_group(Groups.PLAYER)
	assert(is_instance_valid(player))
	
	return player
