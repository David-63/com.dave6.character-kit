using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.Sensor;
using UnityEngine;
using UnityUtils;

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
        [SerializeField] protected float _CastLength = 16f;
        [SerializeField] protected float _CastRadius = 0.25f;
        [SerializeField] protected LayerMask _InteractableMask;
        [SerializeField] protected QueryTriggerInteraction _CastInteraction;

        protected readonly List<IInteractable> _Interactables = new();
        protected IInteractable _CurrentTarget;
        protected RaycastSensor2 _Sensor;
        
        protected virtual void Awake()
        {
            InitializeSensor();
        }

        protected virtual void InitializeSensor()
        {
            _Sensor = new RaycastSensor2(transform)
            {
                CastLength = _CastLength,
                CastRadius = _CastRadius,
                Layermask = _InteractableMask,
                TriggerInteraction = _CastInteraction
            };
        }
        protected virtual void FindTargetInteractable()
        {
            _Interactables.RemoveAll(x => x == null);
            _CurrentTarget = null;

            // interactable이 한개만 있으면
            // 즉시 타겟 설정
            if (_Interactables.Count == 0) return;
            if (_Interactables.Count == 1)
            {
                var target = _Interactables[0];
                if (target.CanInteract(this)) _CurrentTarget = target;
                return;
            }

            // 여러개 있으면 cast 판정을 통해서 결정
            var origin = GetCastOrigin();
            var direction = GetCastDirection();
            _Sensor.SetCastOrigin(origin);
            _Sensor.SetCastDirection(direction);
            _Sensor.Cast();

            Debug.DrawLine(origin, origin + direction * _CastLength, Color.red);

            if (!_Sensor.HasDetecteHit()) return;

            var interactable = _Sensor.GetCollider().GetComponentInParent<IInteractable>();
            if (interactable == null) return;
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
            var interactable = other.GetComponentInParent<IInteractable>();
            if (interactable == null) return;

            if (!_Interactables.Contains(interactable))
            {
                Debug.Log("Add Interactable");
                _Interactables.Add(interactable);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            var interactable = other.GetComponentInParent<IInteractable>();
            if (interactable == null) return;

            _Interactables.Remove(interactable);
            Debug.Log("Remove Interactable");

            if (_CurrentTarget == interactable)
            {
                _CurrentTarget = null;
            }
        }
    }
}