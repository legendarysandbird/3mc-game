using Godot;
using System.Diagnostics;

[GlobalClass]
public partial class EnemyDirector : Node
{
	[Export] private Godot.Collections.Array<PackedScene>? _mobTypes;
	[Export] private PathFollow2D? _spawnNode;
	[Export] private int _maxMobCount;

	private Timer? _spawnTimer;
	private int _curMobCount;


	[Signal] public delegate void EnemyDeathEventHandler();

	public override void _Ready()
	{
		_mobTypes.NotNull(nameof(_mobTypes));
		_spawnNode.NotNull(nameof(_spawnNode));

		Debug.Assert(_maxMobCount > 0);
		Debug.Assert(_curMobCount == 0);
	
		_spawnTimer = GetNode<Timer>("SpawnTimer").NotNull(nameof(_spawnTimer));
		_spawnTimer.Timeout += OnSpawnTimerTimeout;

		// Give us one enemy to start with so we aren't waiting 5 seconds
		// Coordinates are where the debugging enemy usually is
		AddChild(CreateEnemyAt(new Vector2(765, 253)));
		_curMobCount += 1;
	
		_spawnTimer.Start();
	}

	private Enemy CreateEnemy()
	{
		_spawnNode.NotNull(nameof(_spawnNode));

		_spawnNode.ProgressRatio = GD.Randf();
		return CreateEnemyAt(_spawnNode.Position);
	}

	private Enemy CreateEnemyAt(Vector2 pos)
	{
		_mobTypes.NotNull(nameof(_mobTypes));
	
		PackedScene mobScene = _mobTypes.PickRandom();
		Enemy mob = mobScene.Instantiate<Enemy>();
		mob.Position = pos;

		mob.EnemyDeath += OnEnemyDeath;

		return mob;
	}

	private void OnSpawnTimerTimeout()
	{
		if (_curMobCount < _maxMobCount)
		{
			AddChild(CreateEnemy());
			_curMobCount++;
		}
	}

	// Event bus for communicating enemy death with the score counter
	private void OnEnemyDeath()
	{
		EmitSignal(SignalName.EnemyDeath);
		_curMobCount--;
	}
}
