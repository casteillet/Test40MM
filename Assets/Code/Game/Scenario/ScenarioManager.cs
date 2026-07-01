using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VInspector;

public class ScenarioManager : Singleton<ScenarioManager>, IBind<ScenarioData>
{
    public SerializableGuid Id { get; set; }

    private ScenarioData data;
    private Scenario currentScenario;

    public event Action<ScenarioData> OnScenarioDataChanged;
    public event Action<Scenario> OnCurrentScenarioChanged;

    public WeatherType CurrentWeather => currentScenario?.WeatherType ?? default;

    private void Start() // TODO: Move in OnEnable ? Need testing
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;

        var session = SessionManager.Instance;
        if (!session) return;

        foreach (var player in session.Players)
        {
            PushWeatherBaselineTo(player);
        }

        session.OnPlayerRegistered += PushWeatherBaselineTo;
    }

    private void OnDestroy()
    {
        if (SessionManager.Instance)
        {
            SessionManager.Instance.OnPlayerRegistered -= PushWeatherBaselineTo;
        }
    }

    public void Bind(ScenarioData data)
    {
        Debug.Log("[ScenarioManager] Bind ScenarioData");

        this.data = data;
        this.data.Id = Id;

        this.data.Scenarios = data.Scenarios ?? new List<Scenario>();

        OnScenarioDataChanged?.Invoke(this.data);
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

        NotifyCurrentScenarioChanged();

        Debug.Log($"[ScenarioManager] Load Scenario: {currentScenario.Name}");
    }

    public void SetGlobalWeather(WeatherType weather)
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;
        if (currentScenario == null) return;

        currentScenario.WeatherType = weather;

        foreach (var player in SessionManager.Instance.Players)
        {
            player.View.SetDefaultWeather(weather);
        }

        NotifyCurrentScenarioChanged();
    }

    public void SetNavigation(ulong clientId, NetworkObjectReference target)
    {
        if (!NetworkManager.Singleton || !NetworkManager.Singleton.IsServer) return;

        if (SessionManager.Instance.TryGetPlayer(clientId, out var player))
        {
            player.View.SetNavigation(target);
        }
    }

    private void Unload()
    {
        if (currentScenario != null)
        {
            Debug.Log($"[ScenarioManager] Unload currentScenario: {currentScenario.Name}");
        }
    }

    private void PushWeatherBaselineTo(Player player) => player.View.SetDefaultWeather(CurrentWeather);

    private void NotifyCurrentScenarioChanged() => OnCurrentScenarioChanged?.Invoke(currentScenario);

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
        data.Scenarios.RemoveAt(data.Scenarios.Count - 1);

        OnScenarioDataChanged?.Invoke(data);
    }
#endif
}
