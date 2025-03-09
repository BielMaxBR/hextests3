using Godot;
using System;

public class Player : KinematicBody2D
{
    public int Speed = 150;
    public Vector2 Velocity = new Vector2();
    public Vector2 Direction = new Vector2();
    public Vector2 LastDirection = new Vector2();
    public Vector2 LastPosition = new Vector2();
    public bool IsLocal = false;
    public bool IsClient = false;
    public int NetworkId = -1;
    private AnimationPlayer anim;
    private Sprite sprite;

    public Vector2 ServerPosition = new Vector2();
    private Vector2 PredictedPosition = new Vector2();
    public override void _Ready()
    {
        GetNode<Camera2D>("Camera2D").Current = IsLocal;
        anim = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite>("Sprite");
        
        PredictedPosition = Position;
        ServerPosition = Position;
        LastPosition = Position;
        
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
        if (!IsClient) Velocity = MoveAndSlide(Direction * Speed);
        else
        {
            PredictedPosition += Velocity * delta;

            if (PredictedPosition.DistanceTo(ServerPosition) > 50)
            {
                // GD.Print(Velocity);
                PredictedPosition = ServerPosition;
            } else {
                PredictedPosition = new Vector2(
                    Mathf.Lerp(PredictedPosition.x, ServerPosition.x, 0.3f),
                    Mathf.Lerp(PredictedPosition.y, ServerPosition.y, 0.3f)
                );
            }
            Position = PredictedPosition;
        }
        if (LastPosition != Position)
        {
            LastPosition = Position;
        }
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
        sprite.RegionRect = new Rect2(0, direction * height, sprite.Texture.GetWidth(), height);
    }

}
