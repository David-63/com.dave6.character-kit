using Dave6.Foundation.GameLogic.State;

namespace Dave6.CharacterKit.Player.States
{
    public class ActionInteractState : BaseState<PlayerController>
    {
        public ActionInteractState(PlayerController controller) : base(controller) { }
        public override void OnEnter()
        {
            // 여기서 수행
            _Controller.Interactor.InteractAction();
        }
        public override void OnExit()
        {
            _Controller.Interactor.ConsumeExit();
        }
        public override bool CanExit()
        {
            return true;
        }
    }
}