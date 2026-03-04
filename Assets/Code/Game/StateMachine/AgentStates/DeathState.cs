using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public class DeathState : BaseState
    {
        public DeathState(Entity entity, Animator animator) : base(entity, animator) { }

        public override void OnEnter()
        {
            Debug.Log("DeathState.OnEnter");
            
            // DisableAnimator();
            ActivateRagdoll();
            
            StopNavMeshAgent();
            DisableNavAgentInput();
        }
        
        // private void DisableAnimator() => animator.enabled = false;
        private void ActivateRagdoll()
        {
            Debug.Log("Activate ragdoll");
            // agent.Ragdoll.Activate();
        }

        private void StopNavMeshAgent()
        {
            //agent.NavAgentInput.Agent.ResetPath();
        }

        private void DisableNavAgentInput()
        {
            //agent.NavAgentInput.DisableInput();
        }
    }
}