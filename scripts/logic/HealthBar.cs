using Godot;

public partial class HealthBar : ProgressBar
{
    [Export] private Health? _healthPool;

    public override void _Ready()
    {
        _healthPool.NotNull(nameof(_healthPool));

        _healthPool.HealthPoolChanged += OnHealthPoolChanged;

        MaxValue = _healthPool.MaxHealth;
        Value = _healthPool.HealthPool;
    }

    private void OnHealthPoolChanged()
    {
        _healthPool.NotNull(nameof(_healthPool));

        Value = _healthPool.HealthPool;
    }
}
