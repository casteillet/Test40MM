public interface ICommand
{
    public bool CanExecute(Agent agent);
    public void Initialize(Agent agent);
    public void Execute();
    public void Update();
    public bool IsStillValid();
    public bool IsCompleted();
    public void Cancel();
}