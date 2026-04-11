using System;
using System.Collections.Generic;
using Dave6.CharacterKit.AnimHandler;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Combat
{
    public abstract class BaseCombat : MonoBehaviour, IActionAnimation
    {
        protected BaseActionContext _BaseContext;
        protected Dictionary<Type, IActionModule> _Modules = new();
        protected IActionModule _ActiveModule;
        protected Type _ActiveModuleType;

        // 외부 참조
        protected AnimatorHandler _AnimHandler;

        // === 공통 쿼리 ===

        protected virtual void Awake()
        {
            _AnimHandler = GetComponent<AnimatorHandler>();
        }

        #region Combat API
        // === 공통 실행 API ===
        public void TryAction<T>() where T : class, IActionModule
        {
            var nextType = typeof(T);
            if (!_Modules.TryGetValue(typeof(T), out var nextModule)) return;
            
            if (_ActiveModule != null && _ActiveModuleType != nextType)
            {
                SetExit(EActionExitReason.Chained);
                _ActiveModule.CleanupAction(_BaseContext);
            }

            _ActiveModule = nextModule;
            _ActiveModuleType = nextType;
            nextModule.TryAction(_BaseContext, this);
        }
        public bool ExitIs(EActionExitReason reason)
        {
            if (_BaseContext.ExitReason != reason) return false;
            _BaseContext.ExitReason = EActionExitReason.None;
            return true;
        }

        public void PlayAction(string anim, bool allowSameAnim = true)
        {
            _AnimHandler.ChangeAnimation(anim, allowSameAnim);
        }
        #endregion

        protected void SetExit(EActionExitReason reason)
        {
            if (_BaseContext.ExitReason != EActionExitReason.None) return;
            _BaseContext.ExitReason = reason;
        }
    }

    public interface IActionAnimation
    {
        void PlayAction(string anim, bool allowSameAnim = false);
    }

}