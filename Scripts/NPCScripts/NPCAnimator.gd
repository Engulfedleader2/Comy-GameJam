extends Node2D

@export var default_facing: String = "down"
@export var start_idle_on_ready: bool = true
@export var use_left_right_flip: bool = true
@export var sprite_path: NodePath = NodePath("CharacterBody2D/AnimatedSprite2D")

@onready var sprite: AnimatedSprite2D = find_sprite()

var current_facing: String = "down"

func _ready() -> void:
	current_facing = default_facing

	if sprite == null:
		push_warning("NPCAnimator could not find AnimatedSprite2D. Check sprite_path on this NPC.")
		return

	if start_idle_on_ready:
		play_idle()

func find_sprite() -> AnimatedSprite2D:
	if sprite_path != NodePath():
		var configured_sprite := get_node_or_null(sprite_path) as AnimatedSprite2D
		if configured_sprite != null:
			return configured_sprite

	var direct_sprite := get_node_or_null("AnimatedSprite2D") as AnimatedSprite2D
	if direct_sprite != null:
		return direct_sprite

	var body_sprite := get_node_or_null("CharacterBody2D/AnimatedSprite2D") as AnimatedSprite2D
	if body_sprite != null:
		return body_sprite

	var recursive_sprite := find_first_sprite(self)
	if recursive_sprite != null:
		return recursive_sprite

	var parent := get_parent()
	if parent != null:
		return find_first_sprite(parent)

	return null

func find_first_sprite(node: Node) -> AnimatedSprite2D:
	if node is AnimatedSprite2D:
		return node as AnimatedSprite2D

	for child in node.get_children():
		var found := find_first_sprite(child)
		if found != null:
			return found

	return null

func play_idle() -> void:
	if sprite == null:
		return

	if has_animation("idle"):
		sprite.play("idle")

func has_animation(animation_name: String) -> bool:
	return sprite != null and sprite.sprite_frames != null and sprite.sprite_frames.has_animation(animation_name)
