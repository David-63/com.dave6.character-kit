using UnityEngine;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Stat;

namespace Dave6.CharacterKit.States
{
    public class StrafeMoveState : BaseState<PlayerController>
    {
        StatHandler m_StatHandler;
        bool ShiftToggle = false;

        const float m_RotateSmoothTime = 2.0f;
        //const float m_RotateDuration = 0.06f;
        const float m_SpeedLess = 0.4f;

        public StrafeMoveState(PlayerController controller) : base(controller)
        {
            m_StatHandler = controller.statHandler;
            if (m_StatHandler == null)
            {
                Debug.Log("잘못연결된것같아요");
            }
        }

        public override void OnEnter()
        {
            ShiftToggle = true;

            // 카메라에서 제공하는 트렌지션 함수로 변경하기
            controller.cameraHandler.SetStrafeMode(ShiftToggle);
            controller.GetInputReader().ShiftToggleChanged += OnShiftToggled;
            controller.animatorHandler.UpdateUseStrafe(true);
            controller.animatorHandler.UpdateUseShift(false);
        }

        public override void OnExit()
        {
            controller.GetInputReader().ShiftToggleChanged -= OnShiftToggled;
            controller.animatorHandler.UpdateUseStrafe(false);
        }

        public override void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateTargetSpeed(deltaTime);
            UpdateTargetRotate(deltaTime);
            UpdateAnimParams(deltaTime);
        }

        void UpdateTargetSpeed(float deltaTime)
        {
            float targetSpeed = 0;

            SecondaryStat moveStat = m_StatHandler.GetStat("S_MoveSpeed") as SecondaryStat;
            float moveSpeed = moveStat.finalValue;

            if (controller.HasMovementInput())
            {
                targetSpeed = moveSpeed - moveSpeed * m_SpeedLess;
            }

            controller.targetSpeed = targetSpeed;

            if (controller.mover.isGrounded)
            {
                controller.mover.CalcGroundSpeed(deltaTime);
            }
            else
            {
                controller.mover.CalcAirborneSpeed(deltaTime);
            }
        }

        /// <summary>
        /// 상체 하체 따로 회전량 넣어야함
        /// Yaw 계산 진행, Pitch는 mover에서 상시 계산
        /// </summary>
        void UpdateTargetRotate(float deltaTime)
        {
            // 목표 회전값
            float targetYaw = controller.mover.CalcTargetYawByAimPoint();
            // 회전값 보간
            float rotation = controller.mover.SmoothYawUpdate(targetYaw, deltaTime);
            // 캐릭터 회전 적용
            controller.mover.ApplyCharacterRotation(rotation);
            // 이동방향 적용
            controller.moveDirection = controller.mover.CalcMoveDirByCamera(deltaTime);
        }


        void UpdateAnimParams(float deltaTime)
        {
            controller.animatorHandler.UpdateMoveSpeed();
            controller.animatorHandler.UpdateHasMovementInput();
            controller.mover.SmoothInputDirection(deltaTime);
            controller.animatorHandler.UpdateDirectionX(controller.cachedInputDir.x);
            controller.animatorHandler.UpdateDirectionY(controller.cachedInputDir.z);
        }

        void OnShiftToggled()
        {
            ShiftToggle = !ShiftToggle;

            // 카메라에서 제공하는 트렌지션 함수로 변경하기
            controller.cameraHandler.SetStrafeMode(ShiftToggle);
            controller.animatorHandler.UpdateUseShift(!ShiftToggle);
        }
    }
}