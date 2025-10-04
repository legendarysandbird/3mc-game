using Godot;
using System.Collections.Generic;

public partial class GameManager : Node
{
	public const string CurrentSeedEntry = "currentSeed";

	private PackedScene _sceneTempLevel;
	private MainMenu _mainMenu;

	private readonly Dictionary<string, int> _gameInfo = new Dictionary<string, int> { { CurrentSeedEntry, 0 } };
	
	public override void _Ready()
	{
		_sceneTempLevel = GD.Load<PackedScene>("uid://chvfto1w6w3um");
		_mainMenu = GetNode<MainMenu>("Main Menu");

		_mainMenu.SeedSet += OnMainMenuSeedSet;
		_mainMenu.PlayPressed += OnMainMenuPlayPressed;
	}

	private void OnMainMenuPlayPressed()
	{
		Node tempLevel = _sceneTempLevel.Instantiate();
		GetTree().Root.AddChild(tempLevel);

		_mainMenu.QueueFree();
	}

	private void OnMainMenuSeedSet(int newSeed)
	{
		_gameInfo[CurrentSeedEntry] = newSeed;
	}
}
