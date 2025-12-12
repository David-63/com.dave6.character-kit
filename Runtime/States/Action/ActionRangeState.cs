using System;
using Dave6.StateMachine;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.States
{
    // 
    public class ActionRangeState : BaseState<PlayerController>
    {
        float m_AttackDuration = 2f;
        public Timer endTimer;
        public ActionRangeState(PlayerController controller) : base(controller)
        {
            endTimer = new Countdown(m_AttackDuration);
            endTimer.OnTimerStop += AttackFinish;
        }
        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
            endTimer.Pause();
        }

        public override  void Update()
        {
            // 조건을 tap이 아니라 hold로 두고 내부에 RPM을 둬서 제어하는 방식으로 변경하기
            if (controller.attackInputTap)
            {
                DoFire();
            }
        }

        /// <summary>
        /// 시작 위치, 방향 정도?
        /// </summary>
        void DoFire()
        {
            Debug.Log("사격!");
            endTimer.Reset();
            endTimer.Resume();
        }

        void AttackFinish() => controller.exitRangeFlag = true;
    }
}
