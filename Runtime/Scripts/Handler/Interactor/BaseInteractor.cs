using System;
using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.Sensor;
using Dave6.ThirdPersonCamera;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Interactor
{
    /// <summary>
    /// 하는 일:
    /// - 후보 목록 관리
    /// - 현재 대상 결정
    /// - 입력 시 호출
    /// </summary>
    public abstract class BaseInteractor : MonoBehaviour, IInteractor
    {
        public Transform Origin => transform;

        [Header("Detection")]
        [SerializeField] protected float _CastLength = 4f;
        [SerializeField] protected float _CastRadius = 0.25f;
        [SerializeField] protected LayerMask _InteractableMask;

        protected readonly List<IInteractable> _Interactables = new();
        protected IInteractable _CurrentTarget;
        protected RaycastSensor2 _Sensor;
        
        protected virtual void Awake()
        {
            InitializeSensor();
        }
        protected virtual void Tick()
        {
            FindTargetInteractable();
        }

        protected virtual void InitializeSensor()
        {
            _Sensor = new RaycastSensor2(transform)
            {
                CastLength = _CastLength,
                Layermask = _InteractableMask
            };
            _Sensor.SetRadius(_CastRadius);
        }
        protected virtual void FindTargetInteractable()
        {
            _CurrentTarget = null;
            _Sensor.SetCastOrigin(GetCastOrigin());
            _Sensor.SetCastDirection(GetCastDirection());

            _Sensor.Cast();
            if (!_Sensor.HasDetecteHit()) return;
            if (!_Sensor.GetCollider().TryGetComponent<IInteractable>(out var interactable)) return;
            if (!_Interactables.Contains(interactable)) return;
            if (!interactable.CanInteract(this)) return;

            _CurrentTarget = interactable;
        }


        public virtual void RequestInteract()
        {
            if (_CurrentTarget == null) return;
            _CurrentTarget.Interact(this);
        }
        public virtual string GetCurrentPrompt()
        {
            if (_CurrentTarget == null) return "Empty";
            return _CurrentTarget.GetPromptText(this);
        }
        protected abstract Vector3 GetCastOrigin();

        protected abstract Vector3 GetCastDirection();

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            if (!_Interactables.Contains(interactable))
            {
                _Interactables.Add(interactable);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;

            _Interactables.Remove(interactable);

            if (_CurrentTarget == interactable)
            {
                _CurrentTarget = null;
            }
        }
    }

    public class PlayerInteractor : BaseInteractor
    {
        ThirdPersonCameraController _CameraController;

        // prompt UI 객체
        // 인풋 키 (이건 connector에서 이벤트 바인딩 하면 됨)
        // Register 패턴으로 카메라 연결..?
        public void OnUpdate()
        {
            Tick();

            HandleInput();
        }

        protected override Vector3 GetCastOrigin() => _CameraController.CameraPosition;
        protected override Vector3 GetCastDirection() => _CameraController.CameraForward;
        protected virtual void HandleInput()
        {
            // if (_Input == null) return;

            // if (_Input.interactTap)
            // {
            //     RequestInteract();
            // }
        }
    }
}