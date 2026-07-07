using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Components;
using Unity.Netcode;
using UnityEngine;

public class SpectatorController : RuntimeDebugBehaviour
{
    [SerializeField] private PlayerViewBinder binder;

    private ulong? spectatedClientId;

    public bool TrySpectate(ulong clientId) // TODO: Link with button int a SpectatorControllerUI
    {
        if (!NetworkManager.Singleton.IsServer) return false;
        
        if (!SessionManager.Instance.TryGetPlayer(clientId, out var player)) return false;

        spectatedClientId = clientId;
        binder.SetView(player.View);
        return true;
    }
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [DebugAction] public void TestSpectateFirst(ulong clientId) => TrySpectate(clientId);
#endif
    // TODO: Handle an edge case where the current spectating player disconnect, then go back to another panel and clear the binder
}
