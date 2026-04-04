extends AnimatableBody2D
# I know speed should be a const, but if we don't add anything to change it in the code it's fine.
#And I want to make it a quick edit from editor using @export
@export var speed = 180
var Direction: Vector2
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

func _physics_process(delta: float) -> void:
	Direction.x = 0
	Direction.y = 0
	if Input.is_action_pressed("Down"):
		Direction.y = 1
	elif Input.is_action_pressed("Up"):
		Direction.y = -1
	elif Input.is_action_pressed("Left"):
		Direction.x = -1
	elif Input.is_action_pressed("Right"):
		Direction.x = 1
	$".".position += speed * delta * Direction
