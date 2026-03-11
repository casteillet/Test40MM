using KBCore.Refs;
using UnityEngine;
using VInspector;

[SelectionBase]
public class Entity : ValidatedMonoBehaviour
{
    public EntityData EntityData { get; private set; }
    public CommandInvoker CommandInvoker { get; private set; }

    [field: SerializeField, Self] public PathFollower PathFollower { get; private set; }
    [field: SerializeField, Self] public Health Health { get; private set; }
    [field: SerializeField, ReadOnly] public EntityThreat Threat { get; private set; }

    public void Initialize(EntityData data, EntityThreat threat)
    {
        EntityData = data;
        Threat = threat;
        CommandInvoker = new CommandInvoker(this);
    }

    public void SetFaction(EntityThreat threat)
    {
        Threat = threat;
    }
}