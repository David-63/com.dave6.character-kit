using UnityEngine;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Stat;
using Dave6.CharacterKit.Item;

namespace Dave6.CharacterKit.States
{
    public class StrafeMoveState : BaseState<PlayerCharacter>
    {
        StatHandler m_StatHandler;
        bool ShiftToggle = false;

        const float m_RotateSmoothTime = 2.0f;
        //const float m_RotateDuration = 0.06f;
        const float m_SpeedLess = 0.4f;

        BaseStat m_MoveStat;

        public StrafeMoveState(PlayerCharacter controller, BaseStat moveStat) : base(controller)
        {
            m_StatHandler = controller.statHandler;
            m_MoveStat = moveStat;
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

            if (controller.equipHandler.HasFirearm())
            {
                controller.animatorHandler.UpdateIsAim(true);
            }
        }

        public override void OnExit()
        {
            controller.GetInputReader().ShiftToggleChanged -= OnShiftToggled;
            controller.animatorHandler.UpdateUseStrafe(false);
            controller.animatorHandler.UpdateIsAim(false);
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

            float moveSpeed = m_MoveStat.finalValue;

            if (controller.HasMovementInput())
            {
                targetSpeed = moveSpeed - moveSpeed * m_SpeedLess;
            }

            controller.targetSpeed = targetSpeed;
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
            controller.animatorHandler.UpdateMoveInput(controller.horizontalSpeed, controller.HasMovementInput());
            controller.mover.SmoothInputDirection(deltaTime);
            controller.animatorHandler.UpdateDirection(controller.cachedInputDir);
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