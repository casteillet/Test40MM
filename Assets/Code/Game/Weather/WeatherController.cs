using BennyKok.RuntimeDebug.Actions;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Systems;
using Unity.Netcode;
using UnityEngine;

public class WeatherController : NetworkBehaviour
{
    private PlayerView view;
    
    private void OnEnable()
    {
        if (!view)
        {
            GetLocalPlayerView();
        }
        
        view.OnWeatherChanged += HandleWeatherChanged;
        SetWeather(view.Weather);
    }

    private void OnDisable()
    {
        view.OnWeatherChanged -= HandleWeatherChanged;
    }

    private void Start()
    {
        GetLocalPlayerView();
    }

    private void GetLocalPlayerView()
    {
        var local = NetworkManager.LocalClient?.PlayerObject;
        local?.TryGetComponent(out view);
    }

    private void HandleWeatherChanged(WeatherType weather) => SetWeather(weather);

    private void SetWeather(WeatherType weather)
    {
        Debug.Log($"Weather set to {weather}");
    }
    
    public void OverrideWeather(WeatherType weather)
    {
        view.RequestWeatherOverrideRpc(weather);
    }
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private BaseDebugAction[] actions;

    private void Awake()
    {
        actions = RuntimeDebugSystem.RegisterActionsAuto(this);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        RuntimeDebugSystem.UnregisterActions(actions);
    }
    
    [DebugAction] public void TestClientOverrideWeather(int weather) => OverrideWeather((WeatherType)weather);
#endif
}
