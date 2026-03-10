using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    private Entity entity;
    private NavMeshAgent agent;

    private EntityPath currentEntityPath;
    private int currentWaypointIndex;

    private const float WaypointReachedDistance = .15f;

    // private void Awake()
    // {
    //     entity = GetComponent<Entity>();
    //     agent = GetComponent<NavMeshAgent>();
    // }

    public void SetPath(EntityPath entityPath)
    {
        currentEntityPath = entityPath;
        currentWaypointIndex = 0;

        if (currentEntityPath.Waypoints.Count > 0)
        {
            MoveToWaypoint();
        }
    }

    private void Update()
    {
        if (currentEntityPath == null) return;

        if (!agent.pathPending && agent.remainingDistance <= WaypointReachedDistance)
        {
            ReachWaypoint();
        }
    }

    private void MoveToWaypoint()
    {
        if (currentWaypointIndex >= currentEntityPath.Waypoints.Count) return;

        agent.SetDestination(currentEntityPath.Waypoints[currentWaypointIndex].Position);
    }

    private void ReachWaypoint()
    {
        var waypoint = currentEntityPath.Waypoints[currentWaypointIndex];

        ExecuteWaypointCommands(waypoint);

        currentWaypointIndex++;

        if (currentWaypointIndex < currentEntityPath.Waypoints.Count)
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