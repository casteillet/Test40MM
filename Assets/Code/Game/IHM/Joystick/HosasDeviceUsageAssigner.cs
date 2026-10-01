using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public static class HosasDeviceUsageAssigner
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
        InputSystem.onDeviceChange += HandleDeviceChange;

        foreach (InputDevice device in InputSystem.devices)
        {
            AssignStickUsage(device);
        }
    }

    private static void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (change != InputDeviceChange.Added && change != InputDeviceChange.Reconnected) return;

        AssignStickUsage(device);
    }

    private static void AssignStickUsage(InputDevice device)
    {
        if (!HosasDeviceIdentifier.TryIdentifyStickSide(device, out HosasStickSide stickSide)) return;

        InternedString stickUsage = GetUsageForStickSide(stickSide);

        if (HasUsage(device, stickUsage)) return;

        InputSystem.SetDeviceUsage(device, stickUsage);
    }

    private static bool HasUsage(InputDevice device, InternedString usage)
    {
        foreach (InternedString deviceUsage in device.usages)
        {
            if (deviceUsage == usage) return true;
        }

        return false;
    }

    private static InternedString GetUsageForStickSide(HosasStickSide stickSide)
    {
        return stickSide switch
        {
            HosasStickSide.Left => CommonUsages.LeftHand,
            HosasStickSide.Right => CommonUsages.RightHand,
            _ => throw new ArgumentOutOfRangeException(nameof(stickSide), stickSide, null)
        };
    }
}
