using Godot;
using System.Diagnostics;

public partial class Health : Node
{
	[Export] public int MaxHealth;

	[Signal] public delegate void HealthEmptyEventHandler();
	[Signal] public delegate void HealthPoolChangedEventHandler();

	public int HealthPool;
	
	public override void _Ready()
	{
		Debug.Assert(MaxHealth > 0);
		HealthPool = MaxHealth;
	}

	public void ChangeHealth(int value)
	{
		HealthPool = Mathf.Clamp(HealthPool + value, 0, MaxHealth);
		if (HealthPool == 0)
		{
			EmitSignal(SignalName.HealthEmpty);
		}

		EmitSignal(SignalName.HealthPoolChanged);
	}
}
