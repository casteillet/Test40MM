using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;

public class DeviceConnectionIndicatorPresenter : MonoBehaviour
{
    [InputControl]
    [SerializeField] private string _deviceControlPath = "<Joystick>{RightHand}";
    [SerializeField] private ConnectionIndicatorView _indicatorView;

    private void OnEnable()
    {
        InputSystem.onDeviceChange += HandleDeviceChange;
        RefreshIndicator();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= HandleDeviceChange;
    }

    private void HandleDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Removed:
            case InputDeviceChange.Reconnected:
            case InputDeviceChange.Disconnected:
            case InputDeviceChange.UsageChanged:
                RefreshIndicator();
                break;
        }
    }

    private void RefreshIndicator()
    {
        bool isDeviceConnected = InputSystem.FindControl(_deviceControlPath) != null;
        _indicatorView.SetConnected(isDeviceConnected);
    }
}