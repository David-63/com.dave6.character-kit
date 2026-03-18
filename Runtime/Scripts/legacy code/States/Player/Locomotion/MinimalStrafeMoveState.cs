// using Dave6.Foundation.GameLogic.State;
// using UnityEngine;

// namespace Dave6.CharacterKit.States
// {
//     public class MinimalStrafeMoveState : BaseState<BasicPlayerController>
//     {
//         bool ShiftToggle = false;
//         const float m_RotateDuration = 2.0f;

//         public MinimalStrafeMoveState(BasicPlayerController controller) : base(controller) { }

//         public override void OnEnter()
//         {
//             ShiftToggle = true;
//             controller.cameraHandler.SetStrafeMode(ShiftToggle);
//             controller.GetInputReader().ShiftTap += OnShiftToggled;
//         }

//         public override void OnExit()
//         {
//             controller.GetInputReader().ShiftTap -= OnShiftToggled;
//         }

//         public override void Update()
//         {
//             float deltaTime = Time.deltaTime;
//             UpdateTargetSpeed();
//             controller.animatorHandler.UpdateMoveInput(controller.horizontalSpeed, controller.HasMovementInput());
//             float targetRotation = controller.mover.CalcTargetYawByCamera();
//             float rotation = controller.mover.SmoothYawUpdate(targetRotation, deltaTime);
//             controller.mover.ApplyCharacterRotation(rotation);
//             controller.moveDirection = controller.mover.CalcMoveDirByCamera(deltaTime);

//             controller.mover.SmoothInputDirection(deltaTime);
//             controller.animatorHandler.UpdateDirection(controller.cachedInputDir);
//         }

//         void UpdateTargetSpeed()
//         {
//             float targetSpeed = 0;

//             if (controller.HasMovementInput())
//             {
//                 targetSpeed = controller.mover.GetMovementProfile().StrafeSpeed;
//             }

//             controller.targetSpeed = targetSpeed;
//         }

//         void OnShiftToggled()
//         {
//             ShiftToggle = !ShiftToggle;
//             controller.cameraHandler.SetStrafeMode(ShiftToggle);
//         }
//     }
// }