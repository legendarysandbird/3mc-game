// #define LOG_INPUT_MANAGER

using System.Diagnostics;
using System.Linq;
using Godot;

[GlobalClass]
public partial class InputManager : Node
{
    public const string LEFT_ACTION = "player_left";
    public const string RIGHT_ACTION = "player_right";
    public const string FIRE_ACTION = "player_fire";
    public const string JUMP_ACTION = "player_jump";
    public const string AIM_LEFT_ACTION = "player_aim_left";
    public const string AIM_RIGHT_ACTION = "player_aim_right";
    public const string AIM_UP_ACTION = "player_aim_up";
    public const string AIM_DOWN_ACTION = "player_aim_down";

    private const int GODOT_MAX_DEVICE_COUNT = 8;

    private static InputManager? _instance;
    public static InputManager Instance => _instance.NotNull(nameof(_instance));

    private bool _isMultiplayerMappingActive;
    private InputDevice?[] _connectedDevices = new InputDevice[GODOT_MAX_DEVICE_COUNT];

    public override void _Ready()
    {
        _instance = this;

        Input.JoyConnectionChanged += OnJoyConnectionChanged;
    }

    public override void _Input(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion)
        {
            return;
        }

        int device = inputEvent.Device;
        if (device >= GODOT_MAX_DEVICE_COUNT)
        {
            LogInfo($"Skipping device with ID of {device}.");
            return;
        }

        PairDevice(device, inputEvent);
    }

    public int GetConnectedDeviceCount() => _connectedDevices.Count(device => device != null);

    public void SetMultiplayerMapping(int playerCount)
    {
        int connectedDeviceCount = GetConnectedDeviceCount();
        Debug.Assert(connectedDeviceCount >= playerCount);

        var originalActions = InputMap.GetActions();
        foreach (var action in originalActions)
        {
            if (action.ToString().StartsWith("ui_"))
            {
                continue;
            }

            for (int playerIndex = 0; playerIndex < playerCount; playerIndex++)
            {
                string newActionName = $"{action}_{playerIndex + 1}";
                InputMap.AddAction(newActionName);

                var inputDevice = _connectedDevices[playerIndex];
                Debug.Assert(inputDevice != null);

                bool isKeyboard = inputDevice.IsKeyboard;
                var originalEvents = InputMap.ActionGetEvents(action);
                foreach (var inputEvent in originalEvents)
                {
                    if (isKeyboard)
                    {
                        if (inputEvent is not (InputEventKey or InputEventMouseButton))
                        {
                            continue;
                        }
                    }
                    else if (inputEvent is not (InputEventJoypadButton or InputEventJoypadMotion))
                    {
                        continue;
                    }

                    var newInputEvent = (InputEvent)inputEvent.Duplicate();
                    newInputEvent.Device = playerIndex;
                    InputMap.ActionAddEvent(newActionName, newInputEvent);
                }
            }

            InputMap.EraseAction(action);
        }

        foreach (var action in InputMap.GetActions())
        {
            if (action.ToString().StartsWith("ui_"))
            {
                continue;
            }

            foreach (var inputEvent in InputMap.ActionGetEvents(action))
            {
                LogInfo($"Event: {inputEvent.AsText()}. Device: {inputEvent.Device}");
            }
        }

        _isMultiplayerMappingActive = true;

        LogInfo($"Setting multiplayer mapping for {playerCount} players!");
    }

    public void ResetMapping()
    {
        InputMap.LoadFromProjectSettings();
        _isMultiplayerMappingActive = false;

        LogInfo("Restoring Input Map to project settings!");
    }

    public bool IsActionPressed(int playerNumber, string action)
    {
        return Input.IsActionPressed(GetActionStringName(playerNumber, action));
    }

    public float GetAxis(int playerNumber, string negativeAction, string positiveAction)
    {
        float negativeActionStrength = GetPlayerActionStrength(playerNumber, negativeAction);
        float positiveActionStrength = GetPlayerActionStrength(playerNumber, positiveAction);

        return positiveActionStrength - negativeActionStrength;
    }

    public Vector2 GetVector(int playerNumber, string negativeX, string positiveX, string negativeY, string positiveY)
    {
        float xAxis = GetAxis(playerNumber, negativeX, positiveX);
        float yAxis = GetAxis(playerNumber, negativeY, positiveY);

        return new Vector2(xAxis, yAxis);
    }

    public Vector2 GetGlobalMousePosition(int playerNumber, Node2D caller)
    {
        if (ConfigHelper.GetMouseAndKeyboardDisabled())
        {
            return Vector2.Zero;
        }

        if (!IsPlayerUsingKeyboard(playerNumber))
        {
            return Vector2.Zero;
        }

        return caller.GetGlobalMousePosition();
    }

    private float GetPlayerActionStrength(int playerNumber, string action)
    {
        return Input.GetActionStrength(GetActionStringName(playerNumber, action));
    }

    private string GetActionStringName(int playerNumber, string action)
    {
        return _isMultiplayerMappingActive ? $"{action}_{playerNumber}" : action;
    }

    private bool IsPlayerUsingKeyboard(int playerNumber)
    {
        var playerDevice = _connectedDevices[playerNumber - 1];
        return playerDevice != null && playerDevice.IsKeyboard;
    }

    private void OnJoyConnectionChanged(long device, bool connected)
    {
        if (device >= GODOT_MAX_DEVICE_COUNT)
        {
            LogInfo($"Skipping device with ID of {device}.");
            return;
        }

        if (connected)
        {
            var deviceName = Input.GetJoyName((int)device);
            LogInfo($"Device {device} with name {deviceName} detected!");
        }
        else
        {
            var deviceName = _connectedDevices[device]?.Name;
            _connectedDevices[device] = null;
            LogInfo($"Device {device} with name {deviceName} disconnected!");
        }
    }

    private void PairDevice(int device, InputEvent inputEvent)
    {
        if (_connectedDevices[device] != null)
        {
            // Device already paired
            return;
        }

        if (inputEvent is InputEventMouseButton)
        {
            return;
        }

        bool isKeyboard = inputEvent is InputEventKey;
        _connectedDevices[device] = new InputDevice(device, isKeyboard);

        var deviceName = Input.GetJoyName((int)device);
        LogInfo($"Device {device} with name {deviceName} connected!");
    }

    [Conditional("LOG_INPUT_MANAGER")]
    private void LogInfo(params object[] message)
    {
        Logger.Info(message);
    }
}
