using System;
using Godot;

public class InputDevice
{
    public readonly int DeviceId;
    public readonly string GUID;
    public readonly string Name;
    public readonly bool IsKeyboard;

    public InputDevice(int deviceId, bool isKeyboard)
    {
        Validate(deviceId);

        DeviceId = deviceId;
        GUID = Input.GetJoyGuid(deviceId);
        Name = Input.GetJoyName(deviceId);
        IsKeyboard = isKeyboard;
    }

    private void Validate(int deviceId)
    {
        var connectedJoypads = Input.GetConnectedJoypads();
        if (!connectedJoypads.Contains(deviceId))
        {
            throw new Exception($"There is no device connected with an id of {deviceId}");
        }
    }
}

