extends Node2D

var childrenraw: Array
var childrenlist:= []

signal Textaccess
signal endconversation

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	childrenraw = get_children()
	for child in childrenraw:
		print(child)
		childrenlist.append(child.name)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
func startconversation(Conversation_name:String):
	pass

func _on_area_2d_body_entered(body: Node2D) -> void:
	pass


func NPC_dialogue_request(Childname: String, Conversation_List: Array, Conversation_Milestones: Array, Default_Conversation: String) -> void:
	print("Debug2")
	emit_signal("Textaccess",Childname, Conversation_List, Conversation_Milestones, Default_Conversation)





func _on_area_2d_end_conversation() -> void:
	print('abx')
	emit_signal("endconversation")
