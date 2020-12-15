extends Node2D

onready var debug = $debug

func _ready():
	pass # Replace with function body.

func _on_Hitbox_area_entered(area):
	debug.text = "AREA HIT"
	pass # Replace with function body.



func _on_Hitbox_body_entered(body):
	debug.text = "BODY HIT"
	pass # Replace with function body.
