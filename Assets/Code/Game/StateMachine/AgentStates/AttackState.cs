using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public class AttackState : BaseState
    {
        private readonly int attackHash = Animator.StringToHash("Attack");
        
        public AttackState(Entity entity, Animator animator) : base(entity, animator) { }

        public override void OnEnter()
        {
            Debug.Log("AttackState.OnEnter");
            // animator.CrossFade(attackHash, crossFadeDuration);
            // agent.Attack();
        }
    }
}