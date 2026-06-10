using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ScenarioManager : MonoBehaviour, IBind<ScenarioData>
{
    public SerializableGuid Id { get; set; }
    
    private ScenarioData scenarioData = new();
    
    public void Bind(ScenarioData data)
    {
        Debug.Log($"[ScenarioManager] Bind: {data}, {data.Id}");
        foreach (var scenario in data.Scenarios)
        {
            Debug.Log($"{scenario.Name}: {scenario.Id}");
        }
        
        ////////////////////////////////
        
        scenarioData = data;
            
        var isNew = scenarioData.Scenarios == null || scenarioData.Scenarios.Count == 0;

        if (isNew)
        {
            scenarioData.Scenarios = new List<Scenario>();
        }
    }

    [Button]
    private void AddScenario()
    {
        scenarioData.Scenarios.Add(new Scenario(scenarioData.Scenarios.Count.ToString()));
    }
}