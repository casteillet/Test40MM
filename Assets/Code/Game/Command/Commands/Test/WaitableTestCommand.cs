using UnityEngine;

public class WaitableTestCommand : ICommand
{
    private bool isCompleted = false;
    
    public bool CanExecute(Agent agent) => true;
    public void Initialize(Agent agent) { }
    public void Execute() {}

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCompleted = true;
        }
    }
    
    public bool IsStillValid() => true;
    
    public bool IsCompleted()
    {
        return isCompleted;
    }
    
    public void Cancel() { }
}