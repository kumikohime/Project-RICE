using Godot;
using System;

public class Player : Actor
{
    // editable properties
    [Export]
    float TopSpeed = 200.0f; // in pixels
    [Export]
    float JumpHeight = 200.0f;// in pixels
    [Export]
    float JumpMaxBufferTime = 0.1f;// in sec onds
    [Export]
    float CoyoteMaxTime = 0.1f;// in seconds

    // internal constants
    Godot.Vector2 FLOOR_NORMAL = Godot.Vector2.Up;
    float FLOOR_DETECT_DISTANCE = 64.0f;
    // private vars for internal computation
    private Godot.Vector2 inputVector = Godot.Vector2.Zero;
    private Godot.Vector2 snapVector;
    private float jumpInitialVelocity;
    private float jumpTimer = 0;
    private float coyoteTimer = 0;
    // state vars
    // TODO: Implement State Machine 
    bool isJumping = false;
    bool isFalling = false;
    bool isRunning = false;
    bool jumpPressed = false;
    int facingDirection = 1; // 1 right, -1 left

    //child vars
    private Sprite sprite;
    private RayCast2D floorDetector;
    private CollisionShape2D hitbox;
    public override void _Ready()
    {
        snapVector = new Godot.Vector2(0, FLOOR_DETECT_DISTANCE);
        jumpInitialVelocity  = (2 * JumpHeight) / (float)Math.Sqrt(2 * JumpHeight / base.gravity) ;
        sprite = GetNode<Sprite>("Sprite");
        hitbox = GetNode<CollisionShape2D>("Hitbox");
        floorDetector = GetNode<RayCast2D>("FloorDetector");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    // non-vector math updates here (e.g graphics)
    // tied to user fps
      public override void _Process(float delta)
    {
          handleInput();
          updateState();
    }
    
    // for updates that require a fixed timestep
    // not tied to user fps
    public override void _PhysicsProcess(float delta)
    {
        applyGravity();
        calculateJump();
        applyMovement(inputVector);
    }

    private void handleInput()
    {
        inputVector = new Vector2(
            Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left"),
            Input.GetActionStrength("ui_up") - Input.GetActionStrength("ui_down")
        );

        if(Input.IsActionJustPressed("ui_left"))
        {
            //sets facing direction to left
            facingDirection = -1;
            var newTransform = this.Transform;
            newTransform.x.x = facingDirection;
            this.Transform = newTransform;
        }
        if(Input.IsActionJustPressed("ui_right"))
        {
            //sets facing direction to right
            facingDirection = 1;
            var newTransform = this.Transform;
            newTransform.x.x = facingDirection;
            this.Transform = newTransform;
        }

        if(Input.IsActionJustPressed("ui_select"))
        {
            jumpTimer = 0;
            jumpPressed = true;
            snapVector = Vector2.Zero;
        }

        if(Input.IsActionJustPressed("Console"))
        {
            this.hitbox.Visible = !this.hitbox.Visible;
        }
    }
    private void applyGravity()
    {
        velocity.y += gravity * GetPhysicsProcessDeltaTime();
    }

    private void applyMovement(Vector2 moveVector)
    {
        /******* currently breaks movement on slopes ***************
        lerp simulates smooth start and stop
        velocity.x = lerp(velocity.x, move_vector.x * top_speed, 0.34)
        ***********************************************************/
        velocity.x = moveVector.x * TopSpeed;
        velocity = MoveAndSlideWithSnap(velocity, snapVector, FLOOR_NORMAL, true);
    }

    private void updateState()
    {
        isFalling = velocity.y > 0;
        //reset the snap vector once we are close to the ground
        //so we don't slip on slopes
        if(floorDetector.IsColliding())
        {
            isJumping = false;
            snapVector.y = FLOOR_DETECT_DISTANCE;
        }
    }    

    private void jump()
    {
        velocity.y = -jumpInitialVelocity;
        isJumping = true;
        jumpPressed = true;
    }

    private void calculateJump()
    {
    // checks if we hit the floor within the jump_max_buffer_time of
	// pressing the jump button, then jumps at the first possible frame
	if(jumpTimer < JumpMaxBufferTime)
    {
		jumpTimer += GetPhysicsProcessDeltaTime();
		if(IsOnFloor())
			jump();
    }


	// lets the player jump within coyote_max_time seconds after falling a ledge
	// checks if we're falling because we fell off something, not because we jumped
	if(isFalling && !IsOnFloor() && !isJumping)
    {
		coyoteTimer += GetPhysicsProcessDeltaTime();
		if(coyoteTimer < CoyoteMaxTime && jumpPressed)
			jump();
    }

	else
		coyoteTimer = 0;
    }


}
