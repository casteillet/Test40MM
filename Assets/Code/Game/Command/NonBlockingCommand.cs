public abstract class NonBlockingCommand : ICommand
{
    public abstract void Initialize(Entity entity);
    public abstract void Update();
    public virtual bool IsFinished => false;
}