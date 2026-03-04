using UnityEngine;

[CreateAssetMenu(menuName = "Game/Agent Data")]
public class AgentDataSO : ScriptableObject
{
    public enum EAgentArchetype { Medic, Engineer, Scout, Heavy, Hacker }
    public EAgentArchetype AgentArchetype;
    public GameObject AgentPrefab;
    public GameObject AgentPreviewPrefab;
    public int AgentMaxHealth = 100;
    
    public GameObject AgentUIPrefab;
}

