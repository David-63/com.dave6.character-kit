// using Dave6.CharacterKit.States;
// using Dave6.Foundation.GameLogic.State;
// using UnityEngine;

// namespace Dave6.CharacterKit
// {
//     public class MinimalController : BasicPlayerController
//     {
//         public override void Start()
//         {
//             SetupStateMachine();
//             //m_Input.EnablePlayerAction();

//             m_LocomotionStateMachine.SetState(m_LocomotionStateMachine.GetStateByType(typeof(MinimalFreeLookState)));
//         }
//         protected override void SetupStateMachine()
//         {
//             if (showInitialDebug)
//             {
//                 Debug.Log("상태 초기화");
//             }
//             // FSM 생성 및 상태 정의
//             m_LocomotionStateMachine = new StateMachine();
//             var freeLook = new MinimalFreeLookState(this);
//             var strafeMove = new MinimalStrafeMoveState(this);
//             m_LocomotionStateMachine.At(freeLook, strafeMove, new FuncPredicate(() => focusInput));
//             m_LocomotionStateMachine.At(strafeMove, freeLook, new FuncPredicate(() => !focusInput));
//         }
//     }
// }