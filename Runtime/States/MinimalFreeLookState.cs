using UnityEngine;
using Dave6.StateMachine;

namespace Dave6.CharacterKit.States
{
    public class MinimalFreeLookState : BaseState<BasicPlayerController>
    {
        public MinimalFreeLookState(BasicPlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.GetMover().SetFreeLookMode();
        }

        public override void OnExit() { }

        public override void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateTargetSpeed();
            controller.GetMover().CalculateSpeed(deltaTime);
            controller.GetMover().FreeLookRotate(deltaTime);
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            if (controller.HasMovementInput())
            {
                if (controller.shiftInput)
                {
                    targetSpeed = controller.GetMover().GetMovementProfile().SprintSpeed;
                }
                else
                {
                    targetSpeed = controller.GetMover().GetMovementProfile().JogSpeed;
                }
            }

            controller.targetSpeed = targetSpeed;
        }
    }
}