using System;
using Dave6.StateMachine;
using ProtoCode;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.States
{
    /// <summary>
    /// 진입 조건
    /// AimInput이 없고, Idle상태일 때.
    /// 
    /// 전반적으로 구조를 싹 갈아엎어야함
    /// 당장은 프로토타이핑으로 기능만 구현
    /// </summary>
    public class ActionMeleeState : BaseState<PlayerController>
    {
        
        float m_AttackDuration = 2f;
        Timer m_EndTimer;

        #region combo field
        int m_ComboStep = 0;
        const int comboEnd = 3;
        bool entered = false;
        Timer m_StepTimer;
        float stepDuration = 0.6f;
        #endregion

        #region collition field
        bool canAttack = true;
        GameObject m_HitObject;
        Timer existTimer;
        float colliderDuration = 0.2f;
        #endregion

        public ActionMeleeState(PlayerController controller) : base(controller)
        {
            // 공격 유효시간
            m_EndTimer = new Countdown(m_AttackDuration);
            m_EndTimer.OnTimerStop += AttackFinish;
            // 콤보 제한시간
            m_StepTimer = new Countdown(stepDuration);
            m_StepTimer.OnTimerStop += ComboReset;


            m_HitObject = controller.CreateGameObject(controller.hitColliderPrefab);
            m_HitObject.GetComponent<HitCollider>().Initialize(controller);
            m_HitObject.transform.localPosition = new Vector3(0, 1, 1);
            m_HitObject.SetActive(false);
            existTimer = new Countdown(colliderDuration);
            existTimer.OnTimerStop += CollitionEnd;       
        }



        public override void OnEnter()
        {
            entered = false;
            m_ComboStep = 0;
        }

        public override void OnExit()
        {
            m_StepTimer.Pause();
        }

        public override  void Update()
        {
            if (!entered)
            {
                entered = true;
                DoAttack(m_ComboStep);
                if (m_StepTimer.IsRunning)
                {
                    m_StepTimer.Reset();
                    m_StepTimer.Resume();
                }
                else
                {
                    m_StepTimer.Start();
                }
                
                return;
            }
            // 입력 감지
            // 타이머 연장 및 콤보 연계
            if (controller.attackInputTap)
            {
                DoAttack(m_ComboStep);
                if (m_StepTimer.IsRunning)
                {
                    m_StepTimer.Reset();
                    m_StepTimer.Resume();
                }
                else
                {
                    m_StepTimer.Start();
                }
                return;
            }
        }

        void DoAttack(int comboIndex)
        {
            // 당장은 타이밍 맞게 공격해야 콤보가 이어지지만,
            // 입력을 버퍼로 받아서 선입력 되도록 할 예정
            if (!canAttack) return;
            canAttack = false;

            if (comboIndex == 0)
            {
                Debug.Log($"{m_ComboStep}: 진입 타격!");
                m_ComboStep++;
            }
            else if (comboIndex == comboEnd -1)
            {
                Debug.Log($"{m_ComboStep}: 마무리 타격!");
                ComboReset();
            }
            else
            {
                Debug.Log($"{m_ComboStep}: 타격!");
                m_ComboStep++;
            }

            m_HitObject.SetActive(true);

            // 공격 할때마다 타이머 초기화
            if (existTimer.IsRunning)
            {
                existTimer.Reset();
                existTimer.Resume();
            }
            else
            {
                existTimer.Start();
            }

            if (m_EndTimer.IsRunning)
            {
                m_EndTimer.Reset();
                m_EndTimer.Resume();
            }
            else
            {
                m_EndTimer.Start();
            }

        }

        // 콜백 이벤트
        void AttackFinish()
        {
            controller.exitMeleeFlag = true;
        }
        void ComboReset()
        {
            m_ComboStep = 0;
        }
        void CollitionEnd()
        {
            canAttack = true;
            m_HitObject.SetActive(false);
        }
    }

}
