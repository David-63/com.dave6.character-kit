using Dave6.CharacterKit.Handler.Combat;
using Dave6.Foundation.GameLogic.State;

namespace Dave6.CharacterKit.Player.States
{
    public class ActionReloadState : BaseState<PlayerController>
    {
        public ActionReloadState(PlayerController controller) : base(controller) { }
        public override void OnEnter()
        {
        }
        public override void Update()
        {
            if (!_Controller.InputCtx.reloadTap) return;
            _Controller.Combat.TryAction<ReloadModule>();
        }
    }
}