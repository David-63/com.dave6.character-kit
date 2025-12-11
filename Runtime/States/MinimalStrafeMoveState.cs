using UnityEngine;
using Dave6.StateMachine;

namespace Dave6.CharacterKit.States
{
    public class MinimalStrafeMoveState : BaseState<BasicPlayerController>
    {
        bool ShiftToggle = false;

        public MinimalStrafeMoveState(BasicPlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            ShiftToggle = true;
            controller.GetMover().SetStrafeMode(ShiftToggle);
            controller.GetInputReader().ShiftToggleChanged += OnShiftToggled;
        }

        public override void OnExit()
        {
            controller.GetInputReader().ShiftToggleChanged -= OnShiftToggled;
        }

        public override void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateTargetSpeed();
            controller.GetMover().CalculateSpeed(deltaTime);
            controller.GetMover().StrafeMoveRotate(deltaTime);
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            if (controller.HasMovementInput())
            {
                targetSpeed = controller.GetMover().GetMovementProfile().StrafeSpeed;
            }

            controller.targetSpeed = targetSpeed;
        }

        void OnShiftToggled()
        {
            ShiftToggle = !ShiftToggle;
            controller.GetMover().SetStrafeMode(ShiftToggle);
        }
    }
}