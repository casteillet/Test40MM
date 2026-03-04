using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public class ActionState : BaseState
    {
        private readonly int actionHash = Animator.StringToHash("Action");
        
        public ActionState(Entity entity, Animator animator) : base(entity, animator) { }

        public override void OnEnter()
        {
            Debug.Log("ActionState.OnEnter");
            // animator.CrossFade(actionHash, crossFadeDuration);
            // agent.Action(); // TODO: Create an AgentInteractor or AgentInteraction ?

            DisableNavAgentInput();
        }

        public override void OnExit()
        {
            // agent.CancelAction(); // TODO: Create an AgentInteractor or AgentInteraction ?
            EnableNavAgentInput();
        }
        
        private void DisableNavAgentInput()
        {
            //agent.NavAgentInput.DisableInput();
        }

        private void EnableNavAgentInput()
        {
            //agent.NavAgentInput.EnableInput();
        }
    }
}