using Godot;

public partial class AmmoPoolBar : ProgressBar
{

    [Export] private Node? _ammoPoolConnection;

    private AmmoPool? _ammoPool;

    public override void _Ready()
    {
        _ammoPoolConnection.NotNull(nameof(_ammoPoolConnection));

        if (_ammoPoolConnection is PlayerHud)
        {
            PlayerHud? playerHud = _ammoPoolConnection.NotNull(nameof(_ammoPoolConnection)) as PlayerHud;
            _ammoPool = playerHud.NotNull(nameof(playerHud)).Player.NotNull(nameof(Player)).GetNode<AmmoPool>("AmmoPool") as AmmoPool;
        }
        else
        {
            _ammoPool = _ammoPoolConnection as AmmoPool;
        }

        _ammoPool.NotNull(nameof(_ammoPool));

        _ammoPool.AmmoPoolValueChanged += OnAmmoPoolValueChanged;
        MaxValue = _ammoPool.MaxAmmo;
        Value = _ammoPool.AmmoPoolValue;
    }

    private void OnAmmoPoolValueChanged()
    {
        _ammoPool.NotNull(nameof(_ammoPool));
        Value = _ammoPool.AmmoPoolValue;
    }
}
