using System.Collections.Generic;
using UnityEngine;

public class Waypoint
{
    public Vector3 Position;
    public List<ICommand> Commands = new();

    public Waypoint(Vector3 position)
    {
        Position = position;
    }

    public void AddCommand(ICommand command)
    {
        Commands.Add(command);
    }
    
    // public bool TryGetCommands(out List<ICommand> commands)
    // {
    //     commands = Commands;
    //     return Commands != null;
    // }
}