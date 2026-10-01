using UnityEngine;

public class HosasStickIndicatorPresenter : MonoBehaviour
{
    [SerializeField] private HosasStickSide _stickSide;
    [SerializeField] private ConnectionIndicatorView _indicatorView;

    private void OnEnable()
    {
        HosasConnectionMonitor.StickConnectionChanged += HandleStickConnectionChanged;
        _indicatorView.SetConnected(IsStickCurrentlyConnected());
    }

    private void OnDisable()
    {
        HosasConnectionMonitor.StickConnectionChanged -= HandleStickConnectionChanged;
    }

    private bool IsStickCurrentlyConnected()
    {
        HosasConnectionMonitor monitor = HosasConnectionMonitor.Instance;

        if (monitor == null)
        {
            Debug.LogWarning($"{nameof(HosasConnectionMonitor)} is missing from the scene; {_stickSide} stick indicator will stay disconnected.", this);
            return false;
        }

        return monitor.IsConnected(_stickSide);
    }

    private void HandleStickConnectionChanged(HosasStickSide stickSide, bool isConnected)
    {
        if (stickSide != _stickSide) return;

        _indicatorView.SetConnected(isConnected);
    }
}
