using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerViewBinder : MonoBehaviour, IPlayerView
{
    private IPlayerView source;

    // IWeatherView
    public WeatherType Weather => source?.Weather ?? default;
    public bool IsWeatherOverridden => source is { IsWeatherOverridden: true };
    public event Action<WeatherType> OnWeatherChanged;

    // ITargetView
    public bool TryGetNavigation(out NetworkObject navigation)
    {
        if (source != null) return source.TryGetNavigation(out navigation);
        
        navigation = null;
        return false;
    }
    public event Action OnNavigationChanged;

    // IWeaponView
    public int AmmoToFire => source?.AmmoToFire ?? 0;
    public event Action<int> OnAmmoToFireChanged;

    public void SetView(IPlayerView newSource)
    {
        UnsubscribeView(source);
        source = newSource;
        SubscribeView(source);
        
        OnWeatherChanged?.Invoke(Weather);
        OnNavigationChanged?.Invoke();
        OnAmmoToFireChanged?.Invoke(AmmoToFire);
    }
    
    private void UnsubscribeView(IPlayerView oldView)
    {
        if (oldView == null) return;
        
        oldView.OnWeatherChanged -= HandleWeatherChanged;
        oldView.OnNavigationChanged -= HandleNavigationChanged;
        oldView.OnAmmoToFireChanged -= HandleAmmoToFireChanged;
    }
    
    private void SubscribeView(IPlayerView newView)
    {
        if (newView == null) return;
        
        newView.OnWeatherChanged += HandleWeatherChanged;
        newView.OnNavigationChanged += HandleNavigationChanged;
        newView.OnAmmoToFireChanged += HandleAmmoToFireChanged;
    }

    public void Clear() => SetView(null);

    private void HandleWeatherChanged(WeatherType value) => OnWeatherChanged?.Invoke(value);
    private void HandleNavigationChanged() => OnNavigationChanged?.Invoke();
    private void HandleAmmoToFireChanged(int value) => OnAmmoToFireChanged?.Invoke(value);
}
