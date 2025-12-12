using Dave6.StateMachine;
using UnityEngine;

namespace Dave6.CharacterKit.States
{
    public class ActionIdleState : BaseState<PlayerController>
    {
        public ActionIdleState(PlayerController controller) : base(controller) { }

        public override void OnEnter()
        {
            Debug.Log("대기상태 진입");
        }

        public override void OnExit() { }

        public override  void Update()
        {
            if (controller.attackInput) controller.enterAttackFlag = true;
        }
    }
}
