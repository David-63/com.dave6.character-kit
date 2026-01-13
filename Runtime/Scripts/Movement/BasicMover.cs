using System;
using System.Collections;
using Dave6.CharacterKit.Look;
using Dave6.CharacterKit.Movement;
using Dave6.CharacterKit.Sensor;
using Unity.Cinemachine;
using UnityEngine;
using UnityUtils;
using UnityUtils.Timer;

namespace Dave6.CharacterKit
{
    /// <summary>
    /// 
    /// 카메라 필요함.
    /// PlayerController에서 세팅해주기
    /// </summary>
    public abstract class BasicMover : MonoBehaviour
    {
        public bool showInitialDebug = false;
        CharacterController m_Controller;
        BasicPlayerController m_BasicController; // 이건 레지스터 방식으로 해도 되고
        public BasicPlayerController controller
        {
            get => m_BasicController;
            set => m_BasicController = value;
        }

        #region collider & sensor field
        [Header("Collider Settings")]
        [Range(0f, 1f)][SerializeField] float m_StepHeightRatio = 0.14f;
        //[SerializeField] float colliderStepOffset = 0.25f;
        [SerializeField] float m_ColliderHeight = 1.8f;
        [SerializeField] float m_ColliderRadius = 0.28f;
        [SerializeField] Vector3 m_ColliderOffset = new Vector3(0, 0.5f, 0);

        RaycastSensor m_GroundChecker;

        // public float GroundedOffset = -0.14f; // 이거를 어디..에 써야할지 모르곘네, 없어도 될것같음

        bool m_IsUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions // 이것도 rigidbody에 쓰던거라 필요없을듯?
        public bool m_IsGrounded;
        public bool isGrounded => m_IsGrounded;
        float m_BaseSensorRange;
        int m_CurrentLayer;
        #endregion

        #region move & look field
        [Header("Movement Something")]
        [SerializeField] protected MovementProfile m_MovementProfile;
        public MovementProfile GetMovementProfile()
        {
            if (m_MovementProfile == null)
            {
                Debug.Log("movementProfile 세팅 안했음");
                return null;
            }
            return m_MovementProfile;
        }
        protected float m_TargetInputRotation = 0f;                          // FreeMove 상태에서 회전값 기록하는 용도
        public float targetInputRotation => m_TargetInputRotation;
        protected float m_LastTargetInputRotation;
        public float lastTargetInputRotation => m_LastTargetInputRotation;
        protected const float m_SpeedOffset = 0.1f;
        protected const float m_TerminalVelocity = 53.0f;               // 가속 제한인듯
        protected const float m_Gravity = -15f;

        public CameraController cameraHandler { get; private set; }

        const float m_Threshold = 0.01f;                      // 입력 최소치 제한

        float m_CurrentYaw;
        float m_CurrentPitch;

        public Quaternion characterAim {get; private set;}

        
        #endregion

        #region jump field

        [Header("Jump settings")]
        [SerializeField] float m_JumpDuration = 0.2f;

        Countdown m_JumpTimer;
        #endregion

        #region Initialize & Sensor setting
        /// <summary>
        /// 센서와 콜라이더 세팅
        /// </summary>
        protected virtual void Awake()
        {
            Setup();
            RecalculateColliderDimensions();
        }

        

        void OnValidate()
        {
            if (gameObject.activeInHierarchy)
            {
                RecalculateColliderDimensions();
            }
        }

