extends AnimatableBody2D
# I know speed should be a const, but if we don't add anything to change it in the code it's fine.
#And I want to make it a quick edit from editor using @export
@export var speed = 180
var Direction: Vector2
var Priority1 = "Down"
var Priority2 = "Up"
var Priority3 = "Left"
var Priority4 = "Right"
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _physics_process(delta: float) -> void:
	Direction.x = 0
	Direction.y = 0
	if Input.is_action_pressed(Priority1):
		if Priority1 == "Left":
			Direction.x = -1
		if Priority1 == "Right":
			Direction.x = 1
		if Priority1 == "Down":
			Direction.y = 1
		if Priority1 == "Up":
			Direction.y = -1
	elif Input.is_action_pressed(Priority2):
		Direction.y = -1
		var holdingvariable = Priority1
		Priority1 = Priority2
		Priority2 = Priority3
		Priority3 = Priority4
		Priority4 = holdingvariable
		
		if Priority1 == "Left":
			Direction.x = -1
		if Priority1 == "Right":
			Direction.x = 1
		if Priority1 == "Down":
			Direction.y = 1
		if Priority1 == "Up":
			Direction.y = -1
	elif Input.is_action_pressed(Priority3):
		Direction.x = -1
		var holdingvariable = Priority1
		Priority1 = Priority3
		Priority3 = Priority4
		Priority4 = holdingvariable
		
		if Priority1 == "Left":
			Direction.x = -1
		if Priority1 == "Right":
			Direction.x = 1
		if Priority1 == "Down":
			Direction.y = 1
		if Priority1 == "Up":
			Direction.y = -1
	elif Input.is_action_pressed(Priority4):
		Direction.x = 1
		var holdingvariable = Priority1
		Priority1 = Priority4
		Priority4 = holdingvariable
		
		if Priority1 == "Left":
			Direction.x = -1
		if Priority1 == "Right":
			Direction.x = 1
		if Priority1 == "Down":
			Direction.y = 1
		if Priority1 == "Up":
			Direction.y = -1
	$".".position += speed * delta * Direction
