using UnityEngine;

public class LockAimCommand : INonBlockingCommand
{
    private Entity entity;
    private Vector3 direction;

    public LockAimCommand(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    public bool CanExecute(Entity entity) => direction != Vector3.zero;

    public void Initialize(Entity entity)
    {
        this.entity = entity;
    }

    public void Execute() { }

    public void Update()
    {
        direction.y = 0;
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        entity.transform.rotation = Quaternion.RotateTowards(entity.transform.rotation, targetRotation, entity.NavMeshAgent.angularSpeed * Time.deltaTime);
    }

    public bool IsStillValid() => true;
    public bool IsCompleted() => false;
    public void Cancel() { }
}