        void RecalculateColliderDimensions()
        {
            if (m_Controller == null)
            {
                Setup();
            }

            float stepOffset = m_ColliderHeight * m_StepHeightRatio;

            m_Controller.stepOffset = stepOffset;
            m_Controller.skinWidth = m_ColliderRadius / 10f;
            m_Controller.center = m_ColliderOffset * m_ColliderHeight;
            m_Controller.radius = m_ColliderRadius;
            m_Controller.height = m_ColliderHeight;

            RecalibrateSensor();
        }
        void RecalibrateSensor()
        {
            m_GroundChecker ??= new RaycastSensor(transform);

            m_GroundChecker.SetCastOrigin(m_Controller.bounds.center);
            m_GroundChecker.SetCastDirection(RaycastSensor.CastDirection.Down);
            m_GroundChecker.SetRadius(m_ColliderRadius);
            RecalculateSensorLayerMask();

            const float safetyDistanceFactor = 0.01f; // Small factor added to prevent clipping issues when the sensor range is calcuatetd
            float length = m_ColliderHeight * (1f - m_StepHeightRatio) * 0.5f + m_ColliderHeight * m_StepHeightRatio;
            m_BaseSensorRange = length * (1f + safetyDistanceFactor) * transform.localScale.x;
            m_GroundChecker.castLength = length * transform.localScale.x;
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
            m_GroundChecker.layermask = layerMask;
            m_CurrentLayer = objectLayer;
        }
        public void CheckForGround()
        {
            if (m_CurrentLayer != gameObject.layer)
            {
                RecalculateSensorLayerMask();
            }

            m_GroundChecker.castLength = m_BaseSensorRange;
            m_GroundChecker.SphereCast();

            m_IsGrounded = m_GroundChecker.HasDetecteHit();

            controller.animatorHandler.UpdateGrounded(isGrounded, controller.verticalSpeed);
        }

        
        public Vector3 GetGroundNormal() => m_GroundChecker.GetNormal();
        public void SetExtendedSensorRange(bool isExtended) => m_IsUsingExtendedSensorRange = isExtended;
        #endregion

        #region Setup
        protected virtual void Setup()
        {
            if (showInitialDebug)
            {
                Debug.Log("내장된 컴포넌트 초기화");
            }
            m_Controller = gameObject.GetOrAddComponent<CharacterController>();
            if (m_JumpTimer == null)
            {
                m_JumpTimer = new Countdown(m_JumpDuration);
            }
        }

        public virtual void RegisterMover(BasicPlayerController playerController, CameraController camera)
        {
            controller = playerController;
            cameraHandler = camera;
        }
        #endregion

        #region GameFlow
        public void ResetMover()
        {
            
        }

        #endregion


        public virtual void OnUpdate()
        {
            Jump();
            ApplyGravity();
            CheckForGround();
            UpdateFinalSpeed();
            ApplyMovement();
        }

        #region 속도 계산
        public virtual void CalcBaseSpeed(float deltaTime)
        {
            if (controller.mover.isGrounded)
            {
                if (Mathf.Abs(controller.baseSpeed - controller.targetSpeed) > m_SpeedOffset)
                {
                    controller.baseSpeed = Mathf.Lerp(controller.baseSpeed, controller.targetSpeed, deltaTime * m_MovementProfile.SpeedChangeRate);
                    controller.baseSpeed = Mathf.Round(controller.baseSpeed * 1000f) / 1000f;
                }
                else
                {
                    controller.baseSpeed = controller.targetSpeed;
                }
            }
            else
            {
                float airPenalty = 0.5f;
                controller.baseSpeed = Mathf.Lerp(controller.baseSpeed, 0, deltaTime * airPenalty);
                controller.baseSpeed = Mathf.Round(controller.baseSpeed * 1000f) / 1000f;
            }
        }

        public virtual void CalcImpulseSpeed(float deltaTime)
        {
            if (Mathf.Abs(controller.impulseSpeed) > m_SpeedOffset)
            {
                controller.impulseSpeed = Mathf.Lerp(controller.impulseSpeed, 0, deltaTime * m_MovementProfile.SpeedChangeRate);
            }
            else
            {
                controller.impulseSpeed = 0;
            }
        }

        public virtual void UpdateFinalSpeed()
        {
            float deltaTime = Time.deltaTime;
            CalcBaseSpeed(deltaTime);
            CalcImpulseSpeed(deltaTime);
            controller.horizontalSpeed = controller.baseSpeed + controller.impulseSpeed;
        }
        #endregion


