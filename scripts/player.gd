class_name Player extends CharacterBody2D

const MOVE_SPEED: float = 200.0
const JUMP_VELOCITY: float = 600.0

var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity")
@export var rotation_speed: int = 30

func _ready() -> void:
	add_to_group(Groups.PLAYER)
	$CollisionShape2D

func player_rotate(delta: float) -> void:
	var target_rotation: float = 0
	if is_on_floor():
		target_rotation = get_floor_normal().x
		
	rotation = lerp(rotation, target_rotation, delta * rotation_speed)
	
	
func is_jump_eligible() -> bool:
	return is_on_floor() and $JumpTimer.time_left == 0
	

func _physics_process(delta: float) -> void:
	velocity.x = 0
	
	if !is_on_floor():
		velocity.y += gravity * delta
		
	if Input.is_action_pressed("player_jump") and is_jump_eligible():
		velocity.y -= JUMP_VELOCITY
		$JumpTimer.start()
		
	if Input.is_action_pressed("player_left"):
		velocity.x -= MOVE_SPEED
	if Input.is_action_pressed("player_right"):
		velocity.x += MOVE_SPEED
	
	move_and_slide()
	player_rotate(delta)
