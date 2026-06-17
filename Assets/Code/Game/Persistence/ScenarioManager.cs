using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ScenarioManager : Singleton<ScenarioManager>, IBind<ScenarioData>
{
    public SerializableGuid Id { get; set; }
    public ScenarioData Data { get; private set; }
    
    private Scenario currentScenario;
    
    public event Action<ScenarioData> OnScenarioDataChanged;

    public void Bind(ScenarioData data)
    {
        Debug.Log("[ScenarioManager] Bind ScenarioData");
        
        Data = data;
        Data.Id = Id;

        Data.Scenarios = data.Scenarios ?? new List<Scenario>();
        
        OnScenarioDataChanged?.Invoke(Data);
    }

    public void Load(Scenario scenario)
    {
        Unload();
        
        currentScenario = scenario;
        Debug.Log($"[ScenarioManager] Load Scenario: {currentScenario.Name}");
    }

    private void Unload()
    {
        if (currentScenario != null)
        {
            Debug.Log($"[ScenarioManager] Unload currentScenario: {currentScenario.Name}");
        }
        
        currentScenario = null;
    }

#if UNITY_EDITOR
    [Button]
    private void TestAddScenario()
    {
        Data.Scenarios.Add(
            new Scenario($"{Data.Scenarios.Count}")
        );
        
        OnScenarioDataChanged?.Invoke(Data);
    }
    
    [Button]
    private void TestRemoveLastScenario()
    {
        Data.Scenarios.RemoveAt(Data.Scenarios.Count - 1);
        
        OnScenarioDataChanged?.Invoke(Data);
    }
#endif
}