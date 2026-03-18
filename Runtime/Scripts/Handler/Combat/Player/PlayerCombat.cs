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
            float attackDuration = 3f;
            _CombatCtx.AttackTimer = new Countdown(attackDuration);

            _Modules.Add(typeof(MeleeModule), new MeleeModule());
            _Modules.Add(typeof(RangeModule), new RangeModule());
            _Modules.Add(typeof(ReloadModule), new ReloadModule());
        }

        internal void BindInput(PlayerInputContext inputCtx) => _InputCtx = inputCtx;

        #endregion

        public void OnUpdate()
        {
            
        }

        #region Bind Event
        public void HandleAttackImpulse()
        {

        }
        public void HandleAttackEnd()
        {
            
        }
        public void HandleReloadEnd()
        {
            _CombatCtx.Reloading = false;
            // 탄약 채우기

        }

        #endregion

        #region Transition API

        public bool IsMeleeState() => _InputCtx.attackTap && !_InputCtx.focus;
        public bool IsRangeState() => _CombatCtx.HasFirearm && _InputCtx.focus;
        public bool CanReload() => _CombatCtx.HasFirearm && _InputCtx.reloadTap;
        public bool ReloadFinished() => !_CombatCtx.Reloading;

        #endregion

        #region State API
        public void EnterAttack()
        {
            _CombatCtx.AttackTimer.RestartTimer();
        }
        public void EvaluateActionExit()
        {
            // 이미 종료 조건에 해당하면 스킵
            if (_CombatCtx.ExitReason != EActionExitReason.None) return;
            if (_ActiveModule == null) return;
            if (_ActiveModule.EvaluateExit(_CombatCtx, out var reason))
            {
                _CombatCtx.ExitReason = reason;
            }
        }
        #endregion
    }

}