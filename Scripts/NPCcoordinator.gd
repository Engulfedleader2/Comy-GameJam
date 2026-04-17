extends Node2D

var childrenraw: Array
var childrenlist:= []

signal Textaccess

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	childrenraw = get_children()
	for child in childrenraw:
		childrenlist.append(child.name)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
func startconversation(Conversation_name:String):
	pass

func _on_area_2d_body_entered(body: Node2D) -> void:
	pass


func _on_area_2d_dialogue_request(Childname: String, Conversation_List: Array, Conversation_Milestones: Array, Default_Conversation: String) -> void:
	emit_signal("Textaccess",Childname, Conversation_List, Conversation_Milestones, Default_Conversation)
