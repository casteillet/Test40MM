using UnityEngine;
using UnityEngine.UI;

public class ConnectionIndicatorView : MonoBehaviour
{
    [SerializeField] private Image _indicatorImage;
    [SerializeField] private Color _connectedColor = Color.green;
    [SerializeField] private Color _disconnectedColor = Color.red;

    public void SetConnected(bool isConnected)
    {
        _indicatorImage.color = isConnected ? _connectedColor : _disconnectedColor;
    }
}
