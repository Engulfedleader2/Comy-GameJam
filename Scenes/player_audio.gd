extends Node2D
@onready var sprite_2d: AnimatedSprite2D = $"../AnimatedSprite2D"
@onready var ground_layers = [
	get_node("../../Ground/MapPath"),
	get_node("../../Ground/MapGround")
]

# Called when the node enters the scene tree for the first time.

func _ready() -> void:
	Wwise.register_game_obj(self, self.name)
	Wwise.register_listener(self)
	
## Footstep activated frames
var footstep_frames = [2, 5]

# Find material underneath player
func get_surface_under_player() -> String:
	var foot_pos = global_position + Vector2(0, 10)
	for layer in ground_layers:
		if not is_instance_valid(layer): continue
		
		# Convert global feet position to map coordinates for this specific layer
		var local_pos = layer.to_local(foot_pos)
		var tile_pos = layer.local_to_map(local_pos)
		var tile_data = layer.get_cell_tile_data(tile_pos)
		
		if tile_data:
			var mat = tile_data.get_custom_data("material")
			if mat != "":
				print("Detected Material: ", mat, " on ", layer.name)
				return mat
	
	# If no tile is found on any layer, default to Stone
	return "Stone"

# Play footstep
func _on_animated_sprite_2d_frame_changed() -> void:
	if sprite_2d.animation == "walk_left" or sprite_2d.animation == "walk_right":
		if sprite_2d.frame in footstep_frames:
			var surface_name = get_surface_under_player()
			print("Sending to Wwise: ", surface_name)
			Wwise.set_switch("FootStepsPC", surface_name, self)
			Wwise.post_event_id(AK.EVENTS.PLAY_PCSTEPS, self)
 
