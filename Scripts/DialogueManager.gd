extends Node

signal dialogue_started(npc_name: String)
signal dialogue_ended(npc_name: String)

const DIALOGUE_UI_SCENE := preload("res://Scenes/DialogueScenes/DialogueUI.tscn")

var dialogue_ui: CanvasLayer = null
var speaker_name_label: Label = null
var dialogue_text_label: RichTextLabel = null
var portrait_rect: TextureRect = null

## Dialogue line format:
## {
##   "Speaker": "Luna",
##   "Text": "Hello!",
##   "Text_incomplete": null
## }
##
## Notes:
## - Conversation names are case sensitive.
## - NPC names are case sensitive.
## - This script is intended to be used as an Autoload.

var isactive: bool = false

# Keeps track of each NPC's current conversation and line progress.
# Example:
# {
#   "Luna": { "conversation": "Intro", "line_index": 0 }
# }
var npc_progress: Dictionary = {}

var active_npc_name: String = ""
var active_conversation_key: String = ""
var active_line_index: int = 0
var active_requirements: Array[bool] = []

# Conversation names are case sensitive.
# They must match the NPC DefaultConversation value.
var conversations = {
	"Intro":[
		{
		"Speaker":"Esmerelda",
		"Text":"I wanted to talk to you this year about something important.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"I... alright, what do you have for me, Esme?",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"I can't this year, not yet, but I found a tome of your grandmothers that talks about-",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"Please not this again.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"Just listen to me. It talked about staying awake past the spring while still recovering!",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"It was in the spirit-tongue so I don't have it fully decoded.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"I know that it will be decoded by next year, probably before winter.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"Promise me that you'll listen to me then. I'll drop it this year if you do.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"I can't keep saying bye to you.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"Esme...",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"Okay. You are going so far for just this, for just... for a chance at us like when we were kids.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"So I promise I'll listen.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"I have to go though, I have some food to get everyone.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"Then go, we all need you to do what you do best.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"Next year, I promise.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"I believe you.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"By the way... you look good.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"You look good too.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"I hope so, I don't age nearly as fast as you do.",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"You're wanting to be with a younger woman!",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Esmerelda",
		"Text":"Shut up and get out of here!",
		"Text_incomplete": null,
		},
		{
		"Speaker":"Keya",
		"Text":"*Thinking to herself* Spirits... I can't help myself with her.",
		"Text_incomplete": null,
		}
	],
	"Forest Meeting":[
		{
		"Speaker":"John",
		"Text":"Oh hey, I didn't see you there.",
		"Text_incomplete": null
		},
		{
		"Speaker":"John",
		"Text":"The forest ahead is pretty peaceful.",
		"Text_incomplete": null
		},
		{
		"Speaker":"John",
		"Text":"Just watch out for anything strange.",
		"Text_incomplete": null
		},
		{
		"Speaker":"John",
		"Text":"Anyway, good luck out there.",
		"Text_incomplete": null
		}
	],
}

func _ready() -> void:
	print("DialogueManager ready")
	_ensure_dialogue_ui()
	_hide_dialogue_ui()


func _ensure_dialogue_ui() -> void:
	if dialogue_ui != null:
		return

	print("Instantiating Dialogue UI from:", DIALOGUE_UI_SCENE.resource_path)
	dialogue_ui = DIALOGUE_UI_SCENE.instantiate() as CanvasLayer
	dialogue_ui.name = "DialogueUI"
	get_tree().root.call_deferred("add_child", dialogue_ui)

	speaker_name_label = dialogue_ui.get_node("Root/DialoguePanel/SpeakerName") as Label
	dialogue_text_label = dialogue_ui.get_node("Root/DialoguePanel/DialogueText") as RichTextLabel
	portrait_rect = dialogue_ui.get_node("Root/DialoguePanel/Portrait") as TextureRect
	print("Dialogue UI wired")

func _show_dialogue_ui() -> void:
	_ensure_dialogue_ui()
	if dialogue_ui != null:
		print("Showing dialogue UI")
		dialogue_ui.visible = true

func _hide_dialogue_ui() -> void:
	if dialogue_ui != null:
		print("Hiding dialogue UI")
		dialogue_ui.visible = false

func _update_dialogue_ui(line_data: Dictionary, display_text: String) -> void:
	_ensure_dialogue_ui()
	if speaker_name_label != null:
		speaker_name_label.text = str(line_data.get("Speaker", ""))
	if dialogue_text_label != null:
		dialogue_text_label.text = display_text
	if portrait_rect != null:
		var speaker := str(line_data.get("Speaker", "")).strip_edges()
		var portrait: Texture2D = null

		if speaker == "Esmerelda" or speaker == "Esmeralda":
			portrait = load("res://Assets/Sprites/NPCSprites/NPCPotraits/Esmeralda/Esmeralda_Neutral.tres")
		elif speaker == "Keya":
			portrait = load("res://Assets/Sprites/NPCSprites/NPCPotraits/Keya/keya_neutral.tres")

		portrait_rect.texture = portrait
	print("Updating UI -> Speaker:", line_data.get("Speaker", ""), " Text:", display_text)


