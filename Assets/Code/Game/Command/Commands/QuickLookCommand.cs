using UnityEngine;

public class QuickLookCommand : INonBlockingCommand
{
    private Entity entity;
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
    public bool CanExecute(Entity entity) => direction != Vector3.zero;

    public void Initialize(Entity entity)
    {
        this.entity = entity;
        waypointsCrossed = 0;
    }

    public void Execute() { }

    public void Update()
    {
        if (IsExpired) return;
        
        direction.y = 0;
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        entity.transform.rotation = Quaternion.RotateTowards(entity.transform.rotation, targetRotation, entity.NavMeshAgent.angularSpeed * Time.deltaTime);
    }

    public bool IsStillValid() => true;
    public bool IsCompleted() => IsExpired;
    public void Cancel() { }
}