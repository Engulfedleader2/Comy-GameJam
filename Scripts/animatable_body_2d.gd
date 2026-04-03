extends AnimatableBody2D

var goingdown := false
var goingup := false
var goingleft := false
var goingright := false
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	#if diagonal movement is implemented make it 20.5 for both axes instead of 29 for both axes
	if goingdown:
		$".".position += Vector2(0,29)
	if goingup:
		$".".position -= Vector2(0,29)
	if goingleft:
		$".".position -= Vector2(29,0)
	if goingright:
		$".".position += Vector2(29,0)
func _input(event: InputEvent) -> void:
	if Input.is_action_just_released("Down"):
		goingdown = false
	if Input.is_action_just_released("Up"):
		goingup = false
	if Input.is_action_just_released("Right"):
		goingright = false
	if Input.is_action_just_released("Left"):
		goingleft = false
	if not goingup or goingdown or goingleft or goingright:
		if Input.is_action_just_pressed("Up"):
			goingup = true
		if Input.is_action_just_pressed("Down"):
			goingdown = true
		if Input.is_action_just_pressed("Left"):
			goingleft = true
		if Input.is_action_just_pressed("Right"):
			goingright = true
	
