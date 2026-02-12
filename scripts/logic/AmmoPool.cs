using Godot;
using System.Diagnostics;

public partial class AmmoPool : Node
{
	[Export] public int MaxAmmo;
	
	[Signal] public delegate void AmmoPoolChangedEventHandler();
	
	public int AmmoPool;

	public override void _Ready()
	{
		Debug.Assert(MaxAmmo > 0);
		AmmoPool = MaxAmmo;
	}

	public void ChangeAmmoPoolValue(int value)
	{
		AmmoPool = Mathf.Clamp(AmmoPool + value, 0, MaxAmmo);
		EmitSignal(SignalName.AmmoPoolChanged);
	}
}
