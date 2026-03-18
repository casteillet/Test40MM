using UnityEngine;

public class PathDrawingTool : IPlannerTool
{
    private Entity selectedEntity;
    private EntityPath currentEntityPath;
    private Camera camera;

    public PathDrawingTool(Camera cam)
    {
        camera = cam;
    }

    public void SetEntity(Entity entity)
    {
        selectedEntity = entity;
        currentEntityPath = new EntityPath();
    }

    public void HandleInput()
    {
        if (!selectedEntity) return;

        if (Input.GetMouseButtonDown(0))
        {
            AddWaypointFromMouse();
        }

        if (Input.GetMouseButtonDown(1))
        {
            FinishPath();
        }
    }

    private void AddWaypointFromMouse()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit)) return;

        currentEntityPath.Waypoints.Add(new Waypoint(hit.point));
    }

    private void FinishPath()
    {
        if (currentEntityPath == null) return;

        selectedEntity.PathFollower.SetPath(currentEntityPath);

        currentEntityPath = new EntityPath();
    }
}