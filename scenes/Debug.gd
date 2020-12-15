extends Label

export var target_path: NodePath setget path_set
var object

func _process(delta):
	if(Input.is_action_just_pressed("Console")):
		self.visible = !visible
	if(object != null):
		self.text = str("posX: ", object.position.x, 
					"\nposY: ", object.position.y,
					"\nvelX: ", object.velocity.x,
					"\nvelY: ", object.velocity.y)
	else:
		object = get_node(target_path)


func path_set(value: NodePath):
	target_path = value
	

