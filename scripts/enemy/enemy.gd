class_name Enemy extends CharacterBody2D

@export var speed: float = 100.0
@export var contact_damage: int = 1

@onready var detectionArea: Area2D = $DetectionDistance
@onready var hitbox: Area2D = $Hitbox
@onready var health_pool: Health = $Health

var calculated_velocity: Vector2
var target: Player

func _ready() -> void:
	for node: Node in [detectionArea, hitbox, health_pool]:
		assert(is_instance_valid(node))

	detectionArea.body_entered.connect(on_distance_detection_body_entered)
	detectionArea.body_exited.connect(on_distance_detection_body_exited)
	hitbox.area_entered.connect(on_hitbox_area_entered)
	health_pool.death.connect(on_health_pool_death)

func _physics_process(_delta: float) -> void:
	velocity = calculate_velocity()
	move_and_slide()

func calculate_velocity() -> Vector2:
	if target == null:
		return Vector2.ZERO
	
	var direction: Vector2 = (target.global_position - global_position).normalized()
	return direction * speed

func on_distance_detection_body_entered(body: Node2D) -> void:
	check_and_set_player_detected(body, true)

func on_distance_detection_body_exited(body: Node2D) -> void:
	check_and_set_player_detected(body, false)

func check_and_set_player_detected(body: Node2D, is_player_detected: bool) -> void:
	if body is Player:
		target = body if is_player_detected else null

func on_hitbox_area_entered(area: Node2D) -> void:
	if area is Projectile:
		health_pool.change_health(area.damage)
		area.queue_free()

func on_health_pool_death() -> void:
	print("enemy died")
	queue_free()
