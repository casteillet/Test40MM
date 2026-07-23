using Unity.Netcode;
using UnityEngine;

public class NavigationUI : MonoBehaviour
{
    [SerializeField] private PlayerViewBinder binder;

    private NetworkObject target;

    private void OnEnable()
    {
        binder.OnNavigationChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (!binder) return;
        
        binder.OnNavigationChanged -= Refresh;
    }

    private void Refresh()
    {
        target = binder.TryGetNavigation(out var networkObject) ? networkObject : null;
    }
}
