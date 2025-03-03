using Godot;
using System;

public class Player : KinematicBody2D
{
    public int Speed = 200;
    public Vector2 Velocity = new Vector2();
    public Vector2 Direction = new Vector2();
    public int NetworkId = -1;

    public override void _PhysicsProcess(float delta)
    {
        Velocity = MoveAndSlide(Direction * Speed);
    }
}
