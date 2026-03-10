using UnityEngine;

public class EntitySelectionTool : IPlannerTool
{
    private Camera camera;

    public Entity SelectedEntity { get; private set; }

    public EntitySelectionTool(Camera cam)
    {
        camera = cam;
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectEntity();
        }
    }

    private void TrySelectEntity()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hit))
        {
            var entity = hit.collider.GetComponent<Entity>();

            if (entity != null)
            {
                SelectedEntity = entity;
            }
        }
    }
}