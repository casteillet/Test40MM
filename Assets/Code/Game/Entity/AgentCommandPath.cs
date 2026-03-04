using System.Collections.Generic;
using System.Linq;
using KBCore.Refs;
using UnityEngine;
using UnityUtils;

public class Waypoint
{
    public Vector3 Position;
    public ICommand Command;

    public Waypoint(Vector3 position)
    {
        Position = position;
    }
    
    public void AddCommand(ICommand command) => Command = command;
    public void RemoveCommand() => Command = null;
    
    public bool TryGetCommand(out ICommand command)
    {
        command = Command;
        return Command != null;
    }
}

public class AgentCommandPath : ValidatedMonoBehaviour
{
    [HideInInspector, SerializeField, Self] private Agent agent;
    
    [SerializeField] private LineRenderer lineRenderer;

    private MovePathCommand currentMovePathCommand;
    
    [HideInInspector] public PathEditMode EditMode = PathEditMode.None;
    
    public List<Waypoint> Waypoints { get; } = new();
    
    public void AppendWaypoint(Vector3 position)
    {
        var waypoint = new Waypoint(position);
        Waypoints.Add(waypoint);
        UpdateLine();

        if (Waypoints.Count == 1)
        {
            SendMovePathCommand();
        }
    }

    public void ClearWaypoints()
    {
        Waypoints.Clear();
        currentMovePathCommand?.Cancel();
    }

    private void SendMovePathCommand()
    {
        currentMovePathCommand = new MovePathCommand(Waypoints);
        agent.StateMachine.CommandInvoker.Enqueue(currentMovePathCommand);
    }
    
    private void UpdateLine()
    {
        lineRenderer.positionCount = Waypoints.Count;
        lineRenderer.SetPositions(Waypoints.Select(waypoint => waypoint.Position.Add(y:.01f)).ToArray());
    }
    
    public void AddCommandAt(int index, ICommand command)
    {
        if (index >= 0 && index < Waypoints.Count)
        {
            Waypoints[index].AddCommand(command);
            //Debug.Log($"[AgentPath] AddCommandAt {index}", this);
        }
    }
    
    public void RemoveCommandAt(int index)
    {
        if (index >= 0 && index < Waypoints.Count)
        {
            Waypoints[index].RemoveCommand();
            //Debug.Log($"[AgentPath] RemoveCommandAt {index}", this);
        }
    }
    
    public bool ExistCommandAt(int index) => Waypoints[index].Command != null;

    private void OnDrawGizmos()
    {
        foreach (var waypoint in Waypoints)
        {
            if (waypoint.Command == null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(waypoint.Position, .05f);
            }
            else
            {
                if (waypoint.Command is InteractCommand)
                {
                    Gizmos.color = Color.green;
                } 
                else if (waypoint.Command is LookAtCommand)
                {
                    Gizmos.color = Color.red;
                }
                else if (waypoint.Command is LockAimCommand)
                {
                    Gizmos.color = Color.blue;
                }
                else if (waypoint.Command is QuickLookCommand)
                {
                    Gizmos.color = Color.yellow;
                }
                Gizmos.DrawSphere(waypoint.Position, .1f);
            }
        }
    }
}