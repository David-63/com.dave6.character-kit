using UnityEngine;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Stat;

namespace Dave6.CharacterKit.States
{
    public class FreeLookState : BaseState<PlayerController>
    {
        StatHandler m_StatHandler;
        bool m_PrevHasInput;
        float m_LastRotation;
        float m_CurrentVelocity;

        public FreeLookState(PlayerController controller) : base(controller)
        {
            m_StatHandler = controller.statHandler;
            if (m_StatHandler == null)
            {
                Debug.Log("잘못연결된것같아요");
            }
        }
        public override void OnEnter()
        {
            controller.cameraHandler.SetFreeLookMode();
        }

        public override void OnExit()
        {
            controller.combatHandler.HideTargetMark();
        }

        public override void Update()
        {
            // 속도 계산
            UpdateTargetSpeed();

            // 애니메이션 업데이트
            bool hasInput = controller.HasMovementInput();
            UpdateAnimationParams(hasInput);

            // 입력이 없거나, 공격중이면 회전 금지
            UpdateCharacterRotation(hasInput);

            // UI 업데이트
            controller.combatHandler.UpdateTargetMark();
        }

        void UpdateCharacterRotation(bool hasInput)
        {
            if (!hasInput) return;
            if (controller.attacking) return;

            float deltaTime = Time.deltaTime;
            
            // 입력 기반으로 회전값 구해서 캐릭터와 이동방향 적용
            float targetRotation = controller.mover.CalcTargetYawByInput();
            float rotation = controller.mover.SmoothYawUpdate(targetRotation, deltaTime);

            controller.mover.ApplyCharacterRotation(rotation);
            controller.moveDirection = controller.mover.CalcMoveDirByInput(rotation, deltaTime);
        }

        void UpdateAnimationParams(bool hasInput)
        {
            controller.animatorHandler.UpdateMoveSpeed();
            controller.animatorHandler.UpdateHasMovementInput();

            if (m_PrevHasInput && !hasInput)
            {
                controller.animatorHandler.UpdateLastMoveSpeed();
            }

            m_PrevHasInput = hasInput;
        }

        //public virtual void FixedUpdate();

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;
            SecondaryStat moveStat = m_StatHandler.GetStat("S_MoveSpeed") as SecondaryStat;

            if (controller.attacking)
            {
                controller.targetSpeed = targetSpeed;
                return;
            }

            if (controller.HasMovementInput())
            {
                if (controller.shiftInput)
                {
                    targetSpeed = moveStat.finalValue * 1.8f;
                }
                else
                {
                    targetSpeed = moveStat.finalValue;
                }
            }

            controller.targetSpeed = targetSpeed;
        }
    }
}