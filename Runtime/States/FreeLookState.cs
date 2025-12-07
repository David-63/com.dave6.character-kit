using UnityEngine;
using Dave6.StateMachine;

namespace Dave6.CharacterKit.States
{
    public class FreeLookState : BaseState<PlayerController>
    {
        public FreeLookState(PlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.GetMover().SetFreeLookMode();
        }

        public override void OnExit()
        {
        }

        public override  void Update()
        {
            UpdateTargetSpeed();
            controller.GetMover().CalculateSpeed(Time.deltaTime);
            controller.GetMover().FreeLookDirection();
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            if (controller.HasMovementInput())
            {
                if (controller.ShiftInput)
                {
                    targetSpeed = controller.GetMover().GetMovementProfile().SprintSpeed;
                }
                else
                {
                    targetSpeed = controller.GetMover().GetMovementProfile().JogSpeed;
                }
            }

            controller.TargetSpeed = targetSpeed;
        }
    }
}