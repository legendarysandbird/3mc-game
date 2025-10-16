using Godot;

public partial class PlayerAnimation : AnimatedSprite2D
{
    private float _xScaleCache;

    public override void _Ready()
    {
        _xScaleCache = Scale.X;
        Play();
    }

    public override void _Process(double delta)
    {
        Animate();
    }

    private void Animate()
    {
        if (Input.IsActionPressed("player_left") != Input.IsActionPressed("player_right"))
        {
            Animation = "walk";
        }
        else
        {
            Animation = "idle";
            return;
        }

        Scale = Scale with { X = _xScaleCache * (Input.IsActionPressed("player_left") ? -1 : 1) };
    }
}
