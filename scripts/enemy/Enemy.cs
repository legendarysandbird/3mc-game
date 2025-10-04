using Godot;

[GlobalClass]
public partial class Enemy : CharacterBody2D
{
	[Export] private float _speed = 100.0f;
	[Export] private int _contactDamage = 1;

	[Signal] public delegate void EnemyDeathEventHandler();
	
	private VisibleOnScreenNotifier2D? _visibilityNotifier;
	private Area2D? _hitbox;
	private Health? _healthPool;
	private Player? _player;
	private Vector2 _calculatedVelocity;

	public override void _Ready()
	{
		_visibilityNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D").GDNotNull(nameof(_visibilityNotifier));
		_hitbox = GetNode<Area2D>("Hitbox").GDNotNull(nameof(_hitbox));
		_healthPool = GetNode<Health>("Health").GDNotNull(nameof(_healthPool));
		_player = (Player)GetTree().Root.FindChildren("*", nameof(Player), true, false)[0].NotNull(nameof(_player));

		_hitbox.AreaEntered += OnHitboxAreaEntered;
		_healthPool.HealthEmpty += OnHealthPoolEmpty;
	}

	public override void _PhysicsProcess(double delta)
	{
		Velocity = CalculateVelocity();
		MoveAndSlide();
	}

	private Vector2 CalculateVelocity()
	{
		_visibilityNotifier.NotNull(nameof(_visibilityNotifier));

		if (!_visibilityNotifier.IsOnScreen() || !IsInstanceValid(_player))
		{
			return Vector2.Zero;
		}

		Vector2 direction = (_player.GlobalPosition - GlobalPosition).Normalized();
		return direction * _speed;
	}

	private void OnHitboxAreaEntered(Node2D area)
	{
		_healthPool.NotNull(nameof(_healthPool));

		if (area is Projectile projectile)
		{
			_healthPool.ChangeHealth(projectile.Damage);
			area.QueueFree();
		}
	}

	private void OnHealthPoolEmpty()
	{
		EmitSignal(SignalName.EnemyDeath);
		QueueFree();
	}
}
