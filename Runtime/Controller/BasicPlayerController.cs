using System;
using Dave6.CharacterKit.AnimHandler;
using Dave6.CharacterKit.Input;
using Dave6.StateMachine;
using UnityEngine;
using UnityUtils;

namespace Dave6.CharacterKit
{
    public abstract class BasicPlayerController : MonoBehaviour
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
        #endregion


        #region input messenger
        public Vector3 inputMove => new Vector3(m_Input.InputMove.x, 0, m_Input.InputMove.y);
        public Vector2 inputLook => m_Input.InputLook;

        bool m_JumpInput = false;
        public bool jumpInput => m_JumpInput;
        bool m_AimInput = false;
        public bool aimInput => m_AimInput;
        bool m_ShiftInput = false;
        public bool shiftInput => m_ShiftInput;
        bool m_AttackInput = false;
        public bool attackInput => m_AttackInput;
        bool m_AttackInputTap = false;
        public bool attackInputTap => m_AttackInputTap;
        #endregion

        #region movement value field
        protected float m_TargetSpeed { get; set; }
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
            // 카메라 세팅
            m_CameraHandler = GetComponent<CameraHandler>();
            m_CameraHandler.RegisterCamera(this);

            // 무버 세팅
            m_Mover = GetComponent<BasicMover>();
            m_Mover.RegisterMover(this, m_CameraHandler);



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
            m_LocomotionStateMachine.Update();
        }
        public virtual void FixedUpdate()
        {
            m_LocomotionStateMachine.FixedUpdate();
        }
        public virtual void LateUpdate()
        {
            m_LocomotionStateMachine.LateUpdate();
            ClearTapInput();
        }

        void ClearTapInput()
        {
            m_AttackInputTap = false;
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
            
        }
        protected abstract void SetupStateMachine();
    }
}
