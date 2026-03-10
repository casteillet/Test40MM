using UnityEngine;

public class PathDrawingTool : IPlannerTool
{
    private Entity selectedEntity;
    private EntityPath currentEntityPath;
    private Camera camera;
    private const float waypointSpacing = .5f;

    public PathDrawingTool(Camera cam)
    {
        camera = cam;
    }

    public void SetEntity(Entity entity)
    {
        selectedEntity = entity;
    }

    public void HandleInput()
    {
        if (!selectedEntity) return;

        if (Input.GetMouseButtonDown(0))
        {
            currentEntityPath = new EntityPath();
        }

        if (Input.GetMouseButton(0))
        {
            AddWaypointFromMouse();
        }

        if (Input.GetMouseButtonUp(0))
        {
            FinishPath();
        }
    }

    private void AddWaypointFromMouse()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit)) return;

        var distance = Vector3.Distance(currentEntityPath.Waypoints[^1].Position, hit.point);
        if (currentEntityPath.Waypoints.Count == 0 || distance > waypointSpacing)
        {
            currentEntityPath.Waypoints.Add(new Waypoint(hit.point));
        }
    }

    private void FinishPath()
    {
        if (currentEntityPath == null) return;

        selectedEntity.PathFollower.SetPath(currentEntityPath);

        currentEntityPath = null;
    }
}