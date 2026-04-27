using Dave6.CharacterKit.Handler.Combat;
using Dave6.Foundation.GameLogic.State;

namespace Dave6.CharacterKit.Player.States
{
    public class ActionReloadState : BaseState<PlayerController>
    {
        public ActionReloadState(PlayerController controller) : base(controller) { }
        public override void OnEnter()
        {
            _Controller.Combat.TryAction<ReloadModule>();
        }
        public override void OnExit()
        {
            _Controller.Combat.EndAction();
            _Controller.Combat.ConsumeExit();
        }
        // 단계별 재장전 구현하면 추가할게
        // public override bool CanOverrideBy(IState next)
        // {
        //     return next is ActionMeleeState || next is ActionRangeState;
        // }
        public override bool CanExit()
        {
            return _Controller.Combat.CheckExit(EActionExitReason.LeaseExpired);
        }
    }
}