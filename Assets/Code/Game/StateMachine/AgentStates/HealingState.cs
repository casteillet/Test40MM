using UnityEngine;

namespace Code.Scripts.StateMachine.AgentStates
{
    public class HealingState : BaseState, IInterruptibleState, IParentAwareState
    {
        private readonly int idleHash = Animator.StringToHash("Idle");
        private readonly int healingHash = Animator.StringToHash("Healing");

        private bool isInterrupted;
        private float healDuration = 3f;
        private float healTimer;
        private HierarchicalState parent;

        public bool IsInterrupted => isInterrupted;

        public HealingState(Entity entity, Animator animator) : base(entity, animator) { }

        public void SetParent(HierarchicalState parent) => this.parent = parent;

        public override void OnEnter()
        {
            base.OnEnter();
            healTimer = 0f;
            isInterrupted = false;
            animator.CrossFade(healingHash, crossFadeDuration);
        }

        public override void Update()
        {
            if (isInterrupted) return;

            healTimer += Time.deltaTime;
            if (healTimer >= healDuration)
            {
                Debug.Log("Finish Healing");
                //agent.FinishHealing();
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            isInterrupted = false;
        }

        public void Interrupt()
        {
            isInterrupted = true;
            animator.CrossFade(idleHash, crossFadeDuration);
        }
    }
}