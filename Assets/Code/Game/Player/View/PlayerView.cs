using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerView : NetworkBehaviour, IPlayerView
{
    private readonly NetworkVariable<bool> weatherOverridden = new(false, NetworkVariableReadPermission.Owner);
    private readonly NetworkVariable<WeatherType> weather = new(default, NetworkVariableReadPermission.Owner);
    private OverridableField<WeatherType> weatherField;

    private readonly NetworkVariable<NetworkObjectReference> target = new(default, NetworkVariableReadPermission.Owner);

    private readonly NetworkVariable<int> ammoToFire = new(0, NetworkVariableReadPermission.Owner);
    
    // IWeatherView
    public WeatherType Weather => weatherField?.Effective ?? default;
    public bool IsWeatherOverridden => weatherField is { IsOverridden: true };
    public event Action<WeatherType> OnWeatherChanged;

    // ITargetView
    public bool TryGetNavigation(out NetworkObject navigation) => target.Value.TryGet(out navigation);
    public event Action OnNavigationChanged;

    // IWeaponView
    public int AmmoToFire => ammoToFire.Value;
    public event Action<int> OnAmmoToFireChanged;

    public override void OnNetworkSpawn()
    {
        weatherField = new OverridableField<WeatherType>(weather, weatherOverridden);
        weatherField.OnEffectiveChanged += HandleWeatherChanged;
        
        target.OnValueChanged += HandleNavigationChanged;
        
        ammoToFire.OnValueChanged += HandleAmmoToFireChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (weatherField != null)
        {
            weatherField.OnEffectiveChanged -= HandleWeatherChanged;
            weatherField.Dispose();
            weatherField = null;
        }
        
        target.OnValueChanged -= HandleNavigationChanged;
        
        ammoToFire.OnValueChanged -= HandleAmmoToFireChanged;
    }

    public void SetDefaultWeather(WeatherType value)
    {
        if (!IsServer) return;
        weatherField.SetBaseline(value);
    }

    public void SetNavigation(NetworkObjectReference enemy)
    {
        if (!IsServer) return;
        target.Value = enemy;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestWeatherOverrideRpc(WeatherType value) => weatherField.ApplyOverride(value);

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Owner)]
    public void RequestAmmoToFireRpc(int value) => ammoToFire.Value = Mathf.Max(0, value);

    private void HandleWeatherChanged(WeatherType value) => OnWeatherChanged?.Invoke(value);
    private void HandleNavigationChanged(NetworkObjectReference _, NetworkObjectReference __) => OnNavigationChanged?.Invoke();
    private void HandleAmmoToFireChanged(int _, int current) => OnAmmoToFireChanged?.Invoke(current);
}