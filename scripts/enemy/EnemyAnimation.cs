using Godot;

public partial class EnemyAnimation : AnimatedSprite2D
{
	[Export] private CharacterBody2D _enemy;
	
	public override void _Ready()
	{
		Play();
	}

	public override void _Process(double _delta)
	{
		Animation = _enemy.Velocity == Vector2.Zero ? "idle" : "aggro";
	}
}
