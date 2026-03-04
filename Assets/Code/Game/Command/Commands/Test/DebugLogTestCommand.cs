using UnityEngine;

public class DebugLogTestCommand : INonBlockingCommand
{
    private string message;
    
    public DebugLogTestCommand(string message)
    {
        this.message = message;
    }
    
    public bool CanExecute(Entity entity) => true;
    public void Initialize(Entity entity) { }
    public void Execute() => Debug.Log(message);
    public void Update() { }
    public bool IsStillValid() => true;
    public bool IsCompleted() => true;
    public void Cancel() { }
}