using UnityEngine;

public class EntityFactory
{
    public Entity Spawn(EntityData data, Vector3 position, EntityThreat threat)
    {
        var go = Object.Instantiate(data.prefab, position, Quaternion.identity);

        var entity = go.GetComponent<Entity>();
        entity.Initialize(data, threat);

        ApplyData(entity, data);

        return entity;
    }

    private void ApplyData(Entity entity, EntityData data)
    {
        var health = entity.Health;
        if (health)
        {
            health.MaxHealth = data.maxHealth;
        }
    }
}