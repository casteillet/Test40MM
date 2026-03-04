namespace Code.Scripts.StateMachine
{
    public interface IParentAwareState : IState
    {
        public void SetParent(HierarchicalState parent);
    }
}