using System.Collections.Generic;
using Godot;

public partial class GameManager : Node
{
    public enum PlayerMode
    {
        SinglePlayer,
        LocalMultiplayer
    }

    public const string CurrentSeedEntry = "currentSeed";

    private PackedScene? _sceneTempLevel;
    private MainMenu? _mainMenu;

    private readonly Dictionary<string, int> _gameInfo = new Dictionary<string, int> { { CurrentSeedEntry, 0 } };

    private PlayerMode _playerMode;

    public override void _Ready()
    {
        _sceneTempLevel = GD.Load<PackedScene>("uid://chvfto1w6w3um").NotNull(nameof(_sceneTempLevel));
        _mainMenu = GetNode<MainMenu>("Main Menu").NotNull(nameof(_mainMenu));

        _mainMenu.SeedSet += OnMainMenuSeedSet;
        _mainMenu.PlayPressed += OnMainMenuPlayPressed;
    }

    private void OnMainMenuPlayPressed(PlayerMode playerMode)
    {
        _sceneTempLevel.NotNull(nameof(_sceneTempLevel));
        _mainMenu.NotNull(nameof(_mainMenu));

        _playerMode = playerMode;

        if (playerMode == PlayerMode.LocalMultiplayer)
        {
            InputManager.Instance.SetDeviceMap(new InputDevice[] { new InputDevice(0, true), new InputDevice(1, false) });
        }

        Node tempLevel = _sceneTempLevel.Instantiate();
        GetTree().Root.AddChild(tempLevel);

        _mainMenu.QueueFree();
    }

    private void OnMainMenuSeedSet(int newSeed)
    {
        _gameInfo[CurrentSeedEntry] = newSeed;
    }
}
