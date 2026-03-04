using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovePathCommand : IInterruptibleCommand
{
    private Agent agent;
    private List<Waypoint> waypoints;
    private CommandInvoker internalInvoker;
    private NavMeshAgent navMeshAgent;
    
    private bool isPaused;
    private int currentIndex;
    private Vector3 targetPosition;
    private bool movingToNext;

    private const int LookAheadWaypoint = 6;

#if UNITY_EDITOR
    public CommandInvoker InternalInvoker => internalInvoker;
#endif

    public MovePathCommand(List<Waypoint> waypoints)
    {
        this.waypoints = waypoints;
    }
    
    public bool CanExecute(Agent agent) => waypoints != null && waypoints.Count != 0;

    public void Initialize(Agent agent)
    {
        this.agent = agent;
        navMeshAgent =  agent.NavMeshAgent;
        internalInvoker = new CommandInvoker(agent);
        currentIndex = -1;
    }
    
    public void Execute()
    {
        navMeshAgent.isStopped = false;
        MoveToNextPosition();
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        
        if (isPaused) return;
        
        internalInvoker.Update();
        
        if (!internalInvoker.HasCurrentCommand && navMeshAgent.isStopped)
        {
            navMeshAgent.isStopped = false;
        }
        
        if (internalInvoker.TryGetCurrentCommand(out var blockingCommand) && blockingCommand is not INonBlockingCommand) return;
        
        if (movingToNext)
        {
            var direction = targetPosition - navMeshAgent.transform.position;
            var distance = direction.magnitude;
        
            if (distance <= navMeshAgent.stoppingDistance)
            {
                navMeshAgent.Move(direction);
                ExecuteWaypointCommands();
                movingToNext = false;

                if (currentIndex == waypoints.Count - 1)
                {
                    currentIndex++;    
                }
            }
            else
            {
                var movement = direction.normalized * (navMeshAgent.speed * Time.deltaTime);
                navMeshAgent.Move(movement);
            }
        }
        else if (CanAdvanceToNextWaypoint())
        {
            if (currentIndex < waypoints.Count - 1 || agent.AgentCommandPath.EditMode != PathEditMode.None)
            {
                MoveToNextPosition();
            }
        }

        if (!internalInvoker.HasCurrentCommand || (internalInvoker.TryGetCurrentCommand(out var command) && command.IsCompleted()))
        {
            ApplyDefaultLookAheadRotation();
        }
    }
    
    private void ExecuteWaypointCommands()
    {
        var currentWaypoint = waypoints[currentIndex];

        if (internalInvoker.TryGetCurrentCommand(out var command) && command is QuickLookCommand quickLookCommand)
        {
            quickLookCommand.NotifyWaypointCrossed();
        }

        if (currentWaypoint.TryGetCommand(out var waypointCommand))
        {
            internalInvoker.Enqueue(waypointCommand);

            if (waypointCommand is not INonBlockingCommand)
            {
                navMeshAgent.isStopped = true;
            }
        }
    }
    
    private bool CanAdvanceToNextWaypoint() => !internalInvoker.HasCurrentCommand || internalInvoker.CurrentCommand is INonBlockingCommand;

    private void ApplyDefaultLookAheadRotation()
    {
        if (waypoints == null || waypoints.Count == 0 || currentIndex >= waypoints.Count) return;

        var lookAheadIndex = Mathf.Min(currentIndex + LookAheadWaypoint, waypoints.Count - 1);
        var lookTarget = waypoints[lookAheadIndex].Position;
        
        var direction = lookTarget - agent.transform.position;
        direction.y = 0;
        if (direction == Vector3.zero) return;
        
        var targetRotation = Quaternion.LookRotation(direction);
        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation, targetRotation, navMeshAgent.angularSpeed * Time.deltaTime);
    }

    private void MoveToNextPosition()
    {
        if (currentIndex + 1 >= waypoints.Count) return;
    
        currentIndex++;
        targetPosition = waypoints[currentIndex].Position;
        movingToNext = true;
    }
    
    public bool IsStillValid()
    {
        if (currentIndex < waypoints.Count)
        {
            if (waypoints[currentIndex].TryGetCommand(out var command))
            {
                if (command != null && !command.IsStillValid())
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    public bool IsCompleted()
    {
        if (agent.AgentCommandPath.EditMode != PathEditMode.None) return false;
        return currentIndex >= waypoints.Count && !internalInvoker.HasCurrentCommand;
    }

    public void Pause()
    {
        isPaused = true;
        navMeshAgent.isStopped = true;
        internalInvoker.Pause();
    }

    public void Resume()
    {
        isPaused = false;
        navMeshAgent.isStopped = false;
        internalInvoker.Resume();
    }

    public void Cancel() => internalInvoker.CancelCurrentCommand();
}