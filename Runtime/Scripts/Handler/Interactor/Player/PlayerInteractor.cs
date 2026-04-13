using Dave6.ThirdPersonCamera;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Interactor
{
    public class PlayerInteractor : BaseInteractor
    {
        ThirdPersonCameraController _CameraController;

        public void BindCamera(ThirdPersonCameraController camera) => _CameraController = camera;

        // prompt UI 객체
        // 인풋 키 (이건 connector에서 이벤트 바인딩 하면 됨)
        // Register 패턴으로 카메라 연결..?
        public void OnUpdate()
        {
            Tick();
        }

        protected override Vector3 GetCastOrigin() => _CameraController.CameraPosition;
        protected override Vector3 GetCastDirection() => _CameraController.CameraForward;
    }
}