using Dave6.CharacterKit.GameFlow;
using Dave6.ThirdPersonCamera;
using UnityEngine;
using UnityEngine.Events;

namespace Dave6.CharacterKit.Handler.Interactor
{
    public class PlayerInteractor : BaseInteractor
    {
        ThirdPersonCameraController _CameraController;

        IInteractable _LastTarget;

        public UnityAction<string> OnShowPrompt;
        public UnityAction OnHidePrompt;

        protected override void Awake()
        {
            base.Awake();
            GameplayHub.Instance.Register(this);
        }

        public void BindCamera(ThirdPersonCameraController camera) => _CameraController = camera;

        // prompt UI 객체
        // 인풋 키 (이건 connector에서 이벤트 바인딩 하면 됨)
        // Register 패턴으로 카메라 연결..?
        public void OnUpdate()
        {
            FindTargetInteractable();

            if (_LastTarget != _CurrentTarget)
            {
                UpdateUI(_CurrentTarget);
                _LastTarget = _CurrentTarget;
            }
        }

        protected override Vector3 GetCastOrigin() => _CameraController.CameraPosition;
        protected override Vector3 GetCastDirection() => _CameraController.CameraForward;


        void UpdateUI(IInteractable target)
        {
            if (target == null)
            {
                HidePrompt();
                return;
            }
            ShowPrompt(target.GetPromptText(this));
        }

        void ShowPrompt(string prompt)
        {
            OnShowPrompt?.Invoke(prompt);
        }

        void HidePrompt()
        {
            OnHidePrompt?.Invoke();
        }
    }
}