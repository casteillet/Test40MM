using UnityEngine;

public class LookAtCommand : INonBlockingCommand
{
    private Agent agent;
    private Vector3 targetPosition;

    public LookAtCommand(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    public bool CanExecute(Agent agent) => true;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
    }

    public void Execute() { }

    public void Update()
    {
        var direction = targetPosition - agent.transform.position;
        direction.y = 0;
        
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, agent.NavMeshAgent.angularSpeed * Time.deltaTime);
    }

    public bool IsStillValid() => true;
    public bool IsCompleted() => false;
    public void Cancel() { }
}