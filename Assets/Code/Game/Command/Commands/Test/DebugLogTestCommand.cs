using UnityEngine;

public class DebugLogTestCommand : INonBlockingCommand
{
    private string message;
    
    public DebugLogTestCommand(string message)
    {
        this.message = message;
    }
    
    public bool CanExecute(Agent agent) => true;
    public void Initialize(Agent agent) { }
    public void Execute() => Debug.Log(message);
    public void Update() { }
    public bool IsStillValid() => true;
    public bool IsCompleted() => true;
    public void Cancel() { }
}