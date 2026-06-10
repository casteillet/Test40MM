using System.Collections.Generic;
using UnityEngine;

public static class EntityDatabase
{
    static Dictionary<SerializableGuid, EntityData> entityDatasById;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void Initialize()
    {
        entityDatasById = new Dictionary<SerializableGuid, EntityData>();

        var entityDatas = Resources.LoadAll<EntityData>("");
        foreach (var entity in entityDatas)
        {
            entityDatasById.Add(entity.Id, entity);
        }
    }

    public static EntityData GetDataById(SerializableGuid id)
    {
        try
        {
            return entityDatasById[id];
        }
        catch
        {
            Debug.LogError($"Cannot find entity datas with id {id}");
            return null;
        }
    }
}