using System;
using System.Collections.Generic;
using BennyKok.RuntimeDebug.Actions;
using BennyKok.RuntimeDebug.Attributes;
using BennyKok.RuntimeDebug.Systems;
using Unity.Netcode;
using UnityEngine;
using VInspector;

public class ScenarioManager : Singleton<ScenarioManager>, IBind<ScenarioData>
{
    public SerializableGuid Id { get; set; }

    private ScenarioData data;
    private Scenario currentScenario;
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    private BaseDebugAction[] actions;
#endif
    
    public event Action<ScenarioData> OnScenarioDataChanged;
    
    public event Action<Scenario> OnScenarioSaved;
    public event Action<Scenario> OnScenarioLoaded;
    public event Action<Scenario> OnScenarioDeleted;
    
    public event Action OnScenarioUpdated;

    public WeatherType CurrentWeather => currentScenario?.WeatherType ?? default;

    private void Start() // TODO: Move in OnEnable ? Need testing
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;

        var session = SessionManager.Instance;
        if (!session) return;

        foreach (var player in session.Players)
        {
            PushDefaultWeather(player);
        }

        session.OnPlayerRegistered += PushDefaultWeather;
        
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        actions = RuntimeDebugSystem.RegisterActionsAuto(this);
#endif
    }
    
    private void OnDestroy()
    {
        if (SessionManager.Instance)
        {
            SessionManager.Instance.OnPlayerRegistered -= PushDefaultWeather;
        }
        
        
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        RuntimeDebugSystem.UnregisterActions(actions);
#endif
    }

    public void Bind(ScenarioData data)
    {
        Debug.Log("[ScenarioManager] Bind ScenarioData");

        this.data = data;
        this.data.Id = Id;

        this.data.Scenarios = data.Scenarios ?? new List<Scenario>();

        OnScenarioDataChanged?.Invoke(this.data);
        
        // TODO: Try load first scenario if it exist ?
    }

    public void Load(Scenario scenario)
    {
        Unload();

        currentScenario = scenario;

        if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
        {
            foreach (var player in SessionManager.Instance.Players)
            {
                player.View.SetDefaultWeather(currentScenario.WeatherType);
            }
        }

        OnScenarioLoaded?.Invoke(currentScenario);
        Debug.Log($"[ScenarioManager] Load Scenario: {currentScenario.Name}");
    }

    public void SetGlobalWeather(WeatherType weather)
    {
        Debug.Log($"NetworkManager.Singleton: {NetworkManager.Singleton}");
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;
        if (currentScenario == null) return;

        currentScenario.WeatherType = weather;
        
        foreach (var player in SessionManager.Instance.Players)
        {
            player.View.SetDefaultWeather(weather);
        }
        
        OnScenarioUpdated?.Invoke();
    }

    public void SetNavigation(ulong clientId, NetworkObjectReference target)
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;

        if (SessionManager.Instance.TryGetPlayer(clientId, out var player))
        {
            player.View.SetNavigation(target);
        }
        
        OnScenarioUpdated?.Invoke();
    }

    private void Unload()
    {
        if (currentScenario != null)
        {
            Debug.Log($"[ScenarioManager] Unload currentScenario: {currentScenario.Name}");
        }
    }

    private void PushDefaultWeather(Player player) => player.View.SetDefaultWeather(CurrentWeather);

#if UNITY_EDITOR
    [Button]
    private void TestAddScenario()
    {
        data.Scenarios.Add(
            new Scenario($"{data.Scenarios.Count}")
        );

        OnScenarioDataChanged?.Invoke(data);
    }

    [Button]
    private void TestRemoveLastScenario()
    {
        OnScenarioDeleted?.Invoke(data.Scenarios[^1]); // TEST ONLY
        
        data.Scenarios.RemoveAt(data.Scenarios.Count - 1);

        OnScenarioDataChanged?.Invoke(data);
    }
#endif
    
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [DebugAction] public void TestServerSetGlobalWeather(int weatherType) => SetGlobalWeather((WeatherType)weatherType);
#endif
}
