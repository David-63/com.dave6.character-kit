using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.Handler.Combat
{
    public class PlayerCombat : BaseCombat
    {
        public PlayerCombatContext _CombatCtx => (PlayerCombatContext)_BaseContext;
        PlayerInputContext _InputCtx;

        #region 초기화
        protected override void Awake()
        {
            base.Awake();
            _BaseContext = new PlayerCombatContext();
            float attackDuration = 2f;
            _CombatCtx.AttackTimer = new Countdown(attackDuration);
            _CombatCtx.ExitReason = EActionExitReason.None;

            _Modules.Add(typeof(MeleeModule), new MeleeModule());
            _Modules.Add(typeof(RangeModule), new RangeModule());
            _Modules.Add(typeof(ReloadModule), new ReloadModule());
        }
        internal void BindInput(PlayerInputContext inputCtx) => _InputCtx = inputCtx;
        #endregion

        public void OnUpdate()
        {
            if (_ActiveModule == null) return;

            if (_ActiveModule.IsFinished(_CombatCtx))
            {
                // 종료 이유 결정
                if (!_CombatCtx.AttackTimer.IsRunning)
                {
                    SetExit(EActionExitReason.LeaseExpired);
                }
            }
        }

        #region Anim Bind Event
        public void HandleAttackImpulse() { }
        public void HandleAttackEnd() { }
        public void HandleReloadEnd()
        {
            _CombatCtx.Reloading = false;
            // 탄약 채우기
        }
        #endregion

        #region Transition API
        public bool ShouldEnterMelee() => _InputCtx.attackTap && !_InputCtx.focus;
        public bool ShouldEnterRange() => _InputCtx.attack && _CombatCtx.HasFirearm && _InputCtx.focus;
        public bool ShouldEnterReload() => _InputCtx.reloadTap && _CombatCtx.HasFirearm;
        #endregion

        #region State API
        public void EnterAttack()
        {
            _CombatCtx.AttackTimer.RestartTimer();
        }
        public void EndAction()
        {
            if (_ActiveModule == null) return;

            _ActiveModule.CleanupAction(_CombatCtx);
            _ActiveModule = null;
            _ActiveModuleType = null;
        }
        #endregion
    }

}