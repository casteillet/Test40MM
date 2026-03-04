using UnityEngine;

[CreateAssetMenu(menuName = "Game/Agent Visuals Data")]
public class AgentVisualDataSO : ScriptableObject
{
    public Mesh headMesh;
    public Material bodyMaterial;
    public RuntimeAnimatorController animatorController;
}