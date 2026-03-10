using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public EntityFactory factory;

    private EntityData selectedEntityData;
    private EntityThreat selectedThreat;

    public void SelectEntity(EntityData data, EntityThreat threat)
    {
        selectedEntityData = data;
        selectedThreat = threat;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        if (selectedEntityData == null) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            factory.Spawn(selectedEntityData, hit.point, selectedThreat);
        }
    }
}