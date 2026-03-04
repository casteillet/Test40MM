using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public abstract class BaseState : IState
    {
        protected readonly Entity Entity;
        protected readonly Animator animator;
        
        protected const float crossFadeDuration = 0.1f;
        
        protected BaseState(Entity entity, Animator animator)
        {
            this.Entity = entity;
            this.animator = animator;
        }

        public virtual void OnEnter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void OnExit() { }
    }
}