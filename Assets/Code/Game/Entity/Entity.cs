using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Entity : ValidatedMonoBehaviour
{
    [field: SerializeField, Self] public EntityThreat Threat { get; private set; }
    [field: SerializeField, Self] public NavMeshAgent NavMeshAgent { get; private set; }
    [field: SerializeField, Self] public PathFollower PathFollower { get; private set; }
    [field: SerializeField, Self] public Health Health { get; private set; }
    public CommandInvoker CommandInvoker { get; private set; }

    public void Initialize(EntityThreat threat)
    {
        CommandInvoker = new CommandInvoker(this);
        Threat = threat;
    }

    public void SetFaction(EntityThreat threat)
    {
        Threat = threat;
    }
}