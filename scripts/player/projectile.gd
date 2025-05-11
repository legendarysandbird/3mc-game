class_name Projectile extends Area2D

const projectile_scene: PackedScene = preload("uid://da4rvan0yy3eh")

var velocity: Vector2
@onready var on_screen_notifier: VisibleOnScreenNotifier2D = $VisibleOnScreenNotifier2D

static func Create(starting_position: Vector2, starting_velocity: Vector2) -> Projectile:
	var projectile: Projectile = projectile_scene.instantiate()
	projectile.global_position = starting_position
	projectile.velocity = starting_velocity
	return projectile

func _ready() -> void:
	assert(is_instance_valid(on_screen_notifier))
	
	on_screen_notifier.screen_exited.connect(on_screen_exited)
	body_entered.connect(on_body_entered)

func _physics_process(delta: float) -> void:
	global_position += velocity * delta

func on_screen_exited() -> void:
	queue_free()

func on_body_entered(body: Node2D) -> void:
	if body.is_in_group(Groups.ENEMY):
		body.queue_free()
		queue_free()
