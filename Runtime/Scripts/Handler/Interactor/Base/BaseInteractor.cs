using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.Sensor;
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
                CastRadius = _CastRadius,
                Layermask = _InteractableMask
            };
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
        
        #region Interactor API
        public IInteractable CurrentTarget => _CurrentTarget;
        public bool HasTarget => _CurrentTarget != null;
        public virtual string GetCurrentPrompt()
        {
            if (_CurrentTarget == null) return string.Empty;
            return _CurrentTarget.GetPromptText(this);
        }
        public virtual bool CanInteract()
        {
            return HasTarget;
        }
        #endregion


        public virtual void RequestInteract()
        {
            if (_CurrentTarget == null) return;
            _CurrentTarget.Interact(this);
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
}