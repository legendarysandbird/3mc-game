using Godot;

public partial class AmmoPoolBar : ProgressBar
{
	
	[Export] private AmmoPool? _ammoPool;
	
	public override void _Ready()
	{
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
