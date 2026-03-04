using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public class LocomotionState : BaseState
    {
        private readonly int locomotionHash = Animator.StringToHash("Locomotion");
        
        public LocomotionState(Agent agent, Animator animator) : base(agent, animator) { }
        
        public override void OnEnter()
        {
            Debug.Log("LocomotionState.OnEnter");
            // animator.CrossFade(locomotionHash, crossFadeDuration);
        }
    }
}