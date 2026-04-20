extends Node2D

# Preload battle background sprite. Un-comment and load image when we get one. 
var battle_sprite_scene =preload("res://Battle.tscn")


func _ready() -> void:
	battle_sprite_setup($"Start Positions/Player/Player Start", $"Battle Sprites/Player Character Battle", true) #player
	battle_sprite_setup($"Start Positions/AI Start", $"Battle Sprites/AI Character Battle", false) #enemy

func battle_sprite_setup(start_positions: Node2D, parent: Node2D, is_player: bool):
	for start_pos_marker: Marker2D in start_positions.get_children():
		var pos = start_pos_marker.position
		create_battle_sprite(pos, parent, is_player)

#Uncomment when we get a battle BG
func create_battle_sprite(pos: Vector2, parent: Node2D, is_player: bool):
	var battle_sprite = battle_sprite_scene.instantiate()
	battle_sprite.setup(pos, is_player)
	parent.add_child(battle_sprite)
