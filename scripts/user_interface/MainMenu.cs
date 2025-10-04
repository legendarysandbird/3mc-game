using Godot;

public partial class MainMenu : Node
{
	[Signal] public delegate void PlayPressedEventHandler();
	[Signal] public delegate void SeedSetEventHandler(int newSeed);

	private Button _buttonPlay;
	private LineEdit _seedEdit;

	public override void _Ready()
	{
		_buttonPlay = GetNode<Button>("PLAY");
		_seedEdit = GetNode<LineEdit>("SeedEdit");

		_buttonPlay.Pressed += OnPlayButtonPressed;
	}

	private void OnPlayButtonPressed()
	{
		string text = _seedEdit.Text;
		uint mySeed = string.IsNullOrEmpty(text) ? GD.Randi() % 1000000 : text.Hash();

		GD.Seed(mySeed);
		EmitSignal(SignalName.SeedSet, mySeed);
		EmitSignal(SignalName.PlayPressed);
	}
}
