using UnityEngine;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Stat;

namespace Dave6.CharacterKit.States
{
    public class FreeLookState : BaseState<PlayerController>
    {
        StatHandler m_StatHandler;

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
            controller.GetMover().SetFreeLookMode();
        }

        public override void OnExit()
        {
        }

        public override void Update()
        {
            float deltaTime = Time.deltaTime;

            // 속도 계산
            UpdateTargetSpeed();
            if (controller.GetMover().isGrounded)
            {
                controller.GetMover().CalcGroundSpeed(deltaTime);
            }
            else
            {
                controller.GetMover().CalcAirborneSpeed(deltaTime);
            }

            // 당장은 수평속도를 사용했는데, 키입력 여부에 따라 애니메이션이 설정되도록 해야함(여기서 말고 Editor에 조건 추가)
            controller.animatorHandler.UpdateMoveSpeed();
            controller.animatorHandler.UpdateHasMovementInput();

            if (!controller.HasMovementInput()) return;

            // 회전 계산
            float targetRotation = controller.GetMover().CalcTargetRotationByInput();
            float rotation = controller.GetMover().SmoothRotateUpdate(controller.GetMover().transform.eulerAngles.y, targetRotation, 0.12f);

            controller.GetMover().ApplyCharacterRotation(rotation);
            controller.moveDirection = controller.GetMover().CalcMoveDirByInput(rotation, deltaTime);
        }

        //public virtual void FixedUpdate();

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;
            if (controller.movementLocked)
            {
                controller.targetSpeed = targetSpeed;
                return;
            }

            SecondaryStat moveStat = m_StatHandler.GetStat("S_MoveSpeed") as SecondaryStat;

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