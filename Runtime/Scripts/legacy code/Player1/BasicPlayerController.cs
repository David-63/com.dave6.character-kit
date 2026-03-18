// using Dave6.CharacterKit.AnimHandler;
// using Dave6.CharacterKit.Inputs;
// using Dave6.Foundation.GameLogic.State;
// using Dave6.GameStateFlow;
// using UnityEngine;
// using UnityUtils;

// namespace Dave6.CharacterKit
// {
//     public abstract class BasicPlayerController : MonoBehaviour, IEntity
//     {
//         #region control field
//         public bool showInitialDebug = false;
//         [SerializeField] protected InputReader m_Input;                         // 인풋
//         public InputReader GetInputReader() => m_Input;
//         public BasicMover mover { get; protected set; }                         // 무브먼트
//         public CameraController cameraHandler { get; protected set; }           // 카메라
//         public AnimatorHandler animatorHandler { get; protected set; }          // 애니메이터
//         protected AnimatorEventProxy m_AnimEventProxy;
//         #endregion


//         #region context field       | input
//         public Vector3 inputMove => new Vector3(m_Input.InputMove.x, 0, m_Input.InputMove.y);
//         public Vector2 inputLook => m_Input.InputLook;


//         public bool jumpInput {get; protected set;}
//         public bool shiftInput {get; protected set;}
//         public bool interactInputTap {get; protected set;}

//         public bool focusInput {get; protected set;}
//         public bool attackInput {get; protected set;}
//         public bool attackInputTap {get; protected set;}
//         public bool reloadInputTap {get; protected set;}
//         #endregion

//         #region context field       | movement value
//         public float baseSpeed { get; set; }
//         public float impulseSpeed { get; set; }
//         public float targetSpeed { get; set; }
//         public float horizontalSpeed { get; set; }
//         public float verticalSpeed { get; set; }
//         public Vector3 moveDirection { get; set; }
//         public bool HasMovementInput() => inputMove.x != 0 || inputMove.z != 0;
//         #endregion
//         protected StateMachine m_LocomotionStateMachine;

//         #region context field       | animator handle
//         Vector3 m_CachedInputDir;
//         public Vector3 cachedInputDir
//         {
//             get => m_CachedInputDir;
//             set => m_CachedInputDir = value;
//         }
//         #endregion

//         // combat이 들고있는게 맞아. 그리고 이건 context 필드에 해당함
//         [SerializeField] LayerMask m_CharacterAimLayerMask;
//         public LayerMask characterAimLayerMask => m_CharacterAimLayerMask;
//         [SerializeField] LayerMask m_CameraAimLayerMask;
//         public LayerMask cameraAimLayerMask => m_CameraAimLayerMask;


//         #region 상호작용 필드
//         protected IInteractable m_CurrentInteractable;
//         public IInteractable currentInteractable => m_CurrentInteractable;
//         #endregion


//         public virtual void Awake()
//         {
//             // 애니메이션 세팅
//             if (this.TryGetComponentInChildren<AnimatorEventProxy>(out var proxy))
//             {
//                 m_AnimEventProxy = proxy;
//             }
//             if (this.TryGetComponentInChildren<Animator>(out var animator))
//             {
//                 animatorHandler = GetComponent<AnimatorHandler>();
//                 animatorHandler.RegisterAnimator(animator, m_AnimEventProxy);
//             }
//             if (showInitialDebug && animatorHandler != null && m_AnimEventProxy != null)
//             {
//                 Debug.Log("애니메이터 초기화 완료");
//             }
//             else if (showInitialDebug)
//             {
//                 Debug.Log("애니메이터 초기화 실패");
//             }
//             // 카메라 세팅
//             cameraHandler = GetComponent<CameraController>();
//             cameraHandler.RegisterCamera(this);

//             if (showInitialDebug && cameraHandler != null)
//             {
//                 Debug.Log("카메라 초기화 완료");
//             }
//             else if (showInitialDebug)
//             {
//                 Debug.Log("카메라 초기화 실패");
//             }

//             // 무버 세팅
//             mover = GetComponent<BasicMover>();
//             mover.RegisterMover(this, cameraHandler);

//             if (showInitialDebug && cameraHandler != null)
//             {
//                 Debug.Log("무버 초기화 완료");
//             }
//             else if (showInitialDebug)
//             {
//                 Debug.Log("무버 초기화 실패");
//             }


//             // 레이어 세팅
//             gameObject.layer = 3;
            
//             if (m_Input == null)
//             {
//                 Debug.Log("인풋 추가 안했음");
//             }
//             InputBind();
//         }

        
//         // Start is called once before the first execution of Update after the MonoBehaviour is created
//         public abstract void Start();

//         // Update is called once per frame
//         public virtual void Update()
//         {
//             mover.OnUpdate();
//             m_LocomotionStateMachine.Update();
//         }
//         public virtual void FixedUpdate()
//         {
//             m_LocomotionStateMachine.FixedUpdate();
//         }
//         public virtual void LateUpdate()
//         {
//             m_LocomotionStateMachine.LateUpdate();
//             cameraHandler.OnLateUpdate();
//             ClearTapInput();
//         }

//         #region GameFlow

//         protected bool m_Paused;
//         public bool paused => m_Paused;

//         void OnEnable()
//         {
//             GameFlowController.Instance.onStateChanged += HandleGameStateChanged;
//         }

//         void OnDisable()
//         {
//             if (GameFlowController.Instance != null)
//             {
//                 GameFlowController.Instance.onStateChanged -= HandleGameStateChanged;
//             }
//         }

//         public void ResetController()
//         {
//             Debug.Log("플레이어 초기화");
//             m_CurrentInteractable = null;
//             mover.ResetMover();
//             animatorHandler.ResetAnimHandler();
//         }

//         void HandleGameStateChanged(eGameState prev, eGameState next)
//         {
//             switch (next)
//             {
//                 // 로딩하면 입력도 막고, 세팅 초기화
//                 case eGameState.Loading:
//                 ResetController();
//                 m_Paused = true;
//                 break;
//                 // 멈춘거면 입력만 막기
//                 case eGameState.Paused:
//                 m_Paused = true;
//                 break;

//                 case eGameState.Running:
//                 m_Paused = false;                    
//                 if (prev != eGameState.Running)
//                 {
//                     ResetController();
//                 }
//                 break;
//             }
//         }
//         #endregion
//         #region Input Event Func
//         protected virtual void ClearTapInput()
//         {
//             interactInputTap = false;


//             attackInputTap = false;
//         }

//         protected virtual void InputBind()
//         {
//             if (showInitialDebug)
//             {
//                 Debug.Log("인풋 초기화");
//             }
//             m_Input.Jump += (value) => jumpInput = value;
//             m_Input.Focus += (value) => focusInput = value;
//             m_Input.Shift += (value) => shiftInput = value;
//             m_Input.Attack += (value) => attackInput = value;
//             m_Input.AttackTap += () => attackInputTap = true;
//             m_Input.InteractTap += () => interactInputTap = true;
//         }
//         #endregion
//         protected abstract void SetupStateMachine();

//         #region Aim Func
//         public Vector3 CalculateAimPoint(Vector3 origin, Vector3 direction, LayerMask hitLayerMask)
//         {
//             Ray ray = new Ray(origin, direction);
//             if (Physics.Raycast(ray, out RaycastHit hit, cameraHandler.cameraLookProfile.MaxLookRange, hitLayerMask))
//             {
//                 return hit.point;
//             }
//             return origin + direction * cameraHandler.cameraLookProfile.MaxLookRange;
//         }
//         #endregion
//     }
// }
