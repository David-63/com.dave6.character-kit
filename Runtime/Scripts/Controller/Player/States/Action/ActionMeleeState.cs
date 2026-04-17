using Dave6.CharacterKit.Handler.Combat;
using Dave6.Foundation.GameLogic.State;
using UnityEngine;

namespace Dave6.CharacterKit.Player.States
{
    public class ActionMeleeState : BaseState<PlayerController>
    {
        public ActionMeleeState(PlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            _Controller.Combat.EnterAttack();
        }
        public override void OnExit()
        {
            _Controller.Combat.EndAction();
            _Controller.Combat.ConsumeExit();
        }

        public override void Update()
        {
            if (!_Controller.InputCtx.attackTap) return;
            _Controller.Combat.TryAction<MeleeModule>();
        }

        public override bool CanOverrideBy(IState next)
        {
            return next is ActionRangeState || next is ActionReloadState;
        }

        public override bool CanExit()
        {
            return _Controller.Combat.CheckExit(EActionExitReason.LeaseExpired);
        }
    }

}