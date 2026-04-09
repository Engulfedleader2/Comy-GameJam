extends Area2D

#change fulltext to export if needing to revert
var Fulltext: String
@export var Spriteresource: SpriteFrames
@export var MaximumTextHeight: float
@export var Margins: float
@export var DefaultConversation: String
@export var Conversations: Array
@export var Requirements_Completion: Array[bool]


var halfmargin
var inrange:= false

signal Dialogue_request
# Called when the node enters the scene tree for the first time.



func _ready() -> void:
	print(Conversations)
	print()
	Spritesetup()
	SetupReadText()
	#
	#print("Detected.label position setup")
	##Makes the script wait 1 Frame
	#await get_tree().process_frame
	#
	#TextWidthManager()
	#print("Detected.label wrap has been set")
	#
	#await get_tree().process_frame
	#
	#Textheightandtrimming()
	#Labelfinishing()

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	#if inrange:
		#$Control.position = $"../../Player".position - Vector2(800,500)
	pass



func _on_body_entered(body: Node2D) -> void:
	var playernode = get_path_to($"../../Player")
	if body == get_node(playernode):
		Marginssetup()
		inrange = true

		emit_signal("Dialogue_request", $".".name, Conversations, Requirements_Completion, DefaultConversation)


func _on_body_exited(body: Node2D) -> void:
	$Control.hide()
	inrange = false



func Spritesetup():
	$AnimatedSprite2D.sprite_frames = Spriteresource
	$AnimatedSprite2D.play("idle")

func SetupReadText():
	$Control.visible = false
	$Control/Label.text_overrun_behavior = TextServer.OVERRUN_NO_TRIM
	$Control/Label.autowrap_mode = TextServer.AUTOWRAP_OFF
	$Control/Label.size.y = 28.0
	$Control/Label.size.x = 2.0
	$Control/Label.position.x = 799.0
	#Removed the set to fulltext from here for testing, if errors later set contents to fulltext

func TextWidthManager():if $Control/Label.size.x >= 1008.0:
	if $Control/Label.size.x >= 1008.0:
		#somewhat inneficient, return later
		$Control/Label.size.x = 1008.0
		$Control/Label.set_anchor(SIDE_BOTTOM, 296.0)
		$Control/Label.autowrap_mode = TextServer.AUTOWRAP_WORD
		$Control/Label.size.x = 1008.0

func Textheightandtrimming():
	if $Control/Label.size.y >= MaximumTextHeight:
		$Control/Label.size.y = MaximumTextHeight
		$Control/Label.clip_text = true
		print("Detected")
		
func Labelfinishing():
	$Control/Label.position.y = 858.0 - $Control/Label.size.y
	$Control.visible = true
	$Control.hide()
func Marginssetup():
	var marginoffset = Margins/2
	$Control/Panel.position.x = $Control/Label.position.x - marginoffset
	$Control/Panel.size.x = $Control/Label.size.x+Margins
	$Control/Panel.position.y = $Control/Label.position.y - marginoffset
	$Control/Panel.size.y = $Control/Label.size.y + Margins
	$Control.show()
