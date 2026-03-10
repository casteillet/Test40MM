using UnityEngine;

public class EntityFactory
{
    public Entity Spawn(EntityData data, Vector3 position, EntityFaction faction)
    {
        var obj = Object.Instantiate(data.prefab, position, Quaternion.identity);
        var entity = obj.GetComponent<Entity>();
        entity.Initialize(faction);
        return entity;
    }
}