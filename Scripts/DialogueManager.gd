extends Node

##Notes:
##The format for line info (the dictionaries iside the dialogue arrays) is as follows:
##{
##"Speaker":"(The name of the NPC as it is with their object! this is case sensitive)",
##"Text": "(Anything)",
##"Text_incomplete": null (Replace with anything if there is a milestone requirement)
##},
##Use the above format to avoid Errors from case sensitivity

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
		"Text":"Nice! You're really showing your value around here!",
		"Text_incomplete":"You have to chop some wood."
		},
		{
		"Speaker":"Luna",
		"Text":"Why not go find a blossom to get yourself accustomed to the environment?",
		"Text_incomplete": null
		},
		{
		"Speaker":"Luna",
		"Text":"Perfect! You're ready to help those in need.",
		"Text_incomplete":"Go find a blossom!"
		},
		{
		"Speaker":"Luna",
		"Text":"Still, you could use some more help",
		"Text_incomplete": null
		},
		{
		"Speaker":"Luna",
		"Text":"Try using your blossoms to do a specific task!",
		"Text_incomplete": null
		},
		{
		"Speaker":"Luna",
		"Text":"Amazing! You now can explore much farther and have many more oppurtunities unlocked!",
		"Text_incomplete": "You should go use one of your blossom's specialties and see what they do."
		},
		{
		"Speaker":"Luna",
		"Text":"See you later!",
		"Text_incomplete": null
		},
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
	var Milestone_Progress = Dialogue_Progress(Conversation_Milestones)
	startconversation(Character_index, Milestone_Progress, Dialogue_Starting_Point)

func startconversation(Character_Index:int, Milestone_Progress:int, Current_Progress: Dictionary):
	var Dialoguekey = currentconversation[Character_Index]
	var is_at_current_point = false
	var is_waiting_for_milestone = false
	#Couldn't make a descriptive name, Line data is each dictionary for a line in a dialogue array
	for line_data in conversations[Dialoguekey]:
		var line_data_index = conversations[Dialoguekey].find(line_data)
		var most_recent_milestone_index = conversations[Dialoguekey].find(conversation_current_progress_list[Character_Index])
		if line_data_index > most_recent_milestone_index and is_waiting_for_milestone:
			pass
		else:
			if line_data_index >= most_recent_milestone_index:
				is_at_current_point = true
				print(line_data_index, "D1")
			if line_data["Text_incomplete"] == null:
				if is_at_current_point:
					print(line_data["Text"])
					print(line_data_index, "D2")
				else:
					print(line_data_index, "D3")
			
			if line_data["Text_incomplete"] != null:
				#the issue is that milestone progress resets when you re-enter
				#and then it displays from the previous line.
				if line_data == conversation_current_progress_list[Character_Index]:
					is_at_current_point = true
					print("Debug Line current point")
					print(line_data_index, "D4")
					print(most_recent_milestone_index,"D5")
				
				else:
					if line_data_index >= most_recent_milestone_index:
						print("Debug Current Point reset")
						is_waiting_for_milestone = true
						print(line_data_index, "D6")
						print(most_recent_milestone_index,"D7")
						endconversation(Character_Index,line_data)
				
				if Milestone_Progress > 0:
					print("Debug2")
					Milestone_Progress -= 1
					
					if is_at_current_point:
						print("Debug3")
						print(line_data["Text"])
						print(line_data_index, "D8")
						is_waiting_for_milestone = false
					
					else:
						print(line_data_index, "D9")
						if line_data_index >= most_recent_milestone_index:
							is_waiting_for_milestone = true
							endconversation(Character_Index,line_data)
				
				else:
					print(line_data_index, "D10")
					if is_at_current_point:
						print(line_data["Text_incomplete"])
					if line_data_index >= most_recent_milestone_index:
						endconversation(Character_Index,line_data)
						is_waiting_for_milestone = true
						
	
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
