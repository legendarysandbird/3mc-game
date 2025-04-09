class_name Enemy extends CharacterBody2D

@export var speed: float = 100.0

@onready var player: Player = get_player_node()
@onready var detectionArea: Area2D = $DetectionDistance
@onready var deathArea: Area2D = $DeathCollision
var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity")

var calculated_velocity: Vector2
var player_detected: bool

func _ready() -> void:
	add_to_group(Groups.ENEMY)
	detectionArea.body_entered.connect(on_distance_detection_body_entered)
	detectionArea.body_exited.connect(on_distance_detection_body_exited)
	deathArea.body_entered.connect(on_death_detection_body_entered)

func _physics_process(delta: float) -> void:
	var x: float = calculate_horizontal_movement()
	var y: float = 0.0 if is_on_floor() else velocity.y + gravity * delta

	velocity = Vector2(x, y)
	move_and_slide()

func calculate_horizontal_movement() -> float:
	if not player_detected:
		return 0
	
	return -speed if global_position.x > player.global_position.x else speed

func get_player_node() -> Player:
	var player_node: Player = get_tree().get_first_node_in_group(Groups.PLAYER)
	assert(is_instance_valid(player_node))
	
	return player_node

func on_distance_detection_body_entered(body: Node2D) -> void:
	check_and_set_player_detected(body, true)

func on_distance_detection_body_exited(body: Node2D) -> void:
	check_and_set_player_detected(body, false)

func check_and_set_player_detected(body: Node2D, is_player_detected: bool) -> void:
	if body.is_in_group(Groups.PLAYER):
		player_detected = is_player_detected

func on_death_detection_body_entered(body: Node2D) -> void:
	if body.is_in_group(Groups.PLAYER):
		print("You were hit!")
