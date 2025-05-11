class_name Enemy extends CharacterBody2D

@export var speed: float = 100.0

@onready var player: Player = get_tree().get_first_node_in_group(Groups.PLAYER)
@onready var detectionArea: Area2D = $DetectionDistance
@onready var deathArea: Area2D = $DeathCollision
@onready var healthPool: Node = $Health

var calculated_velocity: Vector2
var player_detected: bool

func _ready() -> void:
	for node: Node in [player, detectionArea, deathArea, healthPool]:
		assert(is_instance_valid(node))

	add_to_group(Groups.ENEMY)
	detectionArea.body_entered.connect(on_distance_detection_body_entered)
	detectionArea.body_exited.connect(on_distance_detection_body_exited)
	deathArea.body_entered.connect(on_death_detection_body_entered)
	healthPool.death.connect(on_health_pool_death)

func _physics_process(_delta: float) -> void:
	velocity = calculate_velocity()
	move_and_slide()

func calculate_velocity() -> Vector2:
	if not player_detected:
		return Vector2.ZERO
	
	var direction: Vector2 = (player.global_position - global_position).normalized()
	return direction * speed

func on_distance_detection_body_entered(body: Node2D) -> void:
	check_and_set_player_detected(body, true)

func on_distance_detection_body_exited(body: Node2D) -> void:
	check_and_set_player_detected(body, false)

func check_and_set_player_detected(body: Node2D, is_player_detected: bool) -> void:
	if body.is_in_group(Groups.PLAYER):
		player_detected = is_player_detected

func on_death_detection_body_entered(body: Node2D) -> void:
	if body.is_in_group(Groups.PLAYER):
		healthPool.change_health(-1)

func on_health_pool_death() -> void:
	print("enemy died")
	queue_free()
