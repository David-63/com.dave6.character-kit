using System;
using System.Collections;
using Dave6.CharacterKit.Look;
using Unity.Cinemachine;
using UnityEngine;
using UnityUtils;

namespace Dave6.CharacterKit
{
    /// <summary>
    /// 의존하는게 하나도 없음
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public bool showInitialDebug;
        BasicPlayerController m_Controller;
        #region 시네머신 카메라 필드
        [Header("Camera Setting")]
        [SerializeField] GameObject m_PlayerCameraPrefab;
        GameObject m_CinemachineObj;
        CinemachineCamera m_CinemachineCamera;
        CinemachineThirdPersonFollow m_ThirdPersonFollow;     // 아직 쓸댄 없지만 세부 세팅에 필요해보여서 추가함
        [SerializeField] CameraLookProfile m_CameraLookProfile;
        public CameraLookProfile cameraLookProfile => m_CameraLookProfile;

        [SerializeField] float m_TransitionDuration = 0.35f;
        [SerializeField] AnimationCurve m_TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float m_CurrentFOV;
        float m_CurrentSide;
        float m_CurrentDistance;
        Coroutine m_TransitionCoroutine;
        #endregion
        

        #region 카메라 제어 필드
        Transform m_CameraTarget;
        // 원래 외부에 데이터 전달하려고 만든 기능인데 아직 쓸대가 없으니 그냥 private으로 설정
        public Transform mainCamera { get; private set; }
        public float yawAngle => mainCamera.eulerAngles.y;  // 위와 같음

        public float cameraYaw { get; private set; }
        public float cameraPitch { get; private set; }
        const float m_Threshold = 0.001f;                      // 입력 최소치 제한
        #endregion

        #region 에임 제어 필드
        public Quaternion aimAnchor { get; private set; }
        public Quaternion baseAim { get; private set; }
        public Vector3 baseAimPoint { get; private set; }

        #endregion

        void Awake()
        {
            Setup();
        }

        public void OnLateUpdate()
        {
            LookRotation();
            baseAimPoint = m_Controller.CalculateAimPoint(mainCamera.position, baseAim * Vector3.forward, m_Controller.cameraAimLayerMask);
        }
        #region 초기화
        public void RegisterCamera(BasicPlayerController controller)
        {
            m_Controller = controller;
        }
        protected virtual void Setup()
        {
            if (Camera.main == null)
            {
                Debug.Log("Main Camera 태그 달아야함");
            }

            mainCamera = Camera.main.transform;

            // 없으면 추가해줌
            Camera.main.gameObject.GetOrAddComponent<CinemachineBrain>();

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
        }
        #endregion

        #region GameFlow
        public void ResetCameraHandler()
        {
            
        }
        #endregion

        #region 카메라 업데이트

        /// <summary>
        /// 기본적인 카메라 회전, AimAnchor, m_BaseAim 계산
        /// </summary>
        public void LookRotation()
        {
            ProcessInputLook();

            // 회전이 적용되기 전에 클램프
            cameraYaw = ClampAngle(cameraYaw, float.MinValue, float.MaxValue);
            cameraPitch = ClampAngle(cameraPitch, m_CameraLookProfile.BottomClamp, m_CameraLookProfile.TopClamp);

            // 최종적으로 회전
            aimAnchor = Quaternion.Euler(cameraPitch + m_CameraLookProfile.CameraAngleOverride, cameraYaw, 0.0f);
            Quaternion offset = GetRecoilOffset(); // 예: shake, recoil, hit reaction 등
            baseAim = aimAnchor * offset;
            m_CameraTarget.rotation = baseAim;


            // =========================================================================================
        }

        void ProcessInputLook()
        {
            Vector2 look = m_Controller.inputLook;

            // 마우스 감도값
            float sensitive = 1 + m_CameraLookProfile.LookSensitive;

            if (look.sqrMagnitude >= m_Threshold)
            {
                cameraYaw += look.x * sensitive;
                cameraPitch += look.y * sensitive;
            }
        }

