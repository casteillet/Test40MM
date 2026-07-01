using System;
using Unity.Netcode;

public interface INavigationView
{
    bool TryGetNavigation(out NetworkObject navigation); // TODO: Update with navigation info
    event Action OnNavigationChanged;
}
