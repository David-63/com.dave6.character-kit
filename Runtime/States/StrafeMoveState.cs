using System;
using UnityEngine;
using Dave6.StateMachine;

namespace Dave6.CharacterKit.States
{
    public class StrafeMoveState : BaseState<PlayerController>
    {
        bool ShiftToggle = false;

        public StrafeMoveState(PlayerController controller) : base(controller) { }

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
            UpdateTargetSpeed();
            controller.GetMover().CalculateSpeed(Time.deltaTime);
            controller.GetMover().StrafeMoveDirection();
            //HandleInput();
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            if (controller.HasMovementInput())
            {
                targetSpeed = controller.GetMover().GetMovementProfile().StrafeSpeed;
            }

            controller.TargetSpeed = targetSpeed;
        }

        void OnShiftToggled()
        {
            ShiftToggle = !ShiftToggle;
            controller.GetMover().SetStrafeMode(ShiftToggle);
        }
    }
}