using Godot;

public partial class MainMenu : Node
{
	[Signal] public delegate void PlayPressedEventHandler();
	[Signal] public delegate void SeedSetEventHandler(int newSeed);

	private Button? _buttonPlay;
	private LineEdit? _seedEdit;

	public override void _Ready()
	{
		_buttonPlay = GetNode<Button>("PLAY").NotNull(nameof(_buttonPlay));
		_seedEdit = GetNode<LineEdit>("SeedEdit").NotNull(nameof(_seedEdit));

		_buttonPlay.Pressed += OnPlayButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		_seedEdit.NotNull(nameof(_seedEdit));
	
		string text = _seedEdit.Text;
		uint mySeed = string.IsNullOrEmpty(text) ? GD.Randi() % 1000000 : text.Hash();

		GD.Seed(mySeed);
		EmitSignal(SignalName.SeedSet, mySeed);
		EmitSignal(SignalName.PlayPressed);
	}
}
