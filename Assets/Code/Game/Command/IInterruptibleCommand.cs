public interface IInterruptibleCommand : ICommand
{
    public void Pause();
    public void Resume();
}
