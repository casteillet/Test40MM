public interface ICommand
{
    void Initialize(Entity entity);
    void Update();
    bool IsFinished { get; }
}