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
        NotifyCurrentScenarioChanged();
        
        Debug.Log($"[ScenarioManager] Load Scenario: {currentScenario.Name}");
    }

    [ClientRpc]
    public void SendScenarioDataToClients(Scenario scenario)
    {
        
    } 

    private void Unload()
    {
        if (currentScenario != null)
        {
            Debug.Log($"[ScenarioManager] Unload currentScenario: {currentScenario.Name}");
        }
    }
                                                                                             
    public void SetWeather(int weather)
    {
        currentScenario.WeatherType = (WeatherType)weather;
        NotifyCurrentScenarioChanged();
    }
    // public void SetWeather(WeatherType weather)
    // {
    //     CurrentScenario.WeatherType = weather;
    //     NotifyCurrentScenarioChanged();
    // }

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