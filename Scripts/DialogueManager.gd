extends Node

var isactive = false
#list of where each npc is in progress of a given dialogue
var conversation_current_progress_list := []
#activenpcs will serve as an index for current conversation
#get the index of an npc in activenpcs and use that same index in currentconversartion to get you their current conversations
var currentconversation := []
var activenpcs = []
var currentindex = 0
#conversation names are Case sensitive! 
#They have to be the same here as when added to an NPC's conversations!
var conversations = {
	"Intro":[
		{
		"Speaker":"Luna",
		"Text": "Hello!",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Luna",
		"Text":"To get accustomed, chop some wood!",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Luna",
		"Text":"Nice! You're ready to explore now, good luck!",
		"Text_incomplete":"You have to chop some wood."
			
		}
	],
	"Forest Meeting":[
		{"Speaker":"John",
		"Text":"Hello, haven't seen you before!",
		},
		{"Speaker":"John",
		"Text":"Hello",
		#Milestone:True does not mean the milestone is completed, rather there is a milestone required there
		"Milestone":"True"
		},
		{"Speaker":"James",
		"Text":"How Are you?"
		}
	]
}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass



#When you interact with any given npc for dialogue
func _on_np_cs_textaccess(Charactername: String, Conversation_List: Array, Conversation_Milestones: Array, Default_Conversation: String) -> void:
	var Dialogue_Starting_Point
	var Character_index
	if Charactername not in activenpcs:
		activenpcs.append(Charactername)
		Dialogue_Starting_Point = conversations[Default_Conversation][0]
		currentconversation.append(Default_Conversation)
		conversation_current_progress_list.append(Dialogue_Starting_Point)
	elif Charactername in activenpcs:
		Character_index = activenpcs.find(Charactername)
		Dialogue_Starting_Point = conversation_current_progress_list[Character_index]
	Character_index = activenpcs.find(Charactername)
	startconversation(Character_index, Conversation_Milestones, Dialogue_Starting_Point)

func startconversation(Character_Index:int, Milestone_Completion:Array, Current_Progress: Dictionary):
	var Dialoguekey = currentconversation[Character_Index]
	var Milestone_Progress = Dialogue_Progress(Milestone_Completion)
	var is_at_current_point = false
	#Couldn't make a descriptive name, Line data is each dictionary for a line in a dialogue array
	for line_data in conversations[Dialoguekey]:
		if line_data == conversation_current_progress_list[Character_Index]:
			is_at_current_point = true
		if line_data["Text_incomplete"] == null:
			if is_at_current_point:
				print(line_data["Text"])
		
		if line_data["Text_incomplete"] != null:
			if Milestone_Progress > 0:
				Milestone_Progress -= 1
				if conversation_current_progress_list[Character_Index] == line_data:
					if is_at_current_point:
						print(line_data["Text"])
				else:
					endconversation(Character_Index,line_data)
			else:
				if is_at_current_point:
					print(line_data["Text_incomplete"])
				endconversation(Character_Index, line_data)
				
	
func Dialogue_Progress(Completion_List: Array):
	#Once we finish the tasks system this will be updated so you can move a conversation only when a task is complete
	#Right now its just checking Milestone bools from the NPC
	var Operating = true
	var Milestone_Complete_Count = 0
	for Milestone in Completion_List:
		if Operating:
			if Milestone:
				Milestone_Complete_Count+=1
			if not Milestone:
				Operating =false
	return Milestone_Complete_Count

func getdialogue():
	pass
	
func getspeaker(Speaker: String,):
	pass
func endconversation(Character_Index: int, Current_Line):
	conversation_current_progress_list[Character_Index] = Current_Line


func getnextline()->Dictionary:
	currentindex += 1
	return {}
func getcurrentline():
	pass
		
	
func getpreviousline() -> Dictionary:
	currentindex-=1
	return {}
func getlinecount():
	currentconversation.size()
	
func getcharactercount(Conversation:String):
	var line = getcurrentline()
	if line.is_empty():
		print("Debug".length())
		return 0
	else:
		return line["Text".length()]

	
