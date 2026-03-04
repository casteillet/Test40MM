namespace Code.Scripts.StateMachine
{
    public interface IInterruptibleState : IState
    {
        public void Interrupt();
        public bool IsInterrupted { get; }
    }
}