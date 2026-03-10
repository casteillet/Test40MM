using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    private Entity entity;
    private NavMeshAgent agent;

    private Path currentPath;
    private int currentWaypointIndex;

    private const float WaypointReachedDistance = .15f;

    // private void Awake()
    // {
    //     entity = GetComponent<Entity>();
    //     agent = GetComponent<NavMeshAgent>();
    // }

    public void SetPath(Path path)
    {
        currentPath = path;
        currentWaypointIndex = 0;

        if (currentPath.Waypoints.Count > 0)
        {
            MoveToWaypoint();
        }
    }

    private void Update()
    {
        if (currentPath == null) return;

        if (!agent.pathPending && agent.remainingDistance <= WaypointReachedDistance)
        {
            ReachWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= currentPath.Waypoints.Count) return;

        agent.SetDestination(currentPath.Waypoints[currentWaypointIndex].Position);
    }

    private void ReachWaypoint()
    {
        var waypoint = currentPath.Waypoints[currentWaypointIndex];

        ExecuteWaypointCommands(waypoint);

        currentWaypointIndex++;

        if (currentWaypointIndex < currentPath.Waypoints.Count)
        {
            MoveToWaypoint();
        }
    }

    private void ExecuteWaypointCommands(Waypoint waypoint)
    {
        foreach (var command in waypoint.Commands)
        {
            entity.CommandInvoker.Enqueue(command);
        }
    }
}