        #endregion

        #region 시네머신 트랜지션
        public void SetFreeLookMode()
        {
            float fov = 50f;
            float sideLength = 0.65f;
            float distnace = 4.0f;
            StartTransition(fov, sideLength, distnace);
        }
        public void SetStrafeMode(bool shift)
        {
            float fov = 50f;
            float sideLength = 0.85f;
            float distnace = 1.0f;
            if (shift)
            {
                StartTransition(fov, sideLength, distnace);
            }
            else
            {
                StartTransition(fov, 1 - sideLength, distnace);
            }
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
        #endregion

        #region 카메라 흔들림
        public Quaternion GetRecoilOffset()
        {
            float deltaTime = Time.deltaTime;

            // === target 감쇄 속도 동적화 (빠른 복귀 강조) ===
            float targetDecayPitchSpeed = minTargetDecaySpeed + Mathf.Abs(targetRecoilPitch) * targetDecayMultiplier;
            targetRecoilPitch = Mathf.MoveTowards(targetRecoilPitch, 0f, targetDecayPitchSpeed * deltaTime);

            float targetDecayYawSpeed = minTargetDecaySpeed + Mathf.Abs(targetRecoilYaw) * targetDecayMultiplier;
            targetRecoilYaw = Mathf.MoveTowards(targetRecoilYaw, 0f, targetDecayYawSpeed * deltaTime);

            // 동적 복귀 속도 계산
            float applySpeedPitch = (targetRecoilPitch > currentRecoilPitch) ? recoilApplySpeed : recoveryApplySpeed;
            currentRecoilPitch = Mathf.MoveTowards(currentRecoilPitch, targetRecoilPitch, applySpeedPitch * deltaTime);

            float applySpeedYaw = (targetRecoilYaw > currentRecoilYaw) ? recoilApplySpeed : recoveryApplySpeed;
            currentRecoilYaw = Mathf.MoveTowards(currentRecoilYaw, targetRecoilYaw, applySpeedYaw * deltaTime);

            // 거의 완료 시 초기화 (선택적 최적화)
            if (Mathf.Approximately(targetRecoilPitch, 0f) && Mathf.Approximately(targetRecoilYaw, 0f) &&
                Mathf.Approximately(currentRecoilPitch, 0f) && Mathf.Approximately(currentRecoilYaw, 0f))
            {
                currentRecoilPitch = currentRecoilYaw = targetRecoilPitch = targetRecoilYaw = 0f;
            }

            return Quaternion.Euler(currentRecoilPitch, currentRecoilYaw, 0f);
        }


        float currentRecoilPitch = 0f;   // 현재 적용 pitch offset
        float targetRecoilPitch = 0f;    // 누적 목표 pitch (음수로 위로 올라감)
        float currentRecoilYaw = 0f;     // 현재 yaw offset
        float targetRecoilYaw = 0f;      // 누적 목표 yaw


        // 아래 싹다 Config SO로 구현해도 됨

        // 반동 복귀 속도
        float minTargetDecaySpeed = 15f;    // 최소 감쇄 속도
        float targetDecayMultiplier = 20f;  // 크기 비례 증가 속도

        // 반동 반영속도
        float recoilApplySpeed = 15f;
        float recoveryApplySpeed = 10;
        float maxRecoilPitch = -15f;     // 최대 누적 한계 (음수)

        public void PlayRecoil(float amplitude, float sideVariation = 0.6f)
        {
            // 각 발사마다 target 누적 (캡 적용)
            targetRecoilPitch = Mathf.Max(targetRecoilPitch - amplitude, maxRecoilPitch);

            // yaw도 약간 누적 + 랜덤
            targetRecoilYaw += UnityEngine.Random.Range(-amplitude * sideVariation, amplitude * sideVariation);
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
