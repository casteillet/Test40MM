public interface ICommand
{
    public bool CanExecute(Entity entity);
    public void Initialize(Entity entity);
    public void Execute();
    public void Update();
    public bool IsStillValid();
    public bool IsCompleted();
    public void Cancel();
}