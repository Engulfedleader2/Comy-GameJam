extends Node2D
signal PlayerInfoSendSignal

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	var playerpath = get_path_to($"../Player")
	emit_signal("PlayerInfoSendSignal")


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass
