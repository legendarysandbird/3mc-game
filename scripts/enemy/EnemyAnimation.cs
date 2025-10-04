using Godot;

public partial class EnemyAnimation : AnimatedSprite2D
{
    [Export] private CharacterBody2D? _enemy;

    public override void _Ready()
    {
        _enemy.NotNull(nameof(_enemy));

        Play();
    }

    public override void _Process(double _delta)
    {
        _enemy.NotNull(nameof(_enemy));

        Animation = _enemy.Velocity == Vector2.Zero ? "idle" : "aggro";
    }
}
