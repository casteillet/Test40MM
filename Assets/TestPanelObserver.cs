using UnityEngine;

public class TestPanelObserver : MonoBehaviour, IPanelObserver
{
    public void OnPanelActivated()
    {
        Debug.Log("Panel Activated", this);
    }

    public void OnPanelDeactivated()
    {
        Debug.Log("Panel Deactivated", this);
    }
}
