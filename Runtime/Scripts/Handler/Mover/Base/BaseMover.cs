using Dave6.CharacterKit.Sensor;
using UnityEngine;
using UnityUtils;

namespace Dave6.CharacterKit.Handler.Mover
{
    /// <summary>
    /// Unity runtime layer
    /// 
    /// </summary>
    public abstract class BaseMover : MonoBehaviour
    {
        protected BaseMoverContext _BaseContext;
        protected BaseMoverAction _BaseAction;
        [Header("Movement Config")]
        [SerializeField] protected BaseMoverConfig _BaseConfig;
        #region 충돌체 필드
        CharacterController _Controller;
        [Header("Collider Settings")]
        [Range(0f, 1f)][SerializeField] float _StepHeightRatio = 0.14f;
        //[SerializeField] float colliderStepOffset = 0.25f;
        [SerializeField] float _ColliderHeight = 1.8f;
        [SerializeField] float _ColliderRadius = 0.28f;
        [SerializeField] Vector3 _ColliderOffset = new Vector3(0, 0.5f, 0);
        RaycastSensor _GroundChecker;
        RaycastSensor2 _GroundChecker2;
        int _CurrentLayer;
        float _BaseSensorRange;

        public bool IsGrounded => _BaseContext.IsGrounded;
        #endregion



        #region 초기화
        protected void EnsureSetup()
        {
            if (_Controller == null)
            {
                _Controller = gameObject.GetOrAddComponent<CharacterController>();
            }
            // if (_GroundChecker == null)
            // {
            //     RecalibrateSensor();
            // }
            if (_GroundChecker2 == null)
            {
                RecalibrateSensor();
            }
        }
        void OnValidate()
        {
            if (gameObject.activeInHierarchy)
            {
                RecalculateColliderDimensions();
            }
        }
        protected void RecalculateColliderDimensions()
        {
            EnsureSetup();

            float stepOffset = _ColliderHeight * _StepHeightRatio;

            _Controller.stepOffset = stepOffset;
            _Controller.skinWidth = _ColliderRadius / 10f;
            _Controller.center = _ColliderOffset * _ColliderHeight;
            _Controller.radius = _ColliderRadius;
            _Controller.height = _ColliderHeight;

            RecalibrateSensor();
        }
        void RecalibrateSensor()
        {
            // _GroundChecker ??= new RaycastSensor(transform);

            // _GroundChecker.SetCastOrigin(_Controller.bounds.center);
            // _GroundChecker.SetCastDirection(RaycastSensor.CastDirections.Down);
            // _GroundChecker.SetRadius(_ColliderRadius);
            _GroundChecker2 ??= new RaycastSensor2(transform);

            _GroundChecker2.SetCastOrigin(_Controller.bounds.center);
            _GroundChecker2.SetCastDirection(-transform.up);
            _GroundChecker2.SetRadius(_ColliderRadius);
            RecalculateSensorLayerMask();

            const float safetyDistanceFactor = 0.01f; // Small factor added to prevent clipping issues when the sensor range is calcuatetd
            float length = _ColliderHeight * (1f - _StepHeightRatio) * 0.5f + _ColliderHeight * _StepHeightRatio;
            _BaseSensorRange = length * (1f + safetyDistanceFactor) * transform.localScale.x;
            //_GroundChecker.CastLength = length * transform.localScale.x;


            _GroundChecker2.CastLength = length * transform.localScale.x;


            
        }
        void RecalculateSensorLayerMask()
        {
            int objectLayer = gameObject.layer;
            int layerMask = Physics.AllLayers;
            for (int i = 0; i < 32; i++)
            {
                if (Physics.GetIgnoreLayerCollision(objectLayer, i))
                {
                    layerMask &= ~(1 << i);
                }
            }

            int ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
            layerMask &= ~(1 << ignoreRaycastLayer);
            //_GroundChecker.Layermask = layerMask;
            _GroundChecker2.Layermask = layerMask;
            _CurrentLayer = objectLayer;
        }
        #endregion
        
        #region 계산식 API
        protected void CheckForGround()
        {
            if (_CurrentLayer != gameObject.layer)
            {
                RecalculateSensorLayerMask();
            }

            // _GroundChecker.CastLength = _BaseSensorRange;
            // _GroundChecker.SphereCast();

            _GroundChecker2.CastLength = _BaseSensorRange;
            _GroundChecker2.Cast();

            //_BaseContext.IsGrounded = _GroundChecker.HasDetecteHit();
            _BaseContext.IsGrounded = _GroundChecker2.HasDetecteHit();
        }
        protected void ApplyGravity()
        {
            _BaseAction.CalculateGravity(_BaseContext, _BaseConfig, Time.deltaTime);
        }
        protected void UpdateFinalSpeed()
        {
            _BaseAction.CalculateSpeed(_BaseContext, _BaseConfig, Time.deltaTime);
        }
        protected void ApplyMovement()
        {
            _Controller.Move(_BaseAction.GetVelocity(_BaseContext) * Time.deltaTime);
        }
        #endregion

        #region 외부 제공 API
        public Vector3 GetMoveDirection() => _BaseContext.MoveDirection;

        public void CancelMoveSpeed() => _BaseContext.BaseSpeed = 0f;
        #endregion
    }
}