using Godot;

public partial class HealthBar : ProgressBar
{

    [Export] private Node? _healthPoolConnection;

    private Health? _healthPool;

    public override void _Ready()
    {
        _healthPoolConnection.NotNull(nameof(_healthPoolConnection));

        if (_healthPoolConnection is PlayerHud)
        {
            PlayerHud? playerHud = _healthPoolConnection.NotNull(nameof(_healthPoolConnection)) as PlayerHud;
            _healthPool = playerHud.NotNull(nameof(playerHud)).Player.NotNull(nameof(Player)).GetNode<Health>("Health") as Health;
        }
        else
        {
            _healthPool = _healthPoolConnection as Health;
        }

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
