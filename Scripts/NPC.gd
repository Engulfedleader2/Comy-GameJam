extends Area2D

#change fulltext to export if needing to revert
var Fulltext: String
@export var Spriteresource: SpriteFrames
@export var MaximumTextHeight: float
@export var DefaultConversation: String
@export var Conversations: Array
@export var Requirements_Completion: Array[bool]

var inrange:= false

func _is_player_body(body: Node) -> bool:
	if body == null:
		return false
	if body.is_in_group("player"):
		return true
	if body.name == "Player":
		return true
	var parent := body.get_parent()
	if parent != null and parent.name == "Player":
		return true
	return false

# Called when the node enters the scene tree for the first time.



func _ready() -> void:
	Spritesetup()
	if has_node("Control"):
		$Control.hide()




func _on_body_entered(body: Node2D) -> void:
	print("entered:", body.name)
	print("entered in player group:", body.is_in_group("player"))
	if _is_player_body(body):
		inrange = true
		print("inrange set to true")
		print("player in range")
		if has_node("Control"):
			$Control.show()
		


func _on_body_exited(body: Node2D) -> void:
	print("exited:", body.name)
	print("exited in player group:", body.is_in_group("player"))
	if _is_player_body(body):
		if has_node("Control"):
			$Control.hide()
		inrange = false
		print("inrange set to false")
		DialogueManager.end_dialogue()




func Spritesetup():
	pass
func _unhandled_input(_event: InputEvent) -> void:
	if Input.is_action_just_pressed("Interact"):
		print("Interact pressed. inrange:", inrange, " dialogue active:", DialogueManager.isactive)
		if inrange and not DialogueManager.isactive:
			print("debug1")
			var npc_name := get_parent().name
			var conversation_key := DefaultConversation
			if conversation_key == "":
				conversation_key = "Intro"
			DialogueManager.start_dialogue(npc_name, Conversations, Requirements_Completion, conversation_key)
			if has_node("Control"):
				$Control.hide()
