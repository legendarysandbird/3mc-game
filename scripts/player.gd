extends CharacterBody2D

const MOVE_SPEED: float = 200.0
const JUMP_VELOCITY: float = 400.0
# Using project gravity for easy syncing
var gravity: float = ProjectSettings.get_setting("physics/2d/default_gravity")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	$AnimatedSprite2D.set_animation("still")

# Basic player walking animation logic
func animate() -> void:
	if !Input.is_anything_pressed():
		$AnimatedSprite2D.set_animation("still")
		
	if Input.is_action_pressed("player_left") or Input.is_action_pressed("player_right"):
		$AnimatedSprite2D.set_animation("walk")
		
	# If not moving left/right, be still. Elif on the floor, set looking direction appropriately. If in the air and not moving, don't play.
	if velocity.x == 0:
		$AnimatedSprite2D.set_animation("still")
	elif is_on_floor():
		if velocity.x > 0:
			$AnimatedSprite2D.flip_h = false
		elif velocity.x < 0:
			$AnimatedSprite2D.flip_h = true
		$AnimatedSprite2D.play()
	else:
		$AnimatedSprite2D.stop()


# Crude checking if the player is eligibile to jump
func is_jump_eligible() -> bool:
	return is_on_floor() and $JumpTimer.time_left == 0

# Called 60 times a second. 'delta' is the elapsed time since the previous frame.
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
		
	animate()
	move_and_slide()
