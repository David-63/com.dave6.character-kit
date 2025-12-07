
using System;
using System.Collections;
using UnityUtils.Timer;
using Unity.Cinemachine;
using UnityEngine;
using Dave6.CharacterKit.Sensor;
using Dave6.CharacterKit.Look;

namespace Dave6.CharacterKit.Movement
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
    /// 
    /// 
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
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMover : MonoBehaviour
    {
        public bool showInitialDebug = false;
        CharacterController controller;
        PlayerController playerController; // 이건 레지스터 방식으로 해도 되고

        #region collider & sensor field
        [Header("Collider Settings")]
        [Range(0f, 1f)][SerializeField] float stepHeightRatio = 0.14f;
        //[SerializeField] float colliderStepOffset = 0.25f;
        [SerializeField] float colliderHeight = 1.8f;
        [SerializeField] float colliderRadius = 0.28f;
        [SerializeField] Vector3 colliderOffset = new Vector3(0, 0.5f, 0);

        RaycastSensor groundChecker;

        // public float GroundedOffset = -0.14f; // 이거를 어디..에 써야할지 모르곘네, 없어도 될것같음

        bool isUsingExtendedSensorRange = true; // Use extended range for smoother ground transitions // 이것도 rigidbody에 쓰던거라 필요없을듯?
        public bool isGrounded;
        float baseSensorRange;
        int currentLayer;
        #endregion





        #region move & look field
        [Header("Movement Something")]
        [SerializeField] MovementProfile movementProfile;
        public MovementProfile GetMovementProfile()
        {
            if (movementProfile == null)
            {
                Debug.Log("movementProfile 세팅 안했음");
                return null;
            }
            return movementProfile;
        }
        float targetRotation = 0f;                          // FreeMove 상태에서 회전값 기록하는 용도
        float rotationSpeed;
        const float speedOffset = 0.1f;
        const float terminalVelocity = 53.0f;               // 가속 제한인듯
        const float gravity = -15f;

        [Header("Camera Setting")]
        [SerializeField] GameObject playerCameraPrefab;
        GameObject cinemachineObj;
        CinemachineCamera cinemachineCamera;
        CinemachineThirdPersonFollow thirdPersonFollow;     // 아직 쓸댄 없지만 세부 세팅에 필요해보여서 추가함
        [SerializeField] CameraLookProfile cameraLookProfile;

        [SerializeField] float transitionDuration = 0.35f;
        [SerializeField] AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float currentFOV;
        float currentSide;
        float currentDistance;
        Coroutine transitionCoroutine;



        Transform cameraTarget;                             // 이걸 mover 전담할지, 아니면 다른데서 쓸지에 따라 해당 변수의 위치가 바뀔 수 있음
        Transform MainCamera;                               // 원래 외부에 데이터 전달하려고 만든 기능인데 아직 쓸대가 없으니 그냥 private으로 설정
        public float YawAngle => MainCamera.eulerAngles.y;  // 위와 같음

        float cameraYaw = 0f;
        float cameraPitch = 0f;
        const float threshold = 0.01f;                      // 입력 최소치 제한
        
        #endregion

        #region jump field

        [Header("Jump settings")]
        [SerializeField] float jumpDuration = 0.2f;

        Countdown jumpTimer;


        #endregion



        #region Initialize & Sensor setting
        void Awake()
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
            if (controller == null)
            {
                Setup();
            }

            float stepOffset = colliderHeight * stepHeightRatio;

            controller.stepOffset = stepOffset;
            controller.skinWidth = colliderRadius / 10f;
            controller.center = colliderOffset * colliderHeight;
            controller.radius = colliderRadius;
            controller.height = colliderHeight;

            RecalibrateSensor();
        }
        void RecalibrateSensor()
        {
            groundChecker ??= new RaycastSensor(transform);

            groundChecker.SetCastOrigin(controller.bounds.center);
            groundChecker.SetCastDirection(RaycastSensor.CastDirection.Down);
            RecalculateSensorLayerMask();

            const float safetyDistanceFactor = 0.01f; // Small factor added to prevent clipping issues when the sensor range is calcuatetd
            float length = colliderHeight * (1f - stepHeightRatio) * 0.5f + colliderHeight * stepHeightRatio;
            baseSensorRange = length * (1f + safetyDistanceFactor) * transform.localScale.x;
            groundChecker.castLength = length * transform.localScale.x;
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
            groundChecker.layermask = layerMask;
            currentLayer = objectLayer;
        }
        public void CheckForGround()
        {
            if (currentLayer != gameObject.layer)
            {
                RecalculateSensorLayerMask();
            }

            groundChecker.castLength = baseSensorRange;
            groundChecker.Cast();

            isGrounded = groundChecker.HasDetecteHit();
        }

        public bool IsGrounded() => isGrounded;
        public Vector3 GetGroundNormal() => groundChecker.GetNormal();
        public void SetExtendedSensorRange(bool isExtended) => isUsingExtendedSensorRange = isExtended;
        #endregion

        void Setup()
        {
            if (showInitialDebug)
            {
                Debug.Log("내장된 컴포넌트 초기화");
            }
            controller = GetComponent<CharacterController>();
            playerController = GetComponent<PlayerController>();

            MainCamera = Camera.main.transform;

            var brain = Camera.main.GetComponent<CinemachineBrain>();
            if (brain == null)
            {
                Camera.main.gameObject.AddComponent<CinemachineBrain>();
            }
        }
        void OnceSetting()
        {
            if (showInitialDebug)
            {
                Debug.Log("카메라 객체 초기화");
            }
            if (cameraTarget == null)
            {
                cameraTarget = transform.Find("CameraTarget");
                cameraTarget.position = new Vector3(0, 1.4f, 0);
            }
            if (cameraLookProfile == null)
            {
                Debug.Log("카메라 프로파일 세팅 안돼있음");
            }

            if (playerCameraPrefab == null)
            {
                Debug.Log("카메라 프리팹 세팅 안돼있음");
            }
            cinemachineObj = Instantiate(playerCameraPrefab, transform);
            cinemachineCamera = cinemachineObj.GetComponent<CinemachineCamera>();
            thirdPersonFollow = cinemachineObj.GetComponent<CinemachineThirdPersonFollow>();
            if (cameraTarget == null)
            {
                Debug.Log("카메라 타겟을 못찾음 (이름 불일치?)");
            }

            cinemachineCamera.Follow = cameraTarget;

            thirdPersonFollow.Damping = Vector3.zero;
            thirdPersonFollow.ShoulderOffset = new Vector3(1,0,0);
            thirdPersonFollow.VerticalArmLength = 0;
            thirdPersonFollow.AvoidObstacles.Enabled = true;
            thirdPersonFollow.AvoidObstacles.DampingFromCollision = 0.2f;
            thirdPersonFollow.AvoidObstacles.DampingIntoCollision = 0.2f;

            jumpTimer = new Countdown(jumpDuration);
        }

        void Update()
        {
            Jump();
            ApplyGravity();
            CheckForGround();
            ApplyMovement();
        }

        void LateUpdate()
        {
            LookRotation();
        }

        #region 외부 호출 함수들
        public void CalculateSpeed(float deltaTime)
        {
            if (Mathf.Abs(playerController.HorizontalSpeed - playerController.TargetSpeed) > speedOffset)
            {
                playerController.HorizontalSpeed
                 = Mathf.Lerp(playerController.HorizontalSpeed, playerController.TargetSpeed, deltaTime * movementProfile.SpeedChangeRate);
                playerController.HorizontalSpeed = Mathf.Round(playerController.HorizontalSpeed * 1000f) / 1000f;
            }
            else
            {
                playerController.HorizontalSpeed = playerController.TargetSpeed;
            }
        }

        public void FreeLookDirection()
        {
            if (!playerController.HasMovementInput()) return;

            // 입력의 수평 수직 값을 기반으로 각을 구한 뒤, 카메라 회전(yaw)값 누적
            targetRotation = Mathf.Atan2(playerController.InputMove.x, playerController.InputMove.z) * Mathf.Rad2Deg + YawAngle;
            // 현재 transform의 yaw 값을 목표 값으로 보간
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationSpeed, movementProfile.RotationSmoothTime);
            // 캐릭터 회전시키기
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            // 이동방향 전달
            playerController.MoveDirection = (Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward).normalized;
        }

        public void FreeLookCameraSetting()
        {
            cinemachineCamera.Lens.FieldOfView = 50;
            thirdPersonFollow.CameraSide = 0.8f;
            thirdPersonFollow.CameraDistance = 3;
        }
        public void StrafeMoveCameraSetting()
        {
            cinemachineCamera.Lens.FieldOfView = 60;
            thirdPersonFollow.CameraSide = 1f;
            thirdPersonFollow.CameraDistance = 2f;
        }

        public void SetFreeLookMode() => StartTransition(55f, 0.75f, 2f);
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

        public void StartTransition(float fov, float side, float distance, float? duration = null)
        {
            currentFOV = fov;
            currentSide = side;
            currentDistance = distance;

            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine);

            transitionCoroutine = StartCoroutine(TransitionRoutine(duration ?? transitionDuration));
        }


        IEnumerator TransitionRoutine(float duration)
        {
            float elapsed = 0f;
            float invDuration = 1f / duration;

            Debug.Log($"{cinemachineCamera} 에 트렌지션 적용중");

            // 시작값 저장
            float startFOV = cinemachineCamera.Lens.FieldOfView;
            float startSide = thirdPersonFollow.CameraSide;
            float startDistance = thirdPersonFollow.CameraDistance;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed * invDuration);
                float curveT = transitionCurve.Evaluate(t);

                cinemachineCamera.Lens.FieldOfView = Mathf.LerpUnclamped(startFOV, currentFOV, curveT);
                thirdPersonFollow.CameraSide = Mathf.LerpUnclamped(startSide, currentSide, curveT);
                thirdPersonFollow.CameraDistance = Mathf.LerpUnclamped(startDistance, currentDistance, curveT);

                yield return null;
            }

            // 정확히 목표값으로 마무리 (부동소수점 오차 방지)
            cinemachineCamera.Lens.FieldOfView = currentFOV;
            thirdPersonFollow.CameraSide = currentSide;
            thirdPersonFollow.CameraDistance = currentDistance;

            transitionCoroutine = null;
        }

        public void StrafeMoveDirection()
        {
            // 인풋 벡터를 카메라 yaw 기준으로 변환
            Vector3 cameraDirection = Quaternion.Euler(0f, YawAngle, 0f) * playerController.InputMove;
            cameraDirection.Normalize();
            if (playerController.HasMovementInput())
            {
                playerController.MoveDirection = cameraDirection;
            }
            // 카메라 회전값으로 고정!!
            targetRotation = YawAngle;
            // 현재 transform의 yaw 값을 목표 값으로 보간
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationSpeed, movementProfile.RotationSmoothTime);
            // 회전값 적용
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // 이상태로는 외부 호츨이 딱히 필요없이 알아서 작동하긴할듯
        public void Jump()
        {
            if (!isGrounded) return;

            if (playerController.JumpInput && CanJump())
            {
                jumpTimer.Start();
                playerController.VerticalSpeed = Mathf.Sqrt(movementProfile.JumpHeight * -2f * movementProfile.AirborneGravity);
            }
        }

        bool CanJump()
        {
            return jumpTimer.IsFinished;
        }
        #endregion

        #region 내부에서 진행되는 로직
        void ApplyGravity()
        {
            if (isGrounded)
            {
                // stop our velocity dropping infinitely when grounded
                if (playerController.VerticalSpeed < 0.0f)
                {
                    playerController.VerticalSpeed = movementProfile.GroundGravity;
                }
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (playerController.VerticalSpeed < movementProfile.TerminalVelocity)
            {
                playerController.VerticalSpeed += movementProfile.AirborneGravity * Time.deltaTime;
            }
        }

        void ApplyMovement()
        {
            Vector3 velocity = playerController.MoveDirection * playerController.HorizontalSpeed + Vector3.up * playerController.VerticalSpeed;
            controller.Move(velocity * Time.deltaTime);
        }

        // 이건 FSM에서 호출할 가능성이 있다
        public void LookRotation()
        {
            // Look 벡터가 임계값 이상일 때만 카메라 회전 적용
            Vector2 look = playerController.InputLook;

            float sensitive = 1 + cameraLookProfile.LookSensitive;
            if (look.sqrMagnitude >= threshold)
            {
                cameraYaw += look.x *  sensitive;
                cameraPitch += look.y * sensitive;
            }

            cameraYaw = ClampAngle(cameraYaw, float.MinValue, float.MaxValue);
            cameraPitch = ClampAngle(cameraPitch, cameraLookProfile.BottomClamp, cameraLookProfile.TopClamp);

            cameraTarget.rotation = Quaternion.Euler(cameraPitch + cameraLookProfile.CameraAngleOverride, cameraYaw, 0.0f);
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

/*
    메모장.

    Mathf.Atan2 는 y / x 좌표를 기준으로 라디안 각도를 구하는 함수
*/