using Unity.Netcode;
using UnityEngine;

public class PlayerViewBootstrapper : MonoBehaviour
{
    [SerializeField] private PlayerViewBinder viewBinder;
    [SerializeField] private SpectatorController spectator;

    private void Start()
    {
        var networkManager = NetworkManager.Singleton;
        if (!networkManager) return;

        var localClient = networkManager.LocalClient?.PlayerObject;
        if (localClient && localClient.TryGetComponent<PlayerView>(out var localView))
        {
            viewBinder.SetView(localView);        // player client or solo
        }
    }
}
