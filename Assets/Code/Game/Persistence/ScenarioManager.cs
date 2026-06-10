using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ScenarioManager : MonoBehaviour, IBind<ScenarioData>
{
    public SerializableGuid Id { get; set; }
    public List<Scenario> Scenarios = new();
    
    //[ShowInInspector] private ScenarioData scenarioData;
    public ScenarioData scenarioData;
    
    private const int Capacity = 3;

    private void Start()
    {
        Scenarios = new List<Scenario>(Capacity);
    }

    public void Bind(ScenarioData data)
    {
        Debug.Log($"[ScenarioManager] Bind ScenarioData: {data}");
        
        scenarioData = data;
        data.Id = Id;
        
        var isNew = scenarioData.Scenarios == null || scenarioData.Scenarios.Count == 0;
        
        if (isNew)
        {
            scenarioData.Scenarios = new List<Scenario>();
        }
        else
        {
            for (var i = 0; i < Capacity; i++)
            {
                if (Scenarios[i] == null) continue;
                
                scenarioData.Scenarios[i] = Scenarios[i];
            }
        }
        
        if (isNew && Scenarios.Count != 0)
        {
            for (var i = 0; i < Capacity; i++)
            {
                if (Scenarios[i] == null) continue;
                
                scenarioData.Scenarios[i] = Scenarios[i];
            }
        }
        
        Scenarios = scenarioData.Scenarios;
    }

    [Button]
    private void AddScenario()
    {
        Scenarios.Add(new Scenario($"{Scenarios.Count}"));
    }
    
    [Button]
    private void AddScenario2()
    {
        scenarioData.Scenarios.Add(new Scenario($"{scenarioData.Scenarios.Count}"));
    }
    
    [Button]
    private void DebugScenario()
    {
        Debug.Log($"Scenarios: {Scenarios.Count}");
        Debug.Log($"scenarioData.Scenarios: {scenarioData.Scenarios.Count}");
    }
}