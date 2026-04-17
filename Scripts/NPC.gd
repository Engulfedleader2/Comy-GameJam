extends Area2D

#change fulltext to export if needing to revert
var Fulltext: String
@export var Spriteresource: SpriteFrames
@export var MaximumTextHeight: float
@export var DefaultConversation: String
@export var Conversations: Array
@export var Requirements_Completion: Array[bool]

var inrange:= false

signal Dialogue_request
# Called when the node enters the scene tree for the first time.



func _ready() -> void:
	Spritesetup()
	$Control.hide()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	#if inrange:
		#$Control.position = $"../../Player".position - Vector2(800,500)
	pass



func _on_body_entered(body: Node2D) -> void:
	var playernode = get_path_to($"../../Player")
	if body == get_node(playernode):
		inrange = true
		$Control.show()
		


func _on_body_exited(body: Node2D) -> void:
	$Control.hide()
	inrange = false
	$".."._on_area_2d_end_conversation()




func Spritesetup():
	$AnimatedSprite2D.sprite_frames = Spriteresource
	$AnimatedSprite2D.play("idle")
func _input(event: InputEvent) -> void:
	if Input.is_action_just_pressed("Interact") and inrange:
		print("debug1")
		get_parent().NPC_dialogue_request($".".name, Conversations, Requirements_Completion, DefaultConversation)
		$Control.hide()
		inrange = false
