using Dave6.Foundation.GameLogic.State;

namespace Dave6.CharacterKit.Player.States
{
    public class ActionIdleState : BaseState<PlayerController>
    {
        public ActionIdleState(PlayerController controller) : base(controller) { }
        public override void OnEnter()
        {
        }
    }

}