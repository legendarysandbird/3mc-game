class_name Projectile extends Area2D

const projectile_scene: PackedScene = preload("uid://da4rvan0yy3eh")

var velocity: Vector2
var damage: int
@onready var on_screen_notifier: VisibleOnScreenNotifier2D = $VisibleOnScreenNotifier2D

static func Create(starting_position: Vector2, starting_velocity: Vector2, starting_damage: int = -1) -> Projectile:
	var projectile: Projectile = projectile_scene.instantiate()
	projectile.global_position = starting_position
	projectile.velocity = starting_velocity
	projectile.damage = starting_damage
	return projectile

func _ready() -> void:
	assert(is_instance_valid(on_screen_notifier))
	on_screen_notifier.screen_exited.connect(on_screen_exited)
	body_entered.connect(on_body_entered)
	look_at(self.global_position + self.velocity)

func _physics_process(delta: float) -> void:
	global_position += velocity * delta

func on_screen_exited() -> void:
	queue_free()

func on_body_entered(body: Node) -> void:
	if body is TileMapLayer:
		queue_free()
