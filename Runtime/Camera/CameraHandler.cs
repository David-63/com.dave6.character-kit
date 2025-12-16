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
    public class CameraHandler : MonoBehaviour
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

        [SerializeField] float m_TransitionDuration = 0.35f;
        [SerializeField] AnimationCurve m_TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float m_CurrentFOV;
        float m_CurrentSide;
        float m_CurrentDistance;
        Coroutine m_TransitionCoroutine;
        #endregion
        

        #region 카메라 제어 필드
        Transform m_CameraTarget;                             // 이걸 mover 전담할지, 아니면 다른데서 쓸지에 따라 해당 변수의 위치가 바뀔 수 있음
        Transform m_MainCamera;                               // 원래 외부에 데이터 전달하려고 만든 기능인데 아직 쓸대가 없으니 그냥 private으로 설정
        public Transform mainCamera => m_MainCamera;
        public float yawAngle => m_MainCamera.eulerAngles.y;  // 위와 같음

        float m_CameraYaw = 0f;
        public float cameraYaw => m_CameraYaw;
        float m_CameraPitch = 0f;
        public float cameraPitch => m_CameraPitch;
        const float m_Threshold = 0.01f;                      // 입력 최소치 제한
        #endregion

        #region 에임 제어 필드
        Quaternion m_AimAnchor;
        public Quaternion aimAnchor => m_AimAnchor;

        float shakeTime;
        float shakeDuration;        // 유지시간
        float shakeAmplitude;       // 강도
        float shakeFrequency;       // 속도
        #endregion

        protected virtual void Awake()
        {
            Setup();
        }

        protected virtual void LateUpdate()
        {
            LookRotation();
        }
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

            m_MainCamera = Camera.main.transform;

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

        public void LookRotation()
        {
            ProcessInputLook();

            // 회전이 적용되기 전에 클램프
            m_CameraYaw = ClampAngle(m_CameraYaw, float.MinValue, float.MaxValue);
            m_CameraPitch = ClampAngle(m_CameraPitch, m_CameraLookProfile.BottomClamp, m_CameraLookProfile.TopClamp);

            // 최종적으로 회전
            m_AimAnchor = Quaternion.Euler(m_CameraPitch + m_CameraLookProfile.CameraAngleOverride, m_CameraYaw, 0.0f);
            Quaternion offset = GetCameraShakeOffset(); // 예: shake, recoil, hit reaction 등
            m_CameraTarget.rotation = m_AimAnchor * offset;
        }

        void ProcessInputLook()
        {
            Vector2 look = m_Controller.inputLook;

            // 마우스 감도값
            float sensitive = 1 + m_CameraLookProfile.LookSensitive;

            if (look.sqrMagnitude >= m_Threshold)
            {
                m_CameraYaw += look.x * sensitive;
                m_CameraPitch += look.y * sensitive;
            }
        }

        //void CalcRay

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
            float distnace = 1.5f;
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
        public void PlayShake(float amplitude, float duration, float frequency)
        {
            shakeAmplitude = amplitude;
            shakeDuration = duration;
            shakeFrequency = frequency;
            shakeTime = 0f;
        }

        Quaternion GetCameraShakeOffset()
        {
            if (shakeTime >= shakeDuration) return Quaternion.identity;

            shakeTime += Time.deltaTime;

            float t = shakeTime / shakeDuration;      // 0 ~ 1
            float damper = 1f - t;                     // 선형 감쇠 (필요하면 곡선)

            float noiseX = Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f;
            float noiseY = Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f;

            float pitch = noiseX * shakeAmplitude * damper;
            float yaw   = noiseY * shakeAmplitude * damper;

            return Quaternion.Euler(pitch, yaw, 0f);
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
