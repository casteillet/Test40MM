using UnityEngine;

public class QuickLookCommand : INonBlockingCommand
{
    private Agent agent;
    private Vector3 direction;
    private int waypointsToHold;
    private int waypointsCrossed;

    public QuickLookCommand(Vector3 direction, int waypointsToHold)
    {
        this.direction = direction.normalized;
        this.waypointsToHold = waypointsToHold;
    }

    public void NotifyWaypointCrossed() => waypointsCrossed++;
    private bool IsExpired => waypointsCrossed >= waypointsToHold;
    public bool CanExecute(Agent agent) => direction != Vector3.zero;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
        waypointsCrossed = 0;
    }

    public void Execute() { }

    public void Update()
    {
        if (IsExpired) return;
        
        direction.y = 0;
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, agent.NavMeshAgent.angularSpeed * Time.deltaTime);
    }

    public bool IsStillValid() => true;
    public bool IsCompleted() => IsExpired;
    public void Cancel() { }
}