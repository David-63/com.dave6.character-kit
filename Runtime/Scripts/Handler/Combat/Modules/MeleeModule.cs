using UnityUtils.Timer;

namespace Dave6.CharacterKit.Handler.Combat
{
    public class MeleeModule : IActionModule
    {
        Timer _ComboTimer;
        int _ComboStep = 0;
        public bool IsAvailable {get;}

        public MeleeModule()
        {
            float stepDuration = 3f;
            _ComboTimer = new Countdown(stepDuration);
            _ComboTimer.OnTimerStop += ComboReset;
        }

        public void TryAction(BaseActionContext ctx, IActionAnimation anim)
        {
            ctx.AttackTimer.RestartTimer();
            //Debug.Log("Hiiiit!!");

            // m_HitboxExistTimer.RestartTimer();      // 히트박스 유지시간
            // m_Hitbox.gameObject.SetActive(true);

            ComboCount(anim);
        }
        public bool IsFinished(BaseActionContext ctx)
        {
            var playerCtx = ctx as PlayerCombatContext;
            if (!playerCtx.AttackTimer.IsRunning) return true;
            return false;
        }
        public void CleanupAction(BaseActionContext ctx)
        {
            ctx.AttackTimer.Stop();
            ComboReset();
        }

        bool TryGetMeleeTargetYaw(out float yaw)
        {
            yaw = 0f;
            return true;
        }

        void ComboReset()
        {
            _ComboStep = 0;
        }
        // void HitboxReset()
        // {
        //     m_Hitbox.gameObject.SetActive(false);
        // }

        void ComboCount(IActionAnimation anim)
        {
            _ComboTimer.RestartTimer(); // 콤보 시간 초기화
            string[] comboAnims = {"RightHook","LeftPunch","CrossPunch"};       // 이것도 아이템 정보 구조체로 래핑하면 좋을듯?
            anim.PlayAction(comboAnims[_ComboStep]);

            bool isLast = _ComboStep == comboAnims.Length - 1;
            if (isLast)
            {
                ComboReset();
            }
            else
            {
                _ComboStep++;
            }
        }
    }

}