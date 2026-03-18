using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public EntityFactory factory;
    public Camera cam;

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
        if (!selectedEntityData) return;

        var ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit))
        {
            factory.Spawn(selectedEntityData, hit.point, selectedThreat);
        }
    }
}