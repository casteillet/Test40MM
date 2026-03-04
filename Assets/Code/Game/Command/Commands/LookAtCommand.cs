using UnityEngine;

public class LookAtCommand : INonBlockingCommand
{
    private Entity entity;
    private Vector3 targetPosition;

    public LookAtCommand(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public bool CanExecute(Entity entity) => true;

    public void Initialize(Entity entity)
    {
        this.entity = entity;
    }

    public void Execute() { }

    public void Update()
    {
        var direction = targetPosition - entity.transform.position;
        direction.y = 0;
        
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        entity.transform.rotation = Quaternion.RotateTowards(entity.transform.rotation, targetRotation, entity.NavMeshAgent.angularSpeed * Time.deltaTime);
    }

    public bool IsStillValid() => true;
    public bool IsCompleted() => false;
    public void Cancel() { }
}