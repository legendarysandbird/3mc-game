class_name Enemy extends CharacterBody2D

@export var speed: float = 100.0
@export var contact_damage: int = 1

@onready var visibility_notifier: VisibleOnScreenNotifier2D = $VisibleOnScreenNotifier2D
@onready var hitbox: Area2D = $Hitbox
@onready var health_pool: Health = $Health
@onready var player: Player = get_tree().root.find_children("*", "Player", true, false)[0]

signal death

var calculated_velocity: Vector2

func _ready() -> void:
	for node: Node in [visibility_notifier, hitbox, health_pool, player]:
		assert(is_instance_valid(node))

	hitbox.area_entered.connect(on_hitbox_area_entered)
	health_pool.empty.connect(on_health_pool_empty)

func _physics_process(_delta: float) -> void:
	velocity = calculate_velocity()
	move_and_slide()

func calculate_velocity() -> Vector2:
	if not (visibility_notifier.is_on_screen() and is_instance_valid(player)):
		return Vector2.ZERO

	var direction: Vector2 = (player.global_position - global_position).normalized()
	return direction * speed

func on_hitbox_area_entered(area: Node2D) -> void:
	if area is Projectile:
		health_pool.change_health(area.damage)
		area.queue_free()

func on_health_pool_empty() -> void:
	death.emit()
	queue_free()
