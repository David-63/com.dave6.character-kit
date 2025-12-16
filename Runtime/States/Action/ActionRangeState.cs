using Dave6.StateMachine;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.States
{
    // 
    public class ActionRangeState : BaseState<PlayerController>
    {
        float m_AttackDuration = 2f;
        public Timer m_EndTimer;
        GameObject cacheProjectilePrefab;
        public ActionRangeState(PlayerController controller) : base(controller)
        {
            m_EndTimer = new Countdown(m_AttackDuration);
            m_EndTimer.OnTimerStop += AttackFinish;

            // 투사체 캐싱은 전용 함수를 두고 런타임중에 변경하도록 구조를 바꿔야함
            cacheProjectilePrefab = controller.combatHandler.projectilePrefab;
        }
        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
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
            GameObject projectileOjb = controller.InstantiatePrefab(cacheProjectilePrefab, controller.combatHandler.muzzle.position, controller.transform.rotation);
            projectileOjb.GetComponent<ProjectileMover>().Initialize(controller);
            float amplitude = 6f;
            float duration = 0.25f;
            float frequency = 6f;
            controller.cameraHandler.PlayShake(amplitude,duration,frequency);

            CooldownTimer(m_EndTimer);
        }

        void AttackFinish() => controller.exitRangeFlag = true;

        void CooldownTimer(Timer target)
        {
            if (target.IsRunning)
            {
                target.Reset();
                target.Resume();
            }
            else
            {
                target.Start();
            }
        }
    }
}
