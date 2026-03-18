// using Dave6.CharacterKit.Item;
// using Dave6.Foundation.GameLogic.State;
// using UnityEngine;
// using UnityUtils.Timer;

// namespace Dave6.CharacterKit.States
// {
//     // 
//     public class ActionRangeState : BaseState<PlayerCharacter>
//     {
//         public ActionRangeState(PlayerCharacter controller) : base(controller) { }
//         public override void OnEnter()
//         {
//             controller.attackTimer.RestartTimer();
//         }

//         public override void OnExit() { }

//         public override void Update()
//         {
//             controller.EvaluateAttackExit(false);
//             AttackInput();
//         }

//         void AttackInput()
//         {
//             // 조건을 tap이 아니라 hold로 두고 내부에 RPM을 둬서 제어하는 방식으로 변경하기
//             if (!controller.attackInput) return;

//             controller.combatHandler.TryFireProjectile();
//         }
//     }
// }
