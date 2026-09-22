using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScenarioData
{
    public List<Scenario> Scenarios;
}

[Serializable]
public class Scenario
{
    [field: SerializeField] public SerializableGuid Id;
    public string Name;
    public WeatherType WeatherType;

    public Scenario(string scenarioName)
    {
        Id = SerializableGuid.NewGuid();
        Name = scenarioName;
    }
}
