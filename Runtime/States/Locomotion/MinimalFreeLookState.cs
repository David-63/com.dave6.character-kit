using UnityEngine;
using Dave6.StateMachine;

namespace Dave6.CharacterKit.States
{
    public class MinimalFreeLookState : BaseState<BasicPlayerController>
    {
        bool m_PrevHasInput;

        public MinimalFreeLookState(BasicPlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            controller.cameraHandler.SetFreeLookMode();
        }

        public override void OnExit() { }

        public override void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateTargetSpeed();

            // 당장은 수평속도를 사용했는데, 키입력 여부에 따라 애니메이션이 설정되도록 해야함(여기서 말고 Editor에 조건 추가)
            controller.animatorHandler.UpdateMoveSpeed();
            controller.animatorHandler.UpdateHasMovementInput();

            bool hasInput = controller.HasMovementInput();
            if (m_PrevHasInput && !hasInput)
            {
                controller.animatorHandler.UpdateLastMoveSpeed();
            }

            m_PrevHasInput = hasInput;

            if (!hasInput) return;

            // 회전 계산
            float targetRotation = controller.mover.CalcTargetYawByInput();
            float rotation = controller.mover.SmoothYawUpdate(targetRotation, deltaTime);
            controller.mover.ApplyCharacterRotation(rotation);
            controller.moveDirection = controller.mover.CalcMoveDirByInput(rotation, deltaTime);
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            if (controller.HasMovementInput())
            {
                if (controller.shiftInput)
                {
                    targetSpeed = controller.mover.GetMovementProfile().SprintSpeed;
                }
                else
                {
                    targetSpeed = controller.mover.GetMovementProfile().JogSpeed;
                }
            }

            controller.targetSpeed = targetSpeed;
        }
    }
}