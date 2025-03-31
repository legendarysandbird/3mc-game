extends CharacterBody2D

const MOVE_SPEED: float = 200.0
const JUMP_VELOCITY: float = 400.0

var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity")

func _ready() -> void:
	pass

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
