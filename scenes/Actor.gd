extends KinematicBody2D

class_name Actor
## Affected by gravity
export var gravity = 1960.0


##private vars
var velocity = Vector2.ZERO


func _physics_process(delta):
	velocity.y += gravity * delta
