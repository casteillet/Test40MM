using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class HosasConnectionMonitor : PersistentSingleton<HosasConnectionMonitor>
{
    public static event Action<HosasStickSide, bool> StickConnectionChanged;

    private readonly Dictionary<HosasStickSide, HashSet<int>> _connectedDeviceIdsByStickSide = new()
    {
        { HosasStickSide.Left, new HashSet<int>() },
        { HosasStickSide.Right, new HashSet<int>() }
    };

    public bool IsConnected(HosasStickSide stickSide) => _connectedDeviceIdsByStickSide[stickSide].Count > 0;

    protected override void Awake()
    {
        base.Awake();

        if (instance != this) return;

        InputSystem.onDeviceChange += HandleDeviceChange;
        SeedFromCurrentDevices();
    }

    private void OnDestroy()
    {
        if (instance != this) return;

        InputSystem.onDeviceChange -= HandleDeviceChange;
    }

    private void SeedFromCurrentDevices()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            RegisterDevice(device);
        }
    }

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
                RegisterDevice(device);
                break;

            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                UnregisterDevice(device);
                break;
        }
    }

    private void RegisterDevice(InputDevice device)
    {
        if (!HosasDeviceIdentifier.TryIdentifyStickSide(device, out HosasStickSide stickSide)) return;

        bool wasConnected = IsConnected(stickSide);
        _connectedDeviceIdsByStickSide[stickSide].Add(device.deviceId);
        RaiseIfConnectionChanged(stickSide, wasConnected);
    }

    private void UnregisterDevice(InputDevice device)
    {
        if (!HosasDeviceIdentifier.TryIdentifyStickSide(device, out HosasStickSide stickSide)) return;

        bool wasConnected = IsConnected(stickSide);
        _connectedDeviceIdsByStickSide[stickSide].Remove(device.deviceId);
        RaiseIfConnectionChanged(stickSide, wasConnected);
    }

    private void RaiseIfConnectionChanged(HosasStickSide stickSide, bool wasConnected)
    {
        bool isConnected = IsConnected(stickSide);

        if (isConnected == wasConnected) return;

        StickConnectionChanged?.Invoke(stickSide, isConnected);
    }
}