        #region 캐릭터 회전 계산
        /// <summary>
        /// 입력기준 캐릭터 회전 방향 계산
        /// </summary>
        /// <returns> 계산된 목표 yaw 회전값 </returns>
        public virtual float CalcTargetYawByInput()
        {
            m_TargetInputRotation = Mathf.Atan2(controller.inputMove.x, controller.inputMove.z) * Mathf.Rad2Deg + cameraHandler.yawAngle;
            m_LastTargetInputRotation = m_TargetInputRotation;
            return m_TargetInputRotation;
        }
        /// <summary>
        /// 카메라기준 캐릭터 회전 방향 계산
        /// </summary>
        /// <returns> 카메라 기준 yaw </returns>
        public virtual float CalcTargetYawByCamera()
        {
            float targetYaw = cameraHandler.aimAnchor.eulerAngles.y;
            return targetYaw;
        }
        public virtual float CalcTargetYawByAimPoint()
        {
            // 1. 방향 벡터
            Vector3 toAim = cameraHandler.baseAimPoint - controller.transform.position;
            // 2. Y축 제거
            toAim.y = 0f;

            // 너무 가까운 경우 예외처리
            if (toAim.sqrMagnitude < 0.001f)
            {
                return controller.transform.eulerAngles.y;
            }
            // 3. 정규화
            Vector3 aimDirection = toAim.normalized;
            float targetYaw = Mathf.Atan2(aimDirection.x, aimDirection.z) * Mathf.Rad2Deg;

            return targetYaw;
        }
        public virtual float CalcTargetPitchByAimPoint(Vector3 beginPoint)
        {
            Vector3 toAim = cameraHandler.baseAimPoint - beginPoint;

            if (toAim.sqrMagnitude < 0.001f)
            {
                return m_CurrentPitch;
            }
            Vector3 aimDirection = toAim.normalized;

            float targetPitch = -Mathf.Atan2(aimDirection.y, new Vector2(aimDirection.x, aimDirection.z).magnitude) * Mathf.Rad2Deg;
            return targetPitch;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetYaw">Last Target Rotation</param>
        /// <param name="currentVelocity">Ref</param>
        /// <returns></returns>
        public virtual float SmoothYawUpdate(float targetYaw, ref float currentVelocity)
        {
            m_CurrentYaw = Mathf.SmoothDampAngle(m_CurrentYaw, targetYaw, ref currentVelocity, 0.05f);
            return m_CurrentYaw;
        }
        /// <summary>
        /// SmoothDamp를 사용하여 Yaw 보간 (정해진 시간에 수렴)
        /// </summary>
        /// <param name="targetYaw">목표 Yaw</param>
        /// <param name="smoothTime">목표값 도달에 걸리는 시간</param>
        /// <returns>보간된 Yaw 각도</returns>
        public virtual float SmoothYawUpdate(float targetYaw, float deltaTime)
        {
            m_CurrentYaw = Mathf.LerpAngle(m_CurrentYaw, targetYaw, m_MovementProfile.DirRotationSpeed * deltaTime);
            return m_CurrentYaw;
        }
        /// <summary>
        /// LerpAngle을 사용하여 Pitch 보간 (비율 기반)
        /// </summary>
        /// <param name="targetPitch"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public virtual float SmoothPitchUpdate(float targetPitch, float deltaTime)
        {
            m_CurrentPitch = Mathf.LerpAngle(m_CurrentPitch, targetPitch, m_MovementProfile.DirRotationSpeed * deltaTime);
            return m_CurrentPitch;
        }
        /// <summary>
        /// 캐릭터에 회전값 적용
        /// </summary>
        /// <param name="rotation">캐릭터가 바라볼 방향</param>
        public virtual void ApplyCharacterRotation(float rotation)
        {
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }

        public Quaternion CharacterAimUpdate()
        {
            return characterAim = Quaternion.Euler(m_CurrentPitch, m_CurrentYaw, 0f);
        }
        #endregion


        #region 이동 방향 계산
        /// <summary>
        /// 입력에 따라 캐릭터가 이동해야 할 방향 벡터를 계산.
        /// 공중에서는 회전을 완화함.
        /// </summary>
        /// <param name="rotation">입력 기반으로 계산된 캐릭터의 Yaw 회전값.</param>
        /// <returns>transform에 적용할 이동 방향.</returns>
        public virtual Vector3 CalcMoveDirByInput(float rotation, float deltaTime)
        {
            float changeSpeed = 1f;
            float baseRotation = m_IsGrounded ? rotation : m_TargetInputRotation;
            float lerpRotation = baseRotation;
            float currentYaw = GetCurrentYaw();

            if (!m_IsGrounded && controller.moveDirection.sqrMagnitude > 0.001f)
            {
                lerpRotation = Mathf.LerpAngle(currentYaw, baseRotation, deltaTime * changeSpeed);
            }

            return Quaternion.Euler(0.0f, lerpRotation, 0.0f) * Vector3.forward;
        }
        /// <summary>
        /// 카메라의 Yaw를 기준으로 캐릭터가 이동해야 할 방향 벡터를 계산
        /// </summary>
        /// <returns>transform에 적용할 이동 방향</returns>
        public virtual Vector3 CalcMoveDirByCamera(float deltaTime)
        {
            // 입력 없으면 방향은 0,0,0
            if (!controller.HasMovementInput()) return Vector3.zero;

            // 입력 기반 카메라 방향
            Vector3 cameraDirection = Quaternion.Euler(0f, cameraHandler.yawAngle, 0f) * controller.inputMove;
            cameraDirection.Normalize();

            // 목표 Yaw
            float targetYaw = Mathf.Atan2(cameraDirection.x, cameraDirection.z) * Mathf.Rad2Deg;
            // 현재 Yaw
            float currentYaw = GetCurrentYaw();

            // lerp 속도 조절
            float lerpSpeed = m_IsGrounded ? m_MovementProfile.DirRotationSpeed : m_MovementProfile.DirRotationSpeed / 3f;
            float lerpRotation = Mathf.LerpAngle(currentYaw, targetYaw, deltaTime * lerpSpeed);

            return Quaternion.Euler(0f, lerpRotation, 0f) * Vector3.forward;
        }

        float GetCurrentYaw()
        {
            return Mathf.Atan2(controller.moveDirection.x, controller.moveDirection.z) * Mathf.Rad2Deg;
        }

        public void SmoothInputDirection(float deltaTime)
        {
            Vector3 targetInputDir = controller.inputMove;
            Vector3 currentAnimDir = controller.cachedInputDir;
            
            // 보간
            Vector3 lerpInputDir = Vector3.Lerp(currentAnimDir, targetInputDir, deltaTime * m_MovementProfile.DirRotationSpeed);
            controller.cachedInputDir = lerpInputDir;
        }
        #endregion

        #region 내부 물리 로직
        protected void ApplyGravity()
        {
            if (m_IsGrounded)
            {
                // stop our velocity dropping infinitely when grounded
                if (controller.verticalSpeed < 0.0f)
                {
                    controller.verticalSpeed = m_MovementProfile.GroundGravity;
                }
            }
            controller.animatorHandler.UpdateVerticalSpeed(controller.verticalSpeed);
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (controller.verticalSpeed < m_MovementProfile.TerminalVelocity)
            {
                controller.verticalSpeed += m_MovementProfile.AirborneGravity * Time.deltaTime;
            }
        }

        protected void ApplyMovement()
        {
            Vector3 velocity = controller.moveDirection * controller.horizontalSpeed + Vector3.up * controller.verticalSpeed;
            m_Controller.Move(velocity * Time.deltaTime);
        }

        // 이상태로는 외부 호츨이 딱히 필요없이 알아서 작동하긴할듯
        void Jump()
        {
            if (!m_IsGrounded) return;

            if (controller.jumpInput && CanJump())
            {
                m_JumpTimer.RestartTimer();
                controller.verticalSpeed = Mathf.Sqrt(m_MovementProfile.JumpHeight * -2f * m_MovementProfile.AirborneGravity);
            }
        }

        bool CanJump()
        {
            return m_JumpTimer.IsFinished;
        }
        #endregion
    }
}
