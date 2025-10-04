using Godot;

public partial class HealthBar : ProgressBar
{
	[Export] private Health _healthPool;
	
	public override void _Ready()
	{
		_healthPool.HealthPoolChanged += OnHealthPoolChanged;

		MaxValue = _healthPool.MaxHealth;
		Value = _healthPool.HealthPool;
	}

	private void OnHealthPoolChanged()
	{
		Value = _healthPool.HealthPool;
	}
}
