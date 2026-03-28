using System;
using Godot;

public partial class MainMenu : Node
{
    private enum MenuPanel { MainPanel, SettingsPanel };

    public event Action<GameManager.PlayerMode>? PlayPressed;
    public event Action<int>? SeedSet;

    private LineEdit? _seedEdit;
    private Button? _singleplayerPlay;
    private Button? _localMultiplayerPlay;
    private Button? _settings;
    private Button? _disableMouseAndKeyboard;
    private Button? _settingsBack;
    private VBoxContainer? _panelsParent;
    private MenuPanel _currentPanel;

    public override void _Ready()
    {
        _seedEdit = GetNode<LineEdit>("Background/VBoxContainer/Panels/MainPanel/SeedEdit").NotNull(nameof(_seedEdit));
        _singleplayerPlay = GetNode<Button>("Background/VBoxContainer/Panels/MainPanel/Singleplayer").NotNull(nameof(_singleplayerPlay));
        _localMultiplayerPlay = GetNode<Button>("Background/VBoxContainer/Panels/MainPanel/LocalMultiplayer").NotNull(nameof(_localMultiplayerPlay));
        _settings = GetNode<Button>("Background/VBoxContainer/Panels/MainPanel/Settings").NotNull(nameof(_settings));
        _disableMouseAndKeyboard = GetNode<Button>("Background/VBoxContainer/Panels/SettingsPanel/MouseAndKeyboard").NotNull(nameof(_disableMouseAndKeyboard));
        _settingsBack = GetNode<Button>("Background/VBoxContainer/Panels/SettingsPanel/Back").NotNull(nameof(_settingsBack));
        _panelsParent = GetNode<VBoxContainer>("Background/VBoxContainer/Panels").NotNull(nameof(_panelsParent));

        _disableMouseAndKeyboard.ButtonPressed = ConfigHelper.GetMouseAndKeyboardDisabled();

        _singleplayerPlay.Pressed += OnSinglePlayerButtonPressed;
        _localMultiplayerPlay.Pressed += OnLocalMultiplayerPlayerButtonPressed;
        _settings.Pressed += GoToSettingsPanel;
        _disableMouseAndKeyboard.Toggled += ToggleMouseAndKeyboard;
        _settingsBack.Pressed += GoToMainPanel;
    }

    private void OnSinglePlayerButtonPressed()
    {
        OnPlayButtonPressed(GameManager.PlayerMode.SinglePlayer);
    }

    private void OnLocalMultiplayerPlayerButtonPressed()
    {
        OnPlayButtonPressed(GameManager.PlayerMode.LocalMultiplayer);
    }

    private void OnPlayButtonPressed(GameManager.PlayerMode playerMode)
    {
        _seedEdit.NotNull(nameof(_seedEdit));

        string text = _seedEdit.Text;
        uint mySeed = string.IsNullOrEmpty(text) ? GD.Randi() % 1000000 : text.Hash();

        GD.Seed(mySeed);

        SeedSet?.Invoke((int)mySeed);
        PlayPressed?.Invoke(playerMode);
    }

    private void GoToMainPanel() => SetPanel(MenuPanel.MainPanel);
    private void GoToSettingsPanel() => SetPanel(MenuPanel.SettingsPanel);

    private void SetPanel(MenuPanel newPanel)
    {
        _panelsParent.NotNull(nameof(_panelsParent));

        if (newPanel == _currentPanel)
        {
            return;
        }

        var panels = _panelsParent.GetChildren();
        for (int i = 0; i < panels.Count; i++)
        {
            var panel = panels[i];
            if (panel is not Control control)
            {
                continue;
            }

            control.Visible = i == (int)newPanel;
        }

        _currentPanel = newPanel;
    }

    private void ToggleMouseAndKeyboard(bool toggled)
    {
        ConfigHelper.SetMouseAndKeyboardDisabled(toggled);
    }
}
