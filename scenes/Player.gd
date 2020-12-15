extends Actor

# editable properties
export var top_speed: float = 200.0 # in pixels
export var jump_height: float = 200.0 # in pixels
export var jump_max_buffer_time: float = 0.1 # in seconds
export var coyote_max_time: float = 0.1 # in seconds

# internal constants
var FLOOR_NORMAL = Vector2.UP
var FLOOR_DETECT_DISTANCE: float = 64.0 # in pixels

# private vars
var input_vector = Vector2.ZERO
var snap_vector = Vector2(0, FLOOR_DETECT_DISTANCE)
var jump_initial_velocity: float = (2 * jump_height) / sqrt(2 * jump_height / gravity)
var jump_timer: float = 0
var coyote_timer: float = 0
# state vars
####### TODO: Implement State Machine #######
var is_jumping: bool = false
var is_falling: bool = false
var is_running: bool = false
var jump_pressed = false
var facing_direction = 1 setget set_facing_direction # 1 right, -1 left
# child vars
onready var sprite = $Sprite
onready var floor_detector = $FloorDetector
onready var wall_detector_east = $WallDetectorEast
onready var wall_detector_west = $WallDetectorWest
onready var hitbox = $Hitbox

# debug vars here


# non-vector math updates here (e.g graphics)
# tied to user fps
func _process(_delta):
	handle_input()

# for updates that require a fixed timestep
# not tied to user fps
func _physics_process(_delta):
	apply_gravity()	
	calculate_jump()
	apply_movement(input_vector)
	update_state()

func handle_input():
	
	input_vector = Vector2(
		Input.get_action_strength("ui_right") - Input.get_action_strength("ui_left"), #x right = 1, left -1
		Input.get_action_strength("ui_up") - Input.get_action_strength("ui_down") #y up = 1, down -1
		)
		
	if Input.is_action_just_pressed("ui_left"):
		self.transform.x = Transform2D.FLIP_X.x
	if Input.is_action_just_pressed("ui_right"):
		self.transform.x = Transform2D.IDENTITY.x

	if Input.is_action_just_pressed("ui_select"):
		jump_timer = 0
		jump_pressed = true
		snap_vector = Vector2.ZERO
		
	if Input.is_action_just_pressed("Console"):
		hitbox.visible = not hitbox.visible

func apply_gravity():
	#if not is_on_floor():
		velocity.y += gravity * get_physics_process_delta_time()
	#else:
	#	velocity.y = 0
	

func apply_movement(move_vector: Vector2):
	##### currently breaks movement on slopes #####
	# lerp simulates smooth start and stop
	#velocity.x = lerp(velocity.x, move_vector.x * top_speed, 0.34)
	##### currently breaks movement on slopes #####
	velocity.x = move_vector.x * top_speed
	velocity = move_and_slide_with_snap(velocity, snap_vector, FLOOR_NORMAL, true)
	
	#velocity.x = 0 if input_vector.x == 0 else lerp(velocity.x, move_vector.x * top_speed, 0.34)

func update_state():
	is_falling = velocity.y > 0
	if floor_detector.is_colliding(): 
		is_jumping = false
		snap_vector.y = FLOOR_DETECT_DISTANCE


func jump():
	velocity.y = -jump_initial_velocity
	is_jumping = true
	jump_pressed = false


func calculate_jump():
	# checks if we hit the floor within the jump_max_buffer_time of
	# pressing the jump button, then jumps at the first possible frame
	if jump_timer < jump_max_buffer_time:
		jump_timer += get_physics_process_delta_time()
		if is_on_floor():
			jump()

	# lets the player jump within coyote_max_time seconds after falling a ledge
	# checks if we're falling because we fell off something, not because we jumped
	if is_falling and not is_on_floor() and not is_jumping:
		coyote_timer += get_physics_process_delta_time()
		if coyote_timer < coyote_max_time and jump_pressed:
			jump()
	else:
		coyote_timer = 0


func set_facing_direction(direction: int):
	self.transform.x.x = direction
	facing_direction = direction




