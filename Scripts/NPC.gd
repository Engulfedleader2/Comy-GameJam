extends Area2D

@export var Fulltext: String
@export var Spriteresource: SpriteFrames
@export var MaximumTextHeight: int
var inrange:= false
# Called when the node enters the scene tree for the first time.
func MediumLabelResizeCheck():
	if $Control/Label.size.y > MaximumTextHeight:
		$Control/Label.size.x = 1596
		$Control/Label.position.y = 2
	else:
		pass
func _ready() -> void:
	$AnimatedSprite2D.sprite_frames = Spriteresource
	$AnimatedSprite2D.play("idle")
	$Control/Label.text = Fulltext
	$Control/Label.size.x = 1008
	$Control/Label.size.y = MaximumTextHeight
	$Control/Label.position.y = 858 - MaximumTextHeight
	$Control/Label.position.x = 296
	$Control/Panel.position = $Control/Label.position
	$Control/Panel.size = $Control/Label.size
	$Control.hide()
	
		

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if inrange:
		$Control.position = $"../../Player".position - Vector2(800,500)


func _on_np_cs_player_info_send_signal() -> void:
	pass


func _on_body_entered(body: Node2D) -> void:
	var playernode = get_path_to($"../../Player")
	if body == get_node(playernode):
		$Control.show()
		inrange = true


func _on_body_exited(body: Node2D) -> void:
	$Control.hide()
	inrange = false
