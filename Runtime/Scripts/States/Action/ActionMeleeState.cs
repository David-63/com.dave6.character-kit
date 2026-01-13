using System;
using Dave6.CharacterKit.Item;
using Dave6.StateMachine;
using ProtoCode;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.States
{
    /// <summary>
    /// 
    /// 언제 공격을 시도하는가              | 공격 유효 타이머¿
    /// 상태 진입 / 종료                   | 
    /// FSM 전환 조건                       |
    /// 입력 버퍼 여부                      | 버퍼 입력받는 기간을 정해주면 될듯?
    /// </summary>
    public class ActionMeleeState : BaseState<PlayerCharacter>
    {
        bool m_BufferInput = false;

        // 공격시 회전시킴
        float m_LastRotation;
        float m_CurrentVelocity;

        public ActionMeleeState(PlayerCharacter controller) : base(controller) { }

        public override void OnEnter()
        {
            m_BufferInput = false;
            controller.attackTimer.RestartTimer();
            Debug.Log("Melee Enter");
        }

        public override void OnExit()
        {
            m_BufferInput = false;
            Debug.Log("Melee Exit");
        }

        public override void Update()
        {
            controller.EvaluateAttackExit(m_BufferInput);
            HandleMeleeAttack();
        }
        public override void LateUpdate()
        {
            UpdateCharacterDirection();
        }

        void HandleMeleeAttack()
        {
            if (controller.attackInputTap || m_BufferInput)
            {
                if (controller.combatHandler.TryMeleeAttack())
                {
                    m_BufferInput = false;
                }
                else
                {
                    m_BufferInput = true;
                }
            }
        }

        void UpdateCharacterDirection()
        {
            if (!controller.combatHandler.attacking) return;

            float rotation;

            if (controller.combatHandler.TryGetMeleeTargetYaw(out float yaw))
            {
                rotation = controller.mover.SmoothYawUpdate(yaw, ref m_CurrentVelocity);
            }
            else
            {
                rotation = controller.mover.SmoothYawUpdate(controller.mover.lastTargetInputRotation, ref m_CurrentVelocity);
            }

            controller.mover.ApplyCharacterRotation(rotation);
            controller.moveDirection = controller.mover.CalcMoveDirByInput(rotation, Time.deltaTime);
        }
    }
}

// 카메라를 돌리는 로직은 필요 없는듯함
// // 공격이 나가는동안 유효함
// if (existTimer.IsRunning)
// {
//     // 캐릭터가 카메라 방향으로 회전
//     float targetDir = controller.GetMover().CalcTargetRotationByCamera();
//     float currentDir = controller.GetMover().transform.eulerAngles.y;

//     // 캐릭터가 카메라 방향으로 회전하는 로직
//     //float lerpRotation = controller.GetMover().SmoothRotateUpdate(currentDir, targetDir, 0.3f);                
//     //controller.GetMover().ApplyCharacterRotation(lerpRotation);

//     // 카메라가 캐릭터 방향으로 회전하는 로직
//     float angleDiff = Mathf.DeltaAngle(currentDir, targetDir);
//     if (Mathf.Abs(angleDiff) < 120f)
//     {
//         float amount = 4f;
//         controller.GetMover().NudgeCameraToCharacter(amount);    
//     }
// }