// using System.Collections.Generic;
// using Dave6.CharacterKit.CameraControl;
// using Dave6.CharacterKit.Handler.Mover;
// using UnityEngine;

// namespace Dave6.CharacterKit.Handler.Camera
// {
//     /// <summary>
//     /// 당장은 필요없어서 안쓰는게 나을듯
//     /// </summary>
//     public class CameraHandler : MonoBehaviour, ICameraOutput
//     {
//         ThirdPersonCameraController m_CameraSystem;
//         CameraAction m_Action;
//         CameraContext m_Context;

//         float aimYaw;
//         float aimPitch;

//         float cameraYaw;
//         float cameraPitch;
//         float characterYaw;
//         float characterPitch;

//         // 제공 필드
//         public float referenceYaw => aimYaw;
//         public Vector3 cameraForward => m_CameraSystem.transform.forward;


//         // List<CameraShake> activeShackes = new();
//         // CameraSway cameraSway;

//         IMoveMode m_MoveMode;

//         public void OnUpdate()
//         {
//             float deltaTime = Time.deltaTime;
//             var cameraCtx = m_CameraSystem.cameraCtx;
//             float originPitch = m_CameraSystem.cameraCtx.inputPitch;

//             // 캐릭터 회전 구현
//             // Mover에서 진행한것과 비슷하게 하면 될듯

//             // 1. 카메라 yaw 가져오기
//             float originYaw = m_CameraSystem.cameraCtx.inputYaw;

//             // 2. mode에 맞춰서 targetYaw 계산
//             // 2-1) playerMoverContext가 필요함

//             var moverInput = new MoverFrameInput(deltaTime, originYaw, cameraForward);

//             //float targetYaw = m_MoveMode.ResolveFacing(moverInput);
//             // 3. curYaw 값을 targetYaw로 보간

//             // 4. 적용


//             characterYaw = originYaw;
//             characterPitch = originPitch;

//             // 카메라 회전 구현

//             // 1. 원본 회전값 추출

//             // 2. shake offset 누적

//             // 3. 적용

//             // 오프셋 변수를 만들어서 각 shake를 누적시킨 후 aim에 합한 값을 final로 전달하는 방식

//             // // sway 적용
//             // if (cameraSway != null)
//             // {
//             //     cameraSway.UpdateShake(deltaTime, out float swayYaw, out float swayPitch);
//             //     yawOffset += swayYaw;
//             //     pitchOffset += swayPitch;
//             // }

//             // // kick 적용
//             // for (int i = activeShackes.Count - 1; i >= 0; i--)
//             // {
//             //     var shake = activeShackes[i];
//             //     shake.UpdateShake(deltaTime, )
//             // }
//             cameraYaw = originYaw;
//             cameraPitch = originPitch;
//             cameraCtx.finalYaw = cameraYaw;
//             cameraCtx.finalPitch = cameraPitch;
//         }

//         #region Camera API

//         public void Bind(ThirdPersonCamera cameraSystem)
//         {
//             m_CameraSystem = cameraSystem;
//         }
//         public void OnLook(Vector2 delta)
//         {
//             m_Context.lookDelta = delta;
//         }
//         public void OnFocus(bool pressed)
//         {
            
//         }
//         public void SetAim()
//         {
            
//         }

//         public void Kick()
//         {
            
//         }
//         public void Sway()
//         {
            
//         }
//         #endregion

//         [SerializeField] float aimLagSpeed = 6f;

//         void UpdateAimLag(float deltaTime)
//         {
//             // input 원본



//             // 카메라 회전 계산





//             // 캐릭터 회전 계산


//             float targetYaw = m_CameraSystem.cameraCtx.inputYaw;
//             float targetPitch = m_CameraSystem.cameraCtx.inputPitch;

//             aimYaw = Mathf.Lerp(aimYaw, targetYaw, 1 - Mathf.Exp(-aimLagSpeed * deltaTime));
//             aimPitch = Mathf.Lerp(aimPitch, targetPitch, 1 - Mathf.Exp(-aimLagSpeed * deltaTime));

//             //targetYaw = m_MoveMode.ResolveFacing(ctx, new MoverFrameInput(deltaTime, m_CameraSystem.cameraCtx.inputYaw, cameraForward));

//         }
//     }

    
//     public readonly struct CameraFrameInput
//     {
//         public readonly float DeltaTime;
//         public readonly float CurYaw;
//         public readonly float YawVelocity;
//     }

//     public class CameraContext
//     {
//         public Vector2 lookDelta;
//     }
//     public class CameraAction
//     {
//         // float ApplyYawLag(CameraFrameInput input, float deltaTime)
//         // {
//         //     // 입력 변화량 (각속도 개념)
//         //     float yawDelta = Mathf.DeltaAngle(input.CurYaw, ctx.inputYaw);
//         //     float yawSpeed = Mathf.Abs(yawDelta) / Mathf.Max(deltaTime, 0.0001f);
//         // }
//     }

// }