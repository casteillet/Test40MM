using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;

public static class HosasDeviceIdentifier
{
    private const string HidInterfaceName = "HID";
    private const int ThrustmasterVendorId = 0x044F;

    private const int SolRRightStickProductId = 0x0420;
    private const int SolRLeftStickProductId = 0x0428;

    private static readonly Dictionary<int, HosasStickSide> StickSideByProductId = new()
    {
        { SolRRightStickProductId, HosasStickSide.Right },
        { SolRLeftStickProductId, HosasStickSide.Left },
    };

    public static bool TryIdentifyStickSide(InputDevice device, out HosasStickSide stickSide)
    {
        stickSide = default;

        var description = device.description;

        if (description.interfaceName != HidInterfaceName) return false;
        if (string.IsNullOrEmpty(description.capabilities)) return false;

        var hidDescriptor = HID.HIDDeviceDescriptor.FromJson(description.capabilities);

        if (hidDescriptor.vendorId != ThrustmasterVendorId) return false;

        return StickSideByProductId.TryGetValue(hidDescriptor.productId, out stickSide);
    }
}