using Code.Scripts.StateMachine;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Agent : ValidatedMonoBehaviour
{
    [field: HideInInspector, SerializeField, Self] public AgentCommandPath AgentCommandPath { get; private set; }
    [field: HideInInspector, SerializeField, Self] public NavMeshAgent NavMeshAgent { get; private set; }
    [field: HideInInspector, SerializeField, Self] public AgentStateMachine StateMachine { get; private set; }
    [field: HideInInspector, SerializeField, Self] public AgentHealth Health { get; private set; }
    
    public void Initialize()
    {
        Health.MaxHealth = 1f;
    }
}