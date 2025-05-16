class_name Player extends CharacterBody2D

const MOVE_SPEED: float = 200.0
const JUMP_VELOCITY: float = 600.0

var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity")
@export var rotation_speed: int = 30
@export var projectile_speed: float = 300.0

@onready var projectile_spawn_node: Node2D = $AnimatedSprite2D/Arm/ProjectileSpawnPoint
@onready var hitbox: Node2D = $Hitbox
@onready var health_pool: Health = $Health
@onready var jump_timer: Timer = $JumpTimer
@onready var gun_timer: Timer = $GunTimer

func _ready() -> void:
	for node: Node in [projectile_spawn_node, hitbox, health_pool, jump_timer, gun_timer]:
		assert(is_instance_valid(node))
	
	add_to_group(Groups.PLAYER)
	hitbox.body_entered.connect(on_hitbox_body_entered)
	health_pool.death.connect(on_health_pool_death)

func player_rotate(delta: float) -> void:
	var target_rotation: float = 0
	if is_on_floor():
		target_rotation = get_floor_normal().x
		
	rotation = lerp(rotation, target_rotation, delta * rotation_speed)
	
	
func is_jump_eligible() -> bool:
	return is_on_floor() and jump_timer.time_left == 0
	
func player_movement(delta: float) -> void:
	velocity.x = 0
	
	if !is_on_floor():
		velocity.y += gravity * delta

	if Input.is_action_pressed("player_jump") and is_jump_eligible():
		velocity.y -= JUMP_VELOCITY
		jump_timer.start()
		
	if Input.is_action_pressed("player_left"):
		velocity.x -= MOVE_SPEED
	if Input.is_action_prissed("player_right"):
		velocity.x += MOVE_SPEED
		
	move_and_slide()
	player_rotate(delta)

func player_shoot() -> void:
	if Input.is_action_just_pressed("fire") and gun_timer.time_left == 0:
		gun_timer.start()
		var spawn_position: Vector2 = projectile_spawn_node.global_position
		var projectile_direction: Vector2 = (get_global_mouse_position() - spawn_position).normalized()
		var projectile: Projectile = Projectile.Create(projectile_spawn_node.global_position, projectile_direction * projectile_speed)
		get_tree().root.add_child(projectile)
		projectile.add_to_group(Groups.PLAYER_PROJECTILE)

func _physics_process(delta: float) -> void:
	player_movement(delta)
	player_shoot()

func on_hitbox_body_entered(body: Node2D) -> void:
	if body.is_in_group(Groups.ENEMY):
		print("player hit")
		health_pool.change_health(-1)

func on_health_pool_death() -> void:
	print("You died!")
	queue_free()
