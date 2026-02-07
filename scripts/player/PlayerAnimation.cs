using Godot;

public partial class PlayerAnimation : AnimatedSprite2D
{
    private float _xScaleCache;
    [Export] private Node2D? _armNode;
    [Export] private Player? _playerNode;

    public override void _Ready()
    {
        _armNode = GetNode<Node2D>("Arm").NotNull(nameof(_armNode));
        _playerNode = GetParent<Player>().NotNull(nameof(_armNode));
        _xScaleCache = Scale.X;
        Play();
    }

    public override void _Process(double delta)
    {
        SetAnimation();
        AnimateArm();
    }

    private void AnimateArm()
    {
        _armNode.NotNull(nameof(_armNode));
        _playerNode.NotNull(nameof(_playerNode));

        _armNode.LookAt(_armNode.GlobalPosition + _playerNode.ProjectileDirection);
    }

    private void SetAnimation()
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
