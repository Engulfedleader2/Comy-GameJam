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
	$Control/Label.size.y = 42
	$Control/Label.position.y = 858
	$Control/Label.position.x = 296
	$Control.hide()
	if $Control/Label.size.y > MaximumTextHeight:
		if $Control/Label.size.x == 1008:
			$Control/Label.size.x = 1336
			$Control/Label.position.x = 132
			MediumLabelResizeCheck()
			
		if $Control/Label.size.x == 1336: 
			MediumLabelResizeCheck()
		if $Control/Label.size.x == 1596:
			print("too big")
			#working on text sections
		

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	$".".position.x -= 12
	if inrange:
		$Control.position = $"../../Player".position


func _on_np_cs_player_info_send_signal() -> void:
	pass


func _on_body_entered(body: Node2D) -> void:
	var playernode = get_path_to($"../../Player")
	if body == get_node(playernode):
		$Control.show()
		print("entered")
		inrange = true


func _on_body_exited(body: Node2D) -> void:
	$Control.hide()
	print("exited")
	inrange = false
