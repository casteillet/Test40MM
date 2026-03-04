using UnityEngine;

[CreateAssetMenu(menuName = "Game/Agent Preset Data")]
public class AgentPresetDataSO : ScriptableObject
{
    [Header("Base Data")]
    public AgentDataSO BaseData;
    public AgentVisualDataSO Visuals;
}

