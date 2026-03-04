using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public class AgentState : HierarchicalState
    {
        private Entity entity;
        private Animator animator;

        private IdleState idleState;
        private MoveState moveState;
        private CombatState combatState;

        public AgentState(Entity entity, Animator animator)
        {
            this.entity = entity;
            this.animator = animator;

            idleState = new IdleState(entity, animator);
            moveState = new MoveState(entity, animator);
            combatState = new CombatState(entity, animator);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            SetSubState(idleState);
        }

        public override void Update()
        {
            base.Update();

            // Example logic to switch substates:
            // if (agent.HasTarget)
            // {
            //     SetSubState(moveState);
            // }
            // else
            // {
            //     SetSubState(idleState);
            // }

            // Combat transitions or interruption could be handled similarly
        }
    }

    public class IdleState : BaseState
    {
        public IdleState(Entity entity, Animator animator) : base(entity, animator) { }

        public override void OnEnter()
        {
            base.OnEnter();
            animator.CrossFade("Idle", crossFadeDuration);
        }

        public override void Update()
        {
            // Idle logic here
        }
    }

    public class MoveState : BaseState
    {
        public MoveState(Entity entity, Animator animator) : base(entity, animator) { }

        public override void OnEnter()
        {
            base.OnEnter();
            animator.CrossFade("Run", crossFadeDuration);
            // agent.MoveToTarget();
        }

        public override void Update()
        {
            // if (agent.ReachedTarget)
            // {
            //     // When reached, maybe switch to an ActionState like open door, heal, etc.
            // }
        }
    }

    public class CombatState : BaseState
    {
        public CombatState(Entity entity, Animator animator) : base(entity, animator) { }

        public override void OnEnter()
        {
            base.OnEnter();
            animator.CrossFade("CombatReady", crossFadeDuration);
        }

        public override void Update()
        {
            // Combat logic here
        }
    }
}
