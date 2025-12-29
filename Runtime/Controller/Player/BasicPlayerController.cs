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
        public BasicMover mover { get; protected set; }
        public CameraHandler cameraHandler { get; protected set; }
        public AnimatorHandler animatorHandler { get; protected set; }

        protected AnimatorEventProxy m_AnimEventProxy;
        public bool movementLocked; // 이건 숨길거임
        public bool attacking; // 이건 숨길거임
        #endregion


        #region input messenger
        public Vector3 inputMove => new Vector3(m_Input.InputMove.x, 0, m_Input.InputMove.y);
        public Vector2 inputLook => m_Input.InputLook;
        
        

        public bool jumpInput {get; protected set;}
        public bool aimInput {get; protected set;}
        public bool shiftInput {get; protected set;}
        public bool attackInput {get; protected set;}
        public bool attackInputTap {get; protected set;}
        public bool interactInputTap {get; protected set;}
        #endregion

        #region movement value field
        public float baseSpeed { get; set; }
        public float impulseSpeed { get; set; }
        public float targetSpeed { get; set; }
        public float horizontalSpeed { get; set; }
        public float verticalSpeed { get; set; }
        public Vector3 moveDirection { get; set; }
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
                animatorHandler = new AnimatorHandler(this, animator);
            }
            if (this.TryGetComponentInChildren<AnimatorEventProxy>(out var proxy))
            {
                m_AnimEventProxy = proxy;
            }
            if (showInitialDebug && animatorHandler != null && m_AnimEventProxy != null)
            {
                Debug.Log("애니메이터 초기화 완료");
            }
            else if (showInitialDebug)
            {
                Debug.Log("애니메이터 초기화 실패");
            }
            // 카메라 세팅
            cameraHandler = GetComponent<CameraHandler>();
            cameraHandler.RegisterCamera(this);

            if (showInitialDebug && cameraHandler != null)
            {
                Debug.Log("카메라 초기화 완료");
            }
            else if (showInitialDebug)
            {
                Debug.Log("카메라 초기화 실패");
            }

            // 무버 세팅
            mover = GetComponent<BasicMover>();
            mover.RegisterMover(this, cameraHandler);

            if (showInitialDebug && cameraHandler != null)
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
            mover.OnUpdate();
            m_LocomotionStateMachine.Update();
        }
        public virtual void FixedUpdate()
        {
            m_LocomotionStateMachine.FixedUpdate();
        }
        public virtual void LateUpdate()
        {
            m_LocomotionStateMachine.LateUpdate();
            cameraHandler.OnLateUpdate();
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
            mover.ResetMover();
            animatorHandler.ResetAnimHandler();
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
            attackInputTap = false;
            interactInputTap = false;
        }

        protected virtual void InputEventBind()
        {
            if (showInitialDebug)
            {
                Debug.Log("인풋 초기화");
            }
            m_Input.Jump += (value) => jumpInput = value;
            m_Input.Aim += (value) => aimInput = value;
            m_Input.Shift += (value) => shiftInput = value;
            m_Input.Attack += (value) => attackInput = value;
            m_Input.AttackTap += () => attackInputTap = true;
            m_Input.InteractTap += () => interactInputTap = true;
        }
        #endregion
        protected abstract void SetupStateMachine();

        #region Aim Func
        public Vector3 CalculateAimPoint(Vector3 origin, Vector3 direction)
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, cameraHandler.cameraLookProfile.MaxLookRange, ignorePlayerLayerMask))
            {
                return hit.point;
            }
            return origin + direction * cameraHandler.cameraLookProfile.MaxLookRange;
        }
        #endregion
    }
}
