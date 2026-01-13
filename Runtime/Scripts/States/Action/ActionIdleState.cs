using Dave6.StateMachine;
using UnityEngine;

namespace Dave6.CharacterKit.States
{
    public class ActionIdleState : BaseState<PlayerCharacter>
    {
        public ActionIdleState(PlayerCharacter controller) : base(controller) { }

        public override void OnEnter() { }
        public override void OnExit() { }
        public override  void Update() { }
    }
}
