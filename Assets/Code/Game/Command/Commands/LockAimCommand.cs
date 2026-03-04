using UnityEngine;

public class LockAimCommand : INonBlockingCommand
{
    private Agent agent;
    private Vector3 direction;

    public LockAimCommand(Vector3 direction)
    {
        this.direction = direction.normalized;
    }

    public bool CanExecute(Agent agent) => direction != Vector3.zero;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
    }

    public void Execute() { }

    public void Update()
    {
        direction.y = 0;
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, agent.NavMeshAgent.angularSpeed * Time.deltaTime);
    }

    public bool IsStillValid() => true;
    public bool IsCompleted() => false;
    public void Cancel() { }
}