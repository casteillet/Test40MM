using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData")]
public class EntityData : ScriptableObject
{
    [field: SerializeField] public SerializableGuid Id = Guid.NewGuid();
    
    [Header("Identity")]
    public string entityName;
    public Sprite icon;
    
    [Header("Prefab")]
    public GameObject prefab;

    [Header("Type")]
    public EntityType entityType;
    
    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3.5f;
    public float angularSpeed = 720f;
}
