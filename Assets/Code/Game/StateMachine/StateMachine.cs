using System;
using System.Collections.Generic;

namespace Code.Scripts.StateMachine
{
    public class StateMachine
    {
        public HierarchicalState CurrentState { get; private set; }
        
        private Dictionary<Type, IState> states = new();
        private HashSet<ITransition> anyTransitions = new();
        
        public void Initialize(HierarchicalState initialState)
        {
            CurrentState = initialState;
            AddStateRecursive(CurrentState);
            CurrentState.OnEnter();
        }

        public void Update()
        {
            var transition = GetTransition();
            if (transition != null)
            {
                ChangeState(transition.To);
            }

            CurrentState?.Update();
        }

        public void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }

        private void AddStateRecursive(IState state)
        {
            if (state == null) return;

            if (!states.ContainsKey(state.GetType()))
            {
                states.Add(state.GetType(), state);
            }

            if (state is HierarchicalState hs && hs.CurrentSubState != null)
            {
                AddStateRecursive(hs.CurrentSubState);
            }
        }

        private void ChangeState(IState nextState)
        {
            if (nextState == null || CurrentState == nextState) return;

            CurrentState.OnExit();
            CurrentState = nextState as HierarchicalState;
            if (CurrentState == null)
                throw new Exception("Root state must be a HierarchicalState!");

            AddStateRecursive(CurrentState);
            CurrentState.OnEnter();
        }

        private ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if (transition.Condition.Evaluate())
                {
                    return transition;
                }
            }

            if (CurrentState == null) return null;

            // You can extend this to ask all substates for transitions, or keep root transitions only
            return null;
        }

        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(to, condition));
        }
    }
}