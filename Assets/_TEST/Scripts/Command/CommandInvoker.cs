using UnityEngine;

public class CommandInvoker
{
    private ICommand currentCommand;
    private Entity entity;

    public CommandInvoker(Entity entity)
    {
        this.entity = entity;
    }

    public void Enqueue(ICommand command)
    {
        command.Initialize(entity);
        currentCommand = command;
        Debug.Log($"Executing command: {currentCommand}");
    }

    public void Update()
    {
        if (currentCommand == null) return;

        currentCommand.Update();

        if (currentCommand.IsFinished)
        {
            currentCommand = null;
        }
    }
}