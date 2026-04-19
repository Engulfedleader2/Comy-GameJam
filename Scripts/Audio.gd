extends Node2D


# Called when the node enters the scene tree for the first time.

func _ready() -> void:
	Wwise.register_game_obj(self, self.name)
	Wwise.register_listener(self)
	
	# Load bank 
	## Main bank is the only utilized bank as of now
	Wwise.load_bank_id(AK.BANKS.MAINBANK)
	
	# Select music to play during the main menu
	Wwise.post_event_id(AK.EVENTS.PLAY_MENUMUSIC, self)
	
	# Music play state
	Wwise.set_state_id(AK.STATES.MAINMENUSTATE.GROUP, AK.STATES.MAINMENUSTATE.STATE.MUSICON)






# Called every frame. 'delta' is the elapsed time since the previous frame.

	
# Button Connections
func _on_start_button_pressed() -> void:
	Wwise.set_state_id(AK.STATES.MAINMENUSTATE.GROUP, AK.STATES.MAINMENUSTATE.STATE.NONE)
