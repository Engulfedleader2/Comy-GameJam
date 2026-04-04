extends AnimatableBody2D
# I know speed should be a const, but if we don't add anything to change it in the code it's fine.
#And I want to make it a quick edit from editor using @export
@export var speed = 180
var Direction: Vector2
#These values are just the default and do change when movement input is given
var MoveInputPriority1 = "Down"
var MoveInputPriority2 = "Up"
var MoveInputPriority3 = "Left"
var MoveInputPriority4 = "Right"
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func prioritychecker():
	if MoveInputPriority1 == "Left":
		Direction.x = -1
	if MoveInputPriority1 == "Right":
		Direction.x = 1
	if MoveInputPriority1 == "Down":
		Direction.y = 1
	if MoveInputPriority1 == "Up":
		Direction.y = -1

func _physics_process(delta: float) -> void:
	Direction.x = 0
	Direction.y = 0
	if Input.is_action_pressed(MoveInputPriority1):
		prioritychecker()
	elif Input.is_action_pressed(MoveInputPriority2):
		var holdingvariable = MoveInputPriority1
		MoveInputPriority1 = MoveInputPriority2
		MoveInputPriority2 = MoveInputPriority3
		MoveInputPriority3 = MoveInputPriority4
		MoveInputPriority4 = holdingvariable
		
		prioritychecker()
	elif Input.is_action_pressed(MoveInputPriority3):
		var holdingvariable = MoveInputPriority1
		MoveInputPriority1 = MoveInputPriority3
		MoveInputPriority3 = MoveInputPriority4
		MoveInputPriority4 = holdingvariable
		
		prioritychecker()
	elif Input.is_action_pressed(MoveInputPriority4):	
		var holdingvariable = MoveInputPriority1
		MoveInputPriority1 = MoveInputPriority4
		MoveInputPriority4 = holdingvariable
		
		prioritychecker()
	$".".position += speed * delta * Direction
