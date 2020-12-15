using Godot;
using System;

// parent class for all player characters affected by gravity
public class Actor: KinematicBody2D
{
    [Export]
    public float gravity = 1960.0f ;
    public Vector2 velocity = Vector2.Zero ;

    public override void _PhysicsProcess(float delta)
    {
         velocity.y += gravity * delta ;
    }

}