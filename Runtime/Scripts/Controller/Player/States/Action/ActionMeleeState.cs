using Dave6.CharacterKit.Handler.Combat;
using Dave6.Foundation.GameLogic.State;

namespace Dave6.CharacterKit.Player.States
{
    public class ActionMeleeState : BaseState<PlayerController>
    {
        public ActionMeleeState(PlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            _Controller.Combat.EnterAttack();
        }

        public override void Update()
        {
            _Controller.Combat.EvaluateActionExit();
            if (!_Controller.InputCtx.attackTap) return;
            _Controller.Combat.TryAction<MeleeModule>();
        }
    }

}