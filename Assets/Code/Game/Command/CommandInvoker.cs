using UnityEngine;

public class CommandInvoker
{
    private ICommand currentCommand;
    private Entity entity;
    
#if UNITY_EDITOR
    public ICommand CurrentCommand => currentCommand;
#endif
    
    public CommandInvoker(Entity entity)
    {
        this.entity = entity;
    }
    
    public void Enqueue(ICommand command)
    {
        if (!command.CanExecute(entity))
        {
            Debug.LogWarning($"[CommandInvoker] Cannot execute command: {command}");
            return;
        }

        CancelCurrentCommand();

        command.Initialize(entity);
        currentCommand = command;
        currentCommand.Execute();
        Debug.Log($"[CommandInvoker] Executing command: {currentCommand}");
    }

    public void Update()
    {
        if (currentCommand == null) return;

        currentCommand.Update();

        // TODO: Reput a frameRate limiter here ??
        if (!currentCommand.IsStillValid())
        {
            Debug.LogWarning($"[CommandInvoker] Command became invalid: {currentCommand}");
            CancelCurrentCommand();
        }
        else if (currentCommand.IsCompleted())
        {
            Debug.Log($"[CommandInvoker] Command completed: {currentCommand}");
            currentCommand = null;
        }
    }
    
    public void CancelCurrentCommand()
    {
        currentCommand?.Cancel();
        currentCommand = null;
    }

    public bool HasCurrentCommand => currentCommand != null;
    
    public bool TryGetCurrentCommand(out ICommand command)
    {
        command = currentCommand;
        return command != null;
    }

    // TODO: Set paused as boolean for the new command in coming to first set the pause before the update
    public void Pause() => (currentCommand as IInterruptibleCommand)?.Pause(); 
    public void Resume() => (currentCommand as IInterruptibleCommand)?.Resume();
}