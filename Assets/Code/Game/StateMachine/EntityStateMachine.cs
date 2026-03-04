using KBCore.Refs;
using UnityEngine;

namespace Code.Scripts.StateMachine
{
    public class EntityStateMachine : ValidatedMonoBehaviour
    {
        [HideInInspector, SerializeField, Self] private Entity entity;
        
        private bool isBeingHealed;
        private StateMachine stateMachine;
        
        public CommandInvoker CommandInvoker;
        
        private void Awake()
        {
            stateMachine = new StateMachine();
            CommandInvoker = new CommandInvoker(entity);
        }

        public void OnStartHealingByOtherAgent()
        {
            isBeingHealed = true;
            CommandInvoker.Pause();

            if (stateMachine.CurrentState.CurrentSubState is IInterruptibleState interruptible)
            {
                interruptible.Interrupt();
            }
        }

        public void FinishHealing()
        {
            isBeingHealed = false;
            CommandInvoker.Resume();
        }

        public void Update()
        {
            CommandInvoker.Update();
            stateMachine.Update();
        }
    }
}