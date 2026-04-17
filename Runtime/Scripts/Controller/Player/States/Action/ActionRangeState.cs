using Dave6.CharacterKit.Handler.Combat;
using Dave6.Foundation.GameLogic.State;

namespace Dave6.CharacterKit.Player.States
{
    public class ActionRangeState : BaseState<PlayerController>
    {
        public ActionRangeState(PlayerController controller) : base(controller) { }

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
            if (!_Controller.InputCtx.attack) return;
            _Controller.Combat.TryAction<RangeModule>();
        }

        public override bool CanOverrideBy(IState next)
        {
            return next is ActionMeleeState || next is ActionReloadState;
        }

        public override bool CanExit()
        {
            return _Controller.Combat.CheckExit(EActionExitReason.LeaseExpired);
        }
    }
}