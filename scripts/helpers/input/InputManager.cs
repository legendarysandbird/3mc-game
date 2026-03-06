using Godot;

[GlobalClass]
public partial class InputManager : Node
{
    private static InputManager? _instance;
    public static InputManager Instance => _instance.NotNull(nameof(_instance));

    private bool _isPairing;
    private DeviceMap? _deviceMap;

    public override void _Ready()
    {
        _instance = this;

        Input.JoyConnectionChanged += OnJoyConnectionChanged;
    }

    public override void _Input(InputEvent inputEvent)
    {
        HandlePairingAttempt(inputEvent);
    }

    public void SetDeviceMap(InputDevice[] inputDevices)
    {
        _deviceMap = new DeviceMap(inputDevices);
    }

    public void ClearDeviceMap()
    {
        _deviceMap = null;
    }

    public bool IsActionPressed(int playerNumber, string action) => ShouldBlockInput(playerNumber, action) ? false : Input.IsActionPressed($"player_{action}");

    public float GetAxis(int playerNumber, string negativeAction, string positiveAction)
    {
        float negativeActionStrength = ShouldBlockInput(playerNumber, negativeAction) ? 0.0f : Input.GetActionStrength($"player_{negativeAction}");
        float positiveActionStrength = ShouldBlockInput(playerNumber, positiveAction) ? 0.0f : Input.GetActionStrength($"player_{positiveAction}");

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

        if (_deviceMap != null && !IsPlayerUsingMouse(playerNumber, _deviceMap))
        {
            return Vector2.Zero;
        }

        return caller.GetGlobalMousePosition();
    }

    private bool ShouldBlockInput(int playerNumber, string action)
    {
        var actionEvents = InputMap.ActionGetEvents(action);

        if (ConfigHelper.GetMouseAndKeyboardDisabled())
        {
            if (IsKeyboardInput(actionEvents))
            {
                return true;
            }
        }

        if (_deviceMap != null && !IsEventFromPlayer(playerNumber, _deviceMap, actionEvents))
        {
            return true;
        }

        return false;
    }

    private bool IsKeyboardInput(Godot.Collections.Array<InputEvent> actionEvents)
    {
        foreach (var actionEvent in actionEvents)
        {
            if (actionEvent is not (InputEventKey or InputEventMouseButton))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsEventFromPlayer(int playerNumber, DeviceMap deviceMap, Godot.Collections.Array<InputEvent> actionEvents)
    {
        var playerDevice = GetInputDevice(playerNumber, deviceMap);
        if (playerDevice == null)
        {
            return false;
        }

        foreach (var actionEvent in actionEvents)
        {
            if (actionEvent.Device == playerDevice.DeviceId)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsPlayerUsingMouse(int playerNumber, DeviceMap deviceMap)
    {
        var playerDevice = GetInputDevice(playerNumber, deviceMap);
        return playerDevice != null && playerDevice.IsKeyboard;
    }

    private static InputDevice? GetInputDevice(int playerNumber, DeviceMap deviceMap)
    {
        if (playerNumber < 1 || playerNumber > DeviceMap.MAX_DEVICE_COUNT)
        {
            Logger.Error($"Player number of {playerNumber} is not valid!");
            return null;
        }

        var playerDevice = deviceMap.GetPlayerDevice(playerNumber);
        return playerDevice;
    }

    private void OnJoyConnectionChanged(long device, bool connected)
    {
        var deviceName = Input.GetJoyName((int)device);
        if (connected)
        {
            Logger.Info($"Device {device} with name {deviceName} connected!");
        }
        else
        {
            if (_deviceMap != null && _deviceMap.HasDevice((int)device))
            {
                _deviceMap.RemoveDevice((int)device);
                _isPairing = true;
            }

            Logger.Info($"Device {device} with name {deviceName} disconnected!");
        }
    }

    private void HandlePairingAttempt(InputEvent inputEvent)
    {
        if (!_isPairing)
        {
            return;
        }

        if (_deviceMap == null)
        {
            Logger.Error("Trying to pair devices but we have no device map!");
            return;
        }

        if (_deviceMap.IsFull())
        {
            Logger.Error("Trying to pair in game, but the device map is full!");
            return;
        }

        int device = inputEvent.Device;
        if (_deviceMap.HasDevice(device))
        {
            return;
        }

        if (inputEvent is InputEventKey)
        {
            if (ConfigHelper.GetMouseAndKeyboardDisabled() || _deviceMap.IsKeyboardAlreadyInUse())
            {
                return;
            }
            else
            {
                _deviceMap.AddDevice(new InputDevice(device, true));
            }
        }
        else if (inputEvent is InputEventJoypadButton)
        {
            _deviceMap.AddDevice(new InputDevice(device, false));
        }

        if (_isPairing && _deviceMap.IsFull())
        {
            _isPairing = false;
        }
    }
}
