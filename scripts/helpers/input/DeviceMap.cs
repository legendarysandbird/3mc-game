using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class DeviceMap
{
    public const int MAX_DEVICE_COUNT = 2;
    private InputDevice?[] _devices;

    public DeviceMap(InputDevice[] devices)
    {
        Validate(devices);

        _devices = devices;
    }

    public void AddDevice(InputDevice device)
    {
        int emptyIndex = GetEmptyDeviceIndex();
        if (emptyIndex == -1)
        {
            Logger.Error("Could not find an empty device slot!");
            return;
        }

        Logger.Info($"Assigning device {device} with name {device.Name} as input {emptyIndex + 1}");

        _devices[emptyIndex] = device;
    }

    public void RemoveDevice(int deviceIdToRemove)
    {
        int index = GetDeviceIndex(deviceIdToRemove);
        if (index < 0)
        {
            Logger.Error($"Could not find device with ID of {deviceIdToRemove}!");
            return;
        }

        string? deviceName = _devices[index]?.Name;
        Logger.Info($"Removing device {deviceIdToRemove} with name {deviceName} as input {index + 1}");

        _devices[index] = null;
    }

    public bool IsFull() => GetEmptyDeviceIndex() < 0;

    public bool HasDevice(int device) => GetDeviceIndex(device) >= 0;

    public InputDevice? GetPlayerDevice(int playerNumber) => _devices[playerNumber - 1];

    public bool IsKeyboardAlreadyInUse() => _devices.Any(device => device?.IsKeyboard ?? false);

    private int GetEmptyDeviceIndex() => Array.FindIndex(_devices, device => device == null);

    private int GetDeviceIndex(int deviceIdToFind) => Array.FindIndex(_devices, device => device?.DeviceId == deviceIdToFind);

    private void Validate(InputDevice[] devices)
    {
        int length = devices.Length;
        if (length > MAX_DEVICE_COUNT)
        {
            throw new ArgumentException($"Trying to set a device map of size {length} but the max size is {MAX_DEVICE_COUNT}!");
        }

        if (length < 2)
        {
            throw new ArgumentException($"Trying to set a device map of size {length} but the min size is 2!");
        }

        if (devices.Count(device => device.IsKeyboard) > 1)
        {
            throw new ArgumentException("Trying to map multiple keyboards!", nameof(devices));
        }

        var uniqueDevices = new HashSet<int>();
        foreach (var device in devices)
        {
            int deviceId = device.DeviceId;
            if (!uniqueDevices.Add(deviceId))
            {
                throw new ArgumentException($"Trying to add multiple devices with the same ID: {deviceId}");
            }
        }
    }
}
