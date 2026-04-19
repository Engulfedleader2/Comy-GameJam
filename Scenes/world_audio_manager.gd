extends Node2D

func _ready() -> void:
	Wwise.register_game_obj(self, self.name)
	Wwise.register_listener(self)
	
	# Load bank 
	## Main bank is the only utilized bank as of now
	Wwise.load_bank_id(AK.BANKS.MAINBANK)
	
	# Select music to play during the main menu
	Wwise.post_event_id(AK.EVENTS.PLAY_TOWNMUSIC, self)
	
	# Music play state
	Wwise.set_state_id(AK.STATES.WORLDSTATE.GROUP, AK.STATES.WORLDSTATE.STATE.MUSICON)

	
# Footstep Manager
