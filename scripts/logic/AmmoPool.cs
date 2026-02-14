using System.Diagnostics;
using Godot;

public partial class AmmoPool : Node
{
    [Export] public int MaxAmmo;

    [Signal] public delegate void AmmoPoolValueChangedEventHandler();

    public int AmmoPoolValue;
    private Timer? _ammoReplenishTimer;

    public override void _Ready()
    {
        Debug.Assert(MaxAmmo > 0);
        AmmoPoolValue = MaxAmmo;

        _ammoReplenishTimer = GetNode<Timer>("AmmoReplenishTimer").NotNull(nameof(_ammoReplenishTimer));
        _ammoReplenishTimer.Timeout += OnAmmoReplenishTimerTimeout;

    }

    public override void _Process(double delta)
    {
        _ammoReplenishTimer.NotNull(nameof(_ammoReplenishTimer));
        if (AmmoPoolValue < MaxAmmo && _ammoReplenishTimer.IsStopped())
        {
            _ammoReplenishTimer.Start();
        }
    }

    public void ChangeAmmoPoolValue(int value)
    {
        AmmoPoolValue = Mathf.Clamp(AmmoPoolValue + value, 0, MaxAmmo);
        EmitSignal(SignalName.AmmoPoolValueChanged);
    }

    private void OnAmmoReplenishTimerTimeout()
    {
        ChangeAmmoPoolValue(1);
    }
}