# Called by NPCs when the player starts talking to them.
# Conversation_List is kept in the signature for compatibility with the current NPC script,
# even though this manager currently uses the central `conversations` dictionary above.
func start_dialogue(character_name: String, _conversation_list: Array, conversation_milestones: Array[bool], default_conversation: String) -> void:
	print("start_dialogue called:", character_name, " conversation:", default_conversation)
	if not conversations.has(default_conversation):
		print("Missing conversation key:", default_conversation)
		push_warning("DialogueManager: missing conversation '%s'" % default_conversation)
		return

	if not npc_progress.has(character_name):
		npc_progress[character_name] = {
			"conversation": default_conversation,
			"line_index": 0,
		}

	var saved_state: Dictionary = npc_progress[character_name]
	active_npc_name = character_name
	active_conversation_key = saved_state.get("conversation", default_conversation)
	active_line_index = int(saved_state.get("line_index", 0))
	active_requirements = conversation_milestones.duplicate()
	isactive = true
	_show_dialogue_ui()
	emit_signal("dialogue_started", active_npc_name)
	_show_current_line()

# Compatibility bridge for the old signal-based setup.
func _on_np_cs_textaccess(Charactername: String, Conversation_List: Array, Conversation_Milestones: Array[bool], Default_Conversation: String) -> void:
	start_dialogue(Charactername, Conversation_List, Conversation_Milestones, Default_Conversation)

func advance_dialogue() -> void:
	if not isactive:
		return

	var dialogue_lines: Array = conversations.get(active_conversation_key, [])
	if dialogue_lines.is_empty():
		end_dialogue()
		return

	var current_line: Dictionary = dialogue_lines[active_line_index]
	var can_advance: bool = _can_complete_line(current_line, Dialogue_Progress(active_requirements))

	if not can_advance:
		end_dialogue()
		return

	active_line_index += 1
	if active_line_index >= dialogue_lines.size():
		end_dialogue()
		return

	npc_progress[active_npc_name]["line_index"] = active_line_index
	_show_current_line()

func end_dialogue() -> void:
	if active_npc_name != "":
		npc_progress[active_npc_name] = {
			"conversation": active_conversation_key,
			"line_index": active_line_index,
		}

	isactive = false
	_hide_dialogue_ui()
	emit_signal("dialogue_ended", active_npc_name)
	active_npc_name = ""
	active_conversation_key = ""
	active_line_index = 0
	active_requirements.clear()

func _show_current_line() -> void:
	var dialogue_lines: Array = conversations.get(active_conversation_key, [])
	if dialogue_lines.is_empty():
		end_dialogue()
		return

	if active_line_index < 0 or active_line_index >= dialogue_lines.size():
		end_dialogue()
		return

	var line_data: Dictionary = dialogue_lines[active_line_index]
	var milestone_progress: int = Dialogue_Progress(active_requirements)
	var display_text := _get_display_text(line_data, milestone_progress)
	_update_dialogue_ui(line_data, display_text)

func _get_display_text(line_data: Dictionary, milestone_progress: int) -> String:
	var text_incomplete = line_data.get("Text_incomplete", null)
	if text_incomplete != null and not _can_complete_line(line_data, milestone_progress):
		return str(text_incomplete)
	return str(line_data.get("Text", ""))

func _can_complete_line(line_data: Dictionary, milestone_progress: int) -> bool:
	if line_data.get("Text_incomplete", null) == null:
		return true
	return milestone_progress > active_line_index

func Dialogue_Progress(Completion_List: Array[bool]) -> int:
	# Counts how many requirements at the start of the list are completed.
	var operating := true
	var milestone_complete_count := 0
	for milestone in Completion_List:
		if operating:
			if milestone:
				milestone_complete_count += 1
			else:
				operating = false
	return milestone_complete_count

func getdialogue() -> Array:
	return conversations.get(active_conversation_key, [])

func getspeaker() -> String:
	var line := getcurrentline()
	if line.is_empty():
		return ""
	return str(line.get("Speaker", ""))

func getnextline() -> Dictionary:
	var dialogue_lines: Array = conversations.get(active_conversation_key, [])
	var next_index := active_line_index + 1
	if next_index >= 0 and next_index < dialogue_lines.size():
		return dialogue_lines[next_index]
	return {}

func getcurrentline() -> Dictionary:
	var dialogue_lines: Array = conversations.get(active_conversation_key, [])
	if active_line_index >= 0 and active_line_index < dialogue_lines.size():
		return dialogue_lines[active_line_index]
	return {}

func getpreviousline(current_line_index: int, dialogue_key: String) -> Dictionary:
	if not conversations.has(dialogue_key):
		return {}
	if current_line_index <= 0:
		return {}
	return conversations[dialogue_key][current_line_index - 1]

func getlinecount() -> int:
	return conversations.get(active_conversation_key, []).size()

func getcharactercount(_conversation: String) -> int:
	var line := getcurrentline()
	if line.is_empty():
		return 0
	return str(line.get("Text", "")).length()

func _input(_event: InputEvent) -> void:
	if Input.is_action_just_pressed("Interact") and isactive:
		advance_dialogue()

func _on_np_cs_endconversation() -> void:
	end_dialogue()
