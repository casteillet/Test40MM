using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    private Entity entity;

    private EntityPath currentPath;
    private List<Vector3> controlPoints = new();

    private int currentSegment;
    private float t;

    private float moveSpeed;

    public void Initialize(Entity entity)
    {
        this.entity = entity;
        moveSpeed = entity.EntityData.moveSpeed;
    }

    public void SetPath(EntityPath path)
    {
        if (path.Waypoints.Count < 2) return;

        currentPath = path;

        controlPoints.Clear();
        foreach (var waypoint in path.Waypoints)
        {
            controlPoints.Add(waypoint.Position);
        }

        currentSegment = 0;
        t = 0f;
    }

    private void Update()
    {
        if (currentPath == null) return;

        MoveAlongSpline();
    }

    private void MoveAlongSpline()
    {
        if (currentSegment >= controlPoints.Count - 1) return;

        t += Time.deltaTime * moveSpeed;

        if (t >= 1f)
        {
            ReachWaypoint();

            t = 0f;
            currentSegment++;

            if (currentSegment >= controlPoints.Count - 1) return;
        }

        var position = GetCatmullRomPosition(currentSegment, t);
        
        transform.position = position;

        var nextPosition = GetCatmullRomPosition(currentSegment, t + 0.01f);
        var direction = (nextPosition - position).normalized;
        
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void ReachWaypoint()
    {
        var waypoint = currentPath.Waypoints[currentSegment];
        ExecuteWaypointCommands(waypoint);
    }

    private void ExecuteWaypointCommands(Waypoint waypoint)
    {
        foreach (var command in waypoint.Commands)
        {
            entity.CommandInvoker.Enqueue(command);
        }
    }

    private Vector3 GetCatmullRomPosition(int i, float t)
    {
        var p0 = controlPoints[Mathf.Clamp(i - 1, 0, controlPoints.Count - 1)];
        var p1 = controlPoints[i];
        var p2 = controlPoints[Mathf.Clamp(i + 1, 0, controlPoints.Count - 1)];
        var p3 = controlPoints[Mathf.Clamp(i + 2, 0, controlPoints.Count - 1)];

        var t2 = t * t;
        var t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
}