using Dave6.CharacterKit.States;
using Dave6.StateMachine;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class MinimalController : BasicPlayerController
    {
        public override void Start()
        {
            SetupStateMachine();
            m_Input.EnablePlayerAction();

            m_StateMachine.SetState(m_StateMachine.GetStateByType(typeof(MinimalFreeLookState)));
        }
        protected override void SetupStateMachine()
        {
            if (showInitialDebug)
            {
                Debug.Log("상태 초기화");
            }
            // FSM 생성 및 상태 정의
            m_StateMachine = new GameStateMachine();
            var freeLook = new MinimalFreeLookState(this);
            var strafeMove = new MinimalStrafeMoveState(this);
            At(freeLook, strafeMove, new FuncPredicate(() => aimInput));
            At(strafeMove, freeLook, new FuncPredicate(() => !aimInput));
        }
    }
}