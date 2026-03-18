using System.Collections.Generic;

public class EntityPath
{
    public List<Waypoint> Waypoints { get; } = new();

    public void AddWaypoint(Waypoint waypoint)
    {
        Waypoints.Add(waypoint);
    }

    public void Clear()
    {
        Waypoints.Clear();
    }
}