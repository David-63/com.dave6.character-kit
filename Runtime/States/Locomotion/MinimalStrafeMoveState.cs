using UnityEngine;
using Dave6.StateMachine;

namespace Dave6.CharacterKit.States
{
    public class MinimalStrafeMoveState : BaseState<BasicPlayerController>
    {
        bool ShiftToggle = false;
        const float m_RotateDuration = 2.0f;

        public MinimalStrafeMoveState(BasicPlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            ShiftToggle = true;
            controller.cameraHandler.SetStrafeMode(ShiftToggle);
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

            controller.animatorHandler.UpdateMoveSpeed();
            controller.animatorHandler.UpdateHasMovementInput();

            float targetRotation = controller.mover.CalcTargetYawByCamera();
            float rotation = controller.mover.SmoothYawUpdate(targetRotation, deltaTime);
            controller.mover.ApplyCharacterRotation(rotation);
            controller.moveDirection = controller.mover.CalcMoveDirByCamera(deltaTime);

            controller.mover.SmoothInputDirection(deltaTime);
            controller.animatorHandler.UpdateDirectionX(controller.cachedInputDir.x);
            controller.animatorHandler.UpdateDirectionY(controller.cachedInputDir.z);
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            if (controller.HasMovementInput())
            {
                targetSpeed = controller.mover.GetMovementProfile().StrafeSpeed;
            }

            controller.targetSpeed = targetSpeed;
        }

        void OnShiftToggled()
        {
            ShiftToggle = !ShiftToggle;
            controller.cameraHandler.SetStrafeMode(ShiftToggle);
        }
    }
}