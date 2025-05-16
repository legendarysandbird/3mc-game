class_name Enemy extends CharacterBody2D

@export var speed: float = 100.0
@export var contact_damage: int = 1

@onready var player: Player = get_tree().get_first_node_in_group(Groups.PLAYER)
@onready var detectionArea: Area2D = $DetectionDistance
@onready var hitbox: Area2D = $Hitbox
@onready var health_pool: Health = $Health

var calculated_velocity: Vector2
var player_detected: bool

func _ready() -> void:
	for node: Node in [player, detectionArea, hitbox, health_pool]:
		assert(is_instance_valid(node))

	add_to_group(Groups.ENEMY)
	detectionArea.body_entered.connect(on_distance_detection_body_entered)
	detectionArea.body_exited.connect(on_distance_detection_body_exited)
	hitbox.area_entered.connect(on_hitbox_area_entered)
	health_pool.death.connect(on_health_pool_death)

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

func on_hitbox_area_entered(area: Node2D) -> void:
	if area.is_in_group(Groups.PLAYER_PROJECTILE):
		health_pool.change_health(area.damage)
		area.queue_free()

func on_health_pool_death() -> void:
	print("enemy died")
	queue_free()
