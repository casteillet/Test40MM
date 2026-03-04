namespace Code.Scripts.StateMachine
{
    public abstract class HierarchicalState : IState
    {
        protected internal IState CurrentSubState { get; private set; }

        public virtual void OnEnter() => CurrentSubState?.OnEnter();
        public virtual void Update() => CurrentSubState?.Update();
        public virtual void FixedUpdate() => CurrentSubState?.FixedUpdate();
        public virtual void OnExit() => CurrentSubState?.OnExit();

        private IState parent;
        
        public void SetSubState(IState newState)
        {
            if (CurrentSubState == newState) return;

            CurrentSubState?.OnExit();
            CurrentSubState = newState;

            if (CurrentSubState is HierarchicalState hierarchicalState)
            {
                hierarchicalState.SetParent(this);
            }
            else if (CurrentSubState is IParentAwareState parentAwareState)
            {
                parentAwareState.SetParent(this);
            }

            CurrentSubState?.OnEnter();
        }

        private void SetParent(IState parent) => this.parent = parent;

        public void ExitToParent()
        {
            OnExit();
            if (parent is HierarchicalState parentHs)
            {
                parentHs.SetSubState(null);
            }
        }
        
        public virtual void Interrupt()
        {
            if (CurrentSubState is IInterruptibleState interruptible)
            {
                interruptible.Interrupt();
            }
        }

        public virtual bool IsInterrupted()
        {
            return CurrentSubState is IInterruptibleState { IsInterrupted: true } interruptible;
        }
    }
}