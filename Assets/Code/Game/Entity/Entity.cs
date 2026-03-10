using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Entity : ValidatedMonoBehaviour
{
    public EntityFaction Faction { get; private set; }

    public NavMeshAgent Agent { get; private set; }
    public PathFollower PathFollower { get; private set; }
    public CommandInvoker CommandInvoker { get; private set; }
    //[field: HideInInspector, SerializeField, Self] public Health Health { get; private set; }

    public void Initialize(EntityFaction faction)
    {
        Faction = faction;
    }

    public void SetFaction(EntityFaction faction)
    {
        Faction = faction;
    }
}