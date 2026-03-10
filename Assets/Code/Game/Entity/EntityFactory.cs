using UnityEngine;

public class EntityFactory
{
    public Entity Spawn(EntityData data, Vector3 position, EntityThreat threat)
    {
        var go = Object.Instantiate(data.prefab, position, Quaternion.identity);

        var entity = go.GetComponent<Entity>();
        entity.Initialize(threat);

        ApplyData(entity, data);

        return entity;
    }

    private void ApplyData(Entity entity, EntityData data)
    {
        var navMeshAgent = entity.NavMeshAgent;
        if (navMeshAgent)
        {
            navMeshAgent.speed = data.moveSpeed;
            navMeshAgent.angularSpeed = data.angularSpeed;
        }

        var health = entity.Health;
        if (health)
        {
            health.MaxHealth = data.maxHealth;
        }
    }
}