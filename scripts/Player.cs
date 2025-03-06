using Godot;
using System;

public class Player : KinematicBody2D
{
    public int Speed = 150;
    public Vector2 Velocity = new Vector2();
    public Vector2 Direction = new Vector2();
    public Vector2 LastDirection = new Vector2();
    public int NetworkId = -1;
    private AnimationPlayer anim;
    private Sprite sprite;

    public override void _Ready()
    {
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite>("Sprite");
        LastDirection = Direction;
        anim.Play("parou");
        SetAngle(LastDirection.Rotated(Mathf.Deg2Rad(-90)).Angle());
    }
    public override void _PhysicsProcess(float delta)
    {

        if (Direction != Vector2.Zero)
        {
            if (Direction != LastDirection)
            {
                LastDirection = Direction;
            }
            anim.Play("ande");
            // SetAngle(LastDirection.Rotated(Mathf.Deg2Rad(-90)).Angle());
        }
        else
        {
            anim.Play("parou");
        }
        SetAngle(LastDirection.Rotated(Mathf.Deg2Rad(-90)).Angle());
    
        Velocity = MoveAndSlide(Direction * Speed);
    }

    private void SetAngle(float angle)
    {
        int direction_count = 8;
        int height = 80;
        int width = 80;
        float _angle = Mathf.Round(Mathf.Rad2Deg(angle)) + 180;
        float step = 360 / direction_count;
        
        int direction = Mathf.RoundToInt(_angle / step);
        sprite.Hframes = sprite.Texture.GetWidth() / width;
        sprite.RegionRect = new Rect2(0, direction * height, sprite.Texture.GetWidth(), height );
    }

}
