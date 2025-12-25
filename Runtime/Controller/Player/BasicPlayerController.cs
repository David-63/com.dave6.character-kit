using Dave6.CharacterKit.AnimHandler;
using Dave6.CharacterKit.Input;
using Dave6.GameStateFlow;
using Dave6.StateMachine;
using UnityEngine;
using UnityUtils;

namespace Dave6.CharacterKit
{
    public abstract class BasicPlayerController : MonoBehaviour, IEntity
    {
        #region control field
        public bool showInitialDebug = false;
        [SerializeField] protected InputReader m_Input;
        public InputReader GetInputReader() => m_Input;
        protected BasicMover m_Mover;
        public BasicMover mover => m_Mover;
        protected CameraHandler m_CameraHandler;
        public CameraHandler cameraHandler => m_CameraHandler;
        protected AnimatorHandler m_AnimatorHandler;
        public AnimatorHandler animatorHandler => m_AnimatorHandler;

        protected AnimatorEventProxy m_AnimEventProxy;
        public bool movementLocked; // 이건 숨길거임
        public bool attacking; // 이건 숨길거임
        #endregion


        #region input messenger
        public Vector3 inputMove => new Vector3(m_Input.InputMove.x, 0, m_Input.InputMove.y);
        public Vector2 inputLook => m_Input.InputLook;
        
        

        protected bool m_JumpInput = false;
        public bool jumpInput => m_JumpInput;
        protected bool m_AimInput = false;
        public bool aimInput => m_AimInput;
        protected bool m_ShiftInput = false;
        public bool shiftInput => m_ShiftInput;
        protected bool m_AttackInput = false;
        public bool attackInput => m_AttackInput;
        protected bool m_AttackInputTap = false;
        public bool attackInputTap => m_AttackInputTap;
        protected bool m_InteractInputTap = false;
        public bool interactInputTap => m_InteractInputTap;
        #endregion

        #region movement value field
        protected float m_BaseSpeed;
        public float baseSpeed { get => m_BaseSpeed; set => m_BaseSpeed = value; }
        protected float m_ImpulseSpeed;
        public float impulseSpeed { get => m_ImpulseSpeed; set => m_ImpulseSpeed = value; }
        protected float m_TargetSpeed;
        public float targetSpeed { get => m_TargetSpeed; set => m_TargetSpeed = value; }
        protected float m_HorizontalSpeed;
        public float horizontalSpeed { get => m_HorizontalSpeed; set => m_HorizontalSpeed = value; }
        protected float m_VerticalSpeed;
        public float verticalSpeed { get => m_VerticalSpeed; set => m_VerticalSpeed = value; }
        protected Vector3 m_MoveDirection;
        public Vector3 moveDirection { get => m_MoveDirection; set => m_MoveDirection = value; }
        public bool HasMovementInput() => inputMove.x != 0 || inputMove.z != 0;
        #endregion
        protected MinimumStateMachine m_LocomotionStateMachine;

        #region animator handle field
        Vector3 m_CachedInputDir;
        public Vector3 cachedInputDir
        {
            get => m_CachedInputDir;
            set => m_CachedInputDir = value;
        }
        #endregion

        // 이거 아직 사용처를 못정했음 UI에 쓰려고 한건데
        [SerializeField] LayerMask ignorePlayerLayerMask;

        protected IInteractable m_CurrentInteractable;
        public IInteractable currentInteractable => m_CurrentInteractable;


        public virtual void Awake()
        {
            // 애니메이션 세팅
            if (this.TryGetComponentInChildren<Animator>(out var animator))
            {
                m_AnimatorHandler = new AnimatorHandler(this, animator);
            }
            if (this.TryGetComponentInChildren<AnimatorEventProxy>(out var proxy))
            {
                m_AnimEventProxy = proxy;
            }
            if (showInitialDebug && m_AnimatorHandler != null && m_AnimEventProxy != null)
            {
                Debug.Log("애니메이터 초기화 완료");
            }
            else if (showInitialDebug)
            {
                Debug.Log("애니메이터 초기화 실패");
            }
            // 카메라 세팅
            m_CameraHandler = GetComponent<CameraHandler>();
            m_CameraHandler.RegisterCamera(this);

            if (showInitialDebug && m_CameraHandler != null)
            {
                Debug.Log("카메라 초기화 완료");
            }
            else if (showInitialDebug)
            {
                Debug.Log("카메라 초기화 실패");
            }

            // 무버 세팅
            m_Mover = GetComponent<BasicMover>();
            m_Mover.RegisterMover(this, m_CameraHandler);

            if (showInitialDebug && m_CameraHandler != null)
            {
                Debug.Log("무버 초기화 완료");
            }
            else if (showInitialDebug)
            {
                Debug.Log("무버 초기화 실패");
            }


            // 레이어 세팅
            gameObject.layer = 3;
            
            if (m_Input == null)
            {
                Debug.Log("인풋 추가 안했음");
            }
            InputEventBind();
        }

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public abstract void Start();

        // Update is called once per frame
        public virtual void Update()
        {
            m_Mover.OnUpdate();
            m_LocomotionStateMachine.Update();
        }
        public virtual void FixedUpdate()
        {
            m_LocomotionStateMachine.FixedUpdate();
        }
        public virtual void LateUpdate()
        {
            m_LocomotionStateMachine.LateUpdate();
            m_CameraHandler.OnLateUpdate();
            ClearTapInput();
        }

        #region GameFlow

        protected bool m_Paused;
        public bool paused => m_Paused;

        void OnEnable()
        {
            GameFlowController.instance.onStateChanged += HandleGameStateChanged;
        }

        void OnDisable()
        {
            GameFlowController.instance.onStateChanged -= HandleGameStateChanged;
        }

        public void ResetController()
        {
            Debug.Log("플레이어 초기화");
            m_CurrentInteractable = null;
            m_Mover.ResetMover();
            m_AnimatorHandler.ResetAnimHandler();
        }

        void HandleGameStateChanged(eGameState prev, eGameState next)
        {
            switch (next)
            {
                // 로딩하면 입력도 막고, 세팅 초기화
                case eGameState.Loading:
                ResetController();
                m_Paused = true;
                break;
                // 멈춘거면 입력만 막기
                case eGameState.Paused:
                m_Paused = true;
                break;

                case eGameState.Running:
                m_Paused = false;                    
                if (prev != eGameState.Running)
                {
                    ResetController();
                }
                break;
            }
        }
        #endregion
        #region Input Event Func
        protected virtual void ClearTapInput()
        {
            m_AttackInputTap = false;
            m_InteractInputTap = false;
        }

        protected virtual void InputEventBind()
        {
            if (showInitialDebug)
            {
                Debug.Log("인풋 초기화");
            }
            m_Input.Jump += (value) => m_JumpInput = value;
            m_Input.Aim += (value) => m_AimInput = value;
            m_Input.Shift += (value) => m_ShiftInput = value;
            m_Input.Attack += (value) => m_AttackInput = value;
            m_Input.AttackTap += () => m_AttackInputTap = true;
            m_Input.InteractTap += () => m_InteractInputTap = true;
        }
        #endregion
        protected abstract void SetupStateMachine();

        #region Aim Func
        public Vector3 CalculateAimPoint(Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, m_CameraHandler.cameraLookProfile.MaxLookRange, ignorePlayerLayerMask))
            {
                return hit.point;
            }
            return origin + direction * m_CameraHandler.cameraLookProfile.MaxLookRange;
        }
        #endregion
    }
}
