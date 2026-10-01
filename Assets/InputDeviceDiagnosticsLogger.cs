using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;

public class InputDeviceDiagnosticsLogger : MonoBehaviour
{
    private const string HidInterfaceName = "HID";

    private void OnEnable()
    {
        LogSupportedDevices();
        LogUnsupportedDevices();
        InputSystem.onDeviceChange += HandleDeviceChange;
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }

    private static void LogSupportedDevices()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            Debug.Log($"[Supported] layout={device.layout} {Describe(device.description)}");
        }
    }

    private static void LogUnsupportedDevices()
    {
        foreach (InputDeviceDescription description in InputSystem.GetUnsupportedDevices())
        {
            Debug.Log($"[Unsupported] {Describe(description)}");
        }
    }

    private static void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log($"[{change}] layout={device.layout} {Describe(device.description)}");
    }

    private static string Describe(InputDeviceDescription description)
    {
        string summary = $"interface={description.interfaceName} product=\"{description.product}\" manufacturer=\"{description.manufacturer}\"";

        if (description.interfaceName != HidInterfaceName || string.IsNullOrEmpty(description.capabilities)) return summary;

        HID.HIDDeviceDescriptor hidDescriptor = HID.HIDDeviceDescriptor.FromJson(description.capabilities);
        return $"{summary} vid=0x{hidDescriptor.vendorId:X4} pid=0x{hidDescriptor.productId:X4} usagePage={hidDescriptor.usagePage} usage={hidDescriptor.usage}";
    }
}
