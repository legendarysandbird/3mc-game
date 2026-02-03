using Godot;

[GlobalClass]
public partial class Projectile : Area2D
{
	private static readonly PackedScene _projectileScene = GD.Load<PackedScene>("uid://8tkgka1tp8wf");

	private Vector2 _velocity;
	private VisibleOnScreenNotifier2D? _onScreenNotifier;

	public int Damage;

	public static Projectile Create(Vector2 startingPosition, Vector2 startingVelocity, int startingDamage = -1)
	{
		Projectile projectile = _projectileScene.Instantiate<Projectile>();
		projectile.GlobalPosition = startingPosition;
		projectile._velocity = startingVelocity;
		projectile.Damage = startingDamage;

		return projectile;
	}

	public override void _Ready()
	{
		_onScreenNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D").NotNull(nameof(_onScreenNotifier));

		_onScreenNotifier.ScreenExited += OnScreenExited;
		BodyEntered += OnBodyEntered;
		LookAt(GlobalPosition + _velocity);
	}

	public override void _PhysicsProcess(double delta)
	{
		GlobalPosition += _velocity * (float)delta;
	}

	private void OnScreenExited()
	{
		QueueFree();
	}

	private void OnBodyEntered(Node body)
	{
		if (body is TileMapLayer)
		{
			QueueFree();
		}
	}
}
