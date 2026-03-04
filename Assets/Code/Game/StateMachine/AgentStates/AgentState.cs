using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public class AgentState : HierarchicalState
    {
        private Agent agent;
        private Animator animator;

        private IdleState idleState;
        private MoveState moveState;
        private CombatState combatState;

        public AgentState(Agent agent, Animator animator)
        {
            this.agent = agent;
            this.animator = animator;

            idleState = new IdleState(agent, animator);
            moveState = new MoveState(agent, animator);
            combatState = new CombatState(agent, animator);
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
        public IdleState(Agent agent, Animator animator) : base(agent, animator) { }

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
        public MoveState(Agent agent, Animator animator) : base(agent, animator) { }

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
        public CombatState(Agent agent, Animator animator) : base(agent, animator) { }

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
