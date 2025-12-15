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
    /// Mover가 작동 안하는 경우
    /// 
    /// 1. 카메라가 없어
    /// MainCamera 오브젝트가 없음
    /// MainCamera 오브젝트가 Main Camera 태그를 안달고있음
    /// 
    /// 2. 플레이어 오브젝트(부모)의 레이어가 default임
    /// 

    /// Mover가 할 일
    /// 
    /// 1. 바닥 체크  done
    /// 2. 이동 로직
    /// 3. 점프 로직
    /// 
    /// 여기서 구현해야할건 Controller 로 부터 데이터를 받아서 반환만 하는것임!!!
    /// 그니까 계산만 하고 값은 외부로 반환한다는것
    /// 
    /// </summary>
    public abstract class BasicMover : MonoBehaviour, IMover
    {
        public bool showInitialDebug = false;
        CharacterController m_Controller;
        BasicPlayerController m_BasicController; // 이건 레지스터 방식으로 해도 되고
        public BasicPlayerController controller => m_BasicController;

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
        private bool m_IsGrounded;
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
        protected float m_TargetRotation = 0f;                          // FreeMove 상태에서 회전값 기록하는 용도        
        protected float m_InterpCache;
        protected const float m_SpeedOffset = 0.1f;
        protected const float m_TerminalVelocity = 53.0f;               // 가속 제한인듯
        protected const float m_Gravity = -15f;

        [Header("Camera Setting")]
        [SerializeField] GameObject m_PlayerCameraPrefab;
        GameObject m_CinemachineObj;
        CinemachineCamera m_CinemachineCamera;
        CinemachineThirdPersonFollow m_ThirdPersonFollow;     // 아직 쓸댄 없지만 세부 세팅에 필요해보여서 추가함
        [SerializeField] CameraLookProfile m_CameraLookProfile;

        [SerializeField] float m_TransitionDuration = 0.35f;
        [SerializeField] AnimationCurve m_TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float m_CurrentFOV;
        float m_CurrentSide;
        float m_CurrentDistance;
        Coroutine m_TransitionCoroutine;


        Transform m_CameraTarget;                             // 이걸 mover 전담할지, 아니면 다른데서 쓸지에 따라 해당 변수의 위치가 바뀔 수 있음
        Transform m_MainCamera;                               // 원래 외부에 데이터 전달하려고 만든 기능인데 아직 쓸대가 없으니 그냥 private으로 설정
        public Transform mainCamera => m_MainCamera;
        public float yawAngle => m_MainCamera.eulerAngles.y;  // 위와 같음

        float m_CameraYaw = 0f;
        public float cameraYaw => m_CameraYaw;
        float m_CameraPitch = 0f;
        const float m_Threshold = 0.01f;                      // 입력 최소치 제한
        
        #endregion

        #region jump field

        [Header("Jump settings")]
        [SerializeField] float m_JumpDuration = 0.2f;

        Countdown m_JumpTimer;


        #endregion

        #region Initialize & Sensor setting
        protected virtual void Awake()
        {
            Setup();
            RecalculateColliderDimensions();
            OnceSetting();
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

            controller.animatorHandler.UpdateGrounded();
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
            m_BasicController = GetComponent<BasicPlayerController>();


            if (!Camera.main)
            {
                Debug.Log("Main Camera 태그 달아야함");
            }

            m_MainCamera = Camera.main.transform;

            // 없으면 추가해줌
            Camera.main.gameObject.GetOrAddComponent<CinemachineBrain>();
        }
        protected virtual void OnceSetting()
        {
            if (showInitialDebug)
            {
                Debug.Log("카메라 객체 초기화");
            }
            if (m_CameraTarget == null)
            {
                m_CameraTarget = transform.Find("CameraTarget");
                m_CameraTarget.localPosition = new Vector3(0, 1.4f, 0);
            }
            if (m_CameraLookProfile == null)
            {
                Debug.Log("카메라 프로파일 세팅 안돼있음");
            }

            if (m_PlayerCameraPrefab == null)
            {
                Debug.Log("카메라 프리팹 세팅 안돼있음");
            }
            m_CinemachineObj = Instantiate(m_PlayerCameraPrefab, transform);
            m_CinemachineCamera = m_CinemachineObj.GetComponent<CinemachineCamera>();
            m_ThirdPersonFollow = m_CinemachineObj.GetComponent<CinemachineThirdPersonFollow>();
            if (m_CameraTarget == null)
            {
                Debug.Log("카메라 타겟을 못찾음 (이름 불일치?)");
            }

            m_CinemachineCamera.Follow = m_CameraTarget;

            m_ThirdPersonFollow.Damping = Vector3.zero;
            m_ThirdPersonFollow.ShoulderOffset = new Vector3(1,0,0);
            m_ThirdPersonFollow.VerticalArmLength = 0;
            m_ThirdPersonFollow.AvoidObstacles.Enabled = true;
            m_ThirdPersonFollow.AvoidObstacles.DampingFromCollision = 0.2f;
            m_ThirdPersonFollow.AvoidObstacles.DampingIntoCollision = 0.2f;

            m_JumpTimer = new Countdown(m_JumpDuration);
        }
        #endregion

        protected virtual void Update()
        {
            Jump();
            ApplyGravity();
            CheckForGround();
            ApplyMovement();
        }

        protected virtual void LateUpdate()
        {
            LookRotation();
        }

        #region 외부 호출 함수들
        public void SetFreeLookMode() => StartTransition(55f, 0.75f, 3f);
        public void SetStrafeMode(bool shift)
        {
            if (shift)
            {
                StartTransition(65f, 0.85f, 1.2f);
            }
            else
            {
                StartTransition(65f, 1 - 0.85f, 1.2f);
            }
        }

        public virtual void CalculateSpeed(float deltaTime)
        {
            if (Mathf.Abs(controller.horizontalSpeed - controller.targetSpeed) > m_SpeedOffset)
            {
                controller.horizontalSpeed = Mathf.Lerp
                (
                    controller.horizontalSpeed, controller.targetSpeed, deltaTime * m_MovementProfile.SpeedChangeRate
                );
                controller.horizontalSpeed = Mathf.Round(controller.horizontalSpeed * 1000f) / 1000f;
            }
            else
            {
                controller.horizontalSpeed = controller.targetSpeed;
            }
        }

        public virtual void CalcGroundSpeed(float deltaTime)
        {
            if (Mathf.Abs(controller.horizontalSpeed - controller.targetSpeed) > m_SpeedOffset)
            {
                controller.horizontalSpeed = Mathf.Lerp
                (
                    controller.horizontalSpeed, controller.targetSpeed, deltaTime * m_MovementProfile.SpeedChangeRate
                );
                controller.horizontalSpeed = Mathf.Round(controller.horizontalSpeed * 1000f) / 1000f;
            }
            else
            {
                controller.horizontalSpeed = controller.targetSpeed;
            }
        }
        public virtual void CalcAirborneSpeed(float deltaTime)
        {
            controller.horizontalSpeed = Mathf.Lerp(controller.horizontalSpeed, 0, deltaTime * 0.5f);
            controller.horizontalSpeed = Mathf.Round(controller.horizontalSpeed * 1000f) / 1000f;
        }

        /// <summary>
        /// 하는일
        /// 1. 목표 회전값 (입력 방향 기준)
        /// 2. 회전량 보간
        /// 3. 캐릭터 회전 적용
        /// 4. 이동방향 계산 (이동 방향 기준)
        /// </summary>
        public virtual void FreeLookRotate(float deltaTime)
        {
            if (!controller.HasMovementInput()) return;

            // 입력의 수평 수직 값을 기반으로 각을 구한 뒤, 카메라 회전(yaw)값 누적
            m_TargetRotation = Mathf.Atan2(controller.inputMove.x, controller.inputMove.z) * Mathf.Rad2Deg + yawAngle;
            // 현재 transform의 yaw 값을 목표 값으로 보간
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_InterpCache, m_MovementProfile.RotationSmoothTime);
            // 캐릭터 회전시키기
            transform.rotation = Quaternion.Euler(0, rotation, 0);
            // 이동방향 전달
            controller.moveDirection = (Quaternion.Euler(0.0f, m_TargetRotation, 0.0f) * Vector3.forward).normalized;
        }
        /// <summary>
        /// 하는일
        /// 1. 목표 회전값 (카메라 기준)
        /// 2. 회전량 보간 `중복 코드`
        /// 3. 캐릭터 회전 적용 `중복 코드`
        /// 4. 이동방향 계산 (카메라 기준)
        /// </summary>
        /// <param name="deltaTime"></param>
        public virtual void StrafeMoveRotate(float deltaTime)
        {
            // 카메라 회전값으로 고정!!
            m_TargetRotation = yawAngle;
            // 현재 transform의 yaw 값을 목표 값으로 보간
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_InterpCache, m_MovementProfile.RotationSmoothTime);
            // 회전값 적용
            transform.rotation = Quaternion.Euler(0, rotation, 0);

            // 인풋 벡터를 카메라 yaw 기준으로 변환
            Vector3 cameraDirection = Quaternion.Euler(0f, yawAngle, 0f) * controller.inputMove;
            cameraDirection.Normalize();
            if (controller.HasMovementInput())
            {
                controller.moveDirection = cameraDirection;
            }
        }

        /// <summary>
        /// 입력기준 캐릭터 회전 방향 계산
        /// </summary>
        /// <returns> 계산된 목표 yaw 회전값 </returns>
        public virtual float CalcTargetRotationByInput()
        {
            m_TargetRotation = Mathf.Atan2(controller.inputMove.x, controller.inputMove.z) * Mathf.Rad2Deg + yawAngle;
            return m_TargetRotation;
        }
        /// <summary>
        /// 카메라기준 캐릭터 회전 방향 계산
        /// </summary>
        /// <returns> 카메라 기준 yaw </returns>
        public virtual float CalcTargetRotationByCamera()
        {
            return yawAngle;
        }
        /// <summary>
        /// 현재 각도에서 목표 각도로 보간
        /// </summary>
        /// <param name="currentYaw">현재 회전 값</param>
        /// <param name="targetRotation">목표 Yaw</param>
        /// <param name="rotateDelay">목표값 도달에 걸리는 시간</param>
        /// <returns>보간된 Yaw 각도</returns>
        public virtual float SmoothRotateUpdate(float currentYaw, float targetRotation, float rotateDelay)
        {
            return Mathf.SmoothDampAngle(currentYaw, targetRotation, ref m_InterpCache, rotateDelay);
        }
        /// <summary>
        /// 캐릭터에 회전값 적용
        /// </summary>
        /// <param name="rotation">캐릭터가 바라볼 방향</param>
        public virtual void ApplyCharacterRotation(float rotation)
        {
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
        /// <summary>
        /// 입력에 따라 캐릭터가 이동해야 할 방향 벡터를 계산.
        /// 공중에서는 회전을 완화함.
        /// </summary>
        /// <param name="rotation">입력 기반으로 계산된 캐릭터의 Yaw 회전값.</param>
        /// <returns>transform에 적용할 이동 방향.</returns>
        public virtual Vector3 CalcMoveDirByInput(float rotation, float deltaTime)
        {
            float changeSpeed = 1f;
            float baseRotation = m_IsGrounded ? rotation : m_TargetRotation;
            float lerpRotation = baseRotation;

            if (!m_IsGrounded && controller.moveDirection.sqrMagnitude > 0.001f)
            {
                float currentYaw = Mathf.Atan2(controller.moveDirection.x, controller.moveDirection.z) * Mathf.Rad2Deg;
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
            Vector3 cameraDirection = Quaternion.Euler(0f, yawAngle, 0f) * controller.inputMove;
            cameraDirection.Normalize();

            float targetYaw = Mathf.Atan2(cameraDirection.x, cameraDirection.z) * Mathf.Rad2Deg;
            float lerpRotation = targetYaw;

            if (!m_IsGrounded)
            {
                float currentYaw = Mathf.Atan2(controller.moveDirection.x, controller.moveDirection.z) * Mathf.Rad2Deg;
                lerpRotation = Mathf.LerpAngle(currentYaw, targetYaw, deltaTime * 0.5f);
            }

            return Quaternion.Euler(0f, lerpRotation, 0f) * Vector3.forward;
        }

        public void FreeLookCameraSetting()
        {
            m_CinemachineCamera.Lens.FieldOfView = 50;
            m_ThirdPersonFollow.CameraSide = 0.8f;
            m_ThirdPersonFollow.CameraDistance = 3;
        }
        public void StrafeMoveCameraSetting()
        {
            m_CinemachineCamera.Lens.FieldOfView = 60;
            m_ThirdPersonFollow.CameraSide = 1f;
            m_ThirdPersonFollow.CameraDistance = 2f;
        }


        public void StartTransition(float fov, float side, float distance, float? duration = null)
        {
            m_CurrentFOV = fov;
            m_CurrentSide = side;
            m_CurrentDistance = distance;

            if (m_TransitionCoroutine != null)
                StopCoroutine(m_TransitionCoroutine);

            m_TransitionCoroutine = StartCoroutine(TransitionRoutine(duration ?? m_TransitionDuration));
        }


        IEnumerator TransitionRoutine(float duration)
        {
            float elapsed = 0f;
            float invDuration = 1f / duration;

            // 시작값 저장
            float startFOV = m_CinemachineCamera.Lens.FieldOfView;
            float startSide = m_ThirdPersonFollow.CameraSide;
            float startDistance = m_ThirdPersonFollow.CameraDistance;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed * invDuration);
                float curveT = m_TransitionCurve.Evaluate(t);

                m_CinemachineCamera.Lens.FieldOfView = Mathf.LerpUnclamped(startFOV, m_CurrentFOV, curveT);
                m_ThirdPersonFollow.CameraSide = Mathf.LerpUnclamped(startSide, m_CurrentSide, curveT);
                m_ThirdPersonFollow.CameraDistance = Mathf.LerpUnclamped(startDistance, m_CurrentDistance, curveT);

                yield return null;
            }

            // 정확히 목표값으로 마무리 (부동소수점 오차 방지)
            m_CinemachineCamera.Lens.FieldOfView = m_CurrentFOV;
            m_ThirdPersonFollow.CameraSide = m_CurrentSide;
            m_ThirdPersonFollow.CameraDistance = m_CurrentDistance;

            m_TransitionCoroutine = null;
        }

        // 이상태로는 외부 호츨이 딱히 필요없이 알아서 작동하긴할듯
        public void Jump()
        {
            if (!m_IsGrounded) return;

            if (controller.jumpInput && CanJump())
            {
                m_JumpTimer.Start();
                controller.verticalSpeed = Mathf.Sqrt(m_MovementProfile.JumpHeight * -2f * m_MovementProfile.AirborneGravity);
            }
        }

        bool CanJump()
        {
            return m_JumpTimer.IsFinished;
        }

        public void NudgeCameraToCharacter(float amount)
        {
            float target = controller.transform.eulerAngles.y;  // 캐릭터가 현재 보고 있는 방향
            m_CameraYaw = Mathf.LerpAngle(m_CameraYaw, target, amount * Time.deltaTime);
        }

        void ProcessInputLook()
        {
            Vector2 look = controller.inputLook;

            // 마우스 감도값
            float sensitive = 1 + m_CameraLookProfile.LookSensitive;

            if (look.sqrMagnitude >= m_Threshold)
            {
                m_CameraYaw += look.x * sensitive;
                m_CameraPitch += look.y * sensitive;
            }
        }
        #endregion

        #region 내부에서 진행되는 로직
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

        // 이건 FSM에서 호출할 가능성이 있다
        public void LookRotation()
        {
            ProcessInputLook();

            // 회전이 적용되기 전에 클램프
            m_CameraYaw = ClampAngle(m_CameraYaw, float.MinValue, float.MaxValue);
            m_CameraPitch = ClampAngle(m_CameraPitch, m_CameraLookProfile.BottomClamp, m_CameraLookProfile.TopClamp);

            // 최종적으로 회전
            m_CameraTarget.rotation = Quaternion.Euler(m_CameraPitch + m_CameraLookProfile.CameraAngleOverride, m_CameraYaw, 0.0f);
        }

        #endregion

        static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}
