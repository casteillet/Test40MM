using Code.Scripts.StateMachine;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Entity : ValidatedMonoBehaviour
{
    [field: HideInInspector, SerializeField, Self] public EntityCommandPath EntityCommandPath { get; private set; }
    [field: HideInInspector, SerializeField, Self] public NavMeshAgent NavMeshAgent { get; private set; }
    [field: HideInInspector, SerializeField, Self] public EntityStateMachine StateMachine { get; private set; }
    [field: HideInInspector, SerializeField, Self] public Health Health { get; private set; }
}