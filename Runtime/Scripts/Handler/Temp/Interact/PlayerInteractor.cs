using Dave6.ThirdPersonCamera;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Interact
{
    // 선택
    public class PlayerInteractor : BaseInteractor
    {
        PlayerInputContext m_InputCtx;
        ThirdPersonCameraController m_CameraController;

        [SerializeField] float m_MaxDistance = 6f;
        [SerializeField] float m_SphereRadius = 0.25f;
        [SerializeField] LayerMask m_InteractableLayerMask;

        internal void BindInput(PlayerInputContext inputCtx) => m_InputCtx = inputCtx;
        internal void BindCamera(ThirdPersonCameraController camera) => m_CameraController = camera;

        public void OnUpdate()
        {
            FindTargetInteractable();

            // 탐색
            if (m_InputCtx.interactTap)
            {
                m_TargetInteractable?.Interact(this);
                m_TargetInteractable = null;
            }
        }

        void FindTargetInteractable()
        {
            m_TargetInteractable = null;

            var origin = m_CameraController.transform.position;
            var dir = m_CameraController.CameraForward;

            if (!Physics.SphereCast(origin, m_SphereRadius, dir, out RaycastHit hit, m_MaxDistance, ~0, QueryTriggerInteraction.Collide)) return;
            // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // if (!Physics.Raycast(ray, out RaycastHit hit, 100f)) return;

            if (!hit.collider.TryGetComponent<IInteractable>(out var interactable)) return;

            if (!m_Interactables.Contains(interactable)) return;

            m_TargetInteractable = interactable;
        }
    }
}