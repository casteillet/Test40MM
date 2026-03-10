using UnityEngine;

public class CommandPlacementTool : IPlannerTool
{
    private Camera camera;

    public CommandPlacementTool(Camera cam)
    {
        camera = cam;
    }

    public void HandleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            TryPlaceCommand();
        }
    }

    private void TryPlaceCommand()
    {
        var ray = camera.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit)) return;

        var waypoint = FindClosestWaypoint(hit.point);

        if (waypoint == null) return;

        waypoint.AddCommand(new ChangeFactionCommand(EntityThreat.Neutral));
    }

    private Waypoint FindClosestWaypoint(Vector3 position)
    {
        // Simplified placeholder logic
        return null;
    }
}