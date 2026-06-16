using System.Collections.Generic;
using UnityEngine;
using VInspector;

public class ScenarioManager : MonoBehaviour, IBind<ScenarioData>
{
    public SerializableGuid Id { get; set; }
    
    [SerializeField] private ScenarioData scenarioData;

    public void Bind(ScenarioData data)
    {
        Debug.Log("[ScenarioManager] Bind ScenarioData");
        
        scenarioData = data;
        scenarioData.Id = Id;

        scenarioData.Scenarios = data.Scenarios ?? new List<Scenario>();
    }

#if UNITY_EDITOR
    [Button]
    private void TestAddScenario()
    {
        scenarioData.Scenarios.Add(
            new Scenario($"b{scenarioData.Scenarios.Count}")
        );
    }
    
    [Button]
    private void TestClearScenario()
    {
        scenarioData.Scenarios.Clear();
    }
#endif
}