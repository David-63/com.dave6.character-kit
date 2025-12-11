using Dave6.CharacterKit.Input;
using Dave6.StateMachine;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public abstract class BasicPlayerController : MonoBehaviour
    {
        #region control field
        public bool showInitialDebug = false;
        [SerializeField] protected InputReader m_Input;
        public InputReader GetInputReader() => m_Input;
        protected BasicMover m_Mover;
        public BasicMover GetMover() => m_Mover;
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
        protected GameStateMachine m_StateMachine;


        public virtual void Awake()
        {
            m_Mover = GetComponent<BasicMover>();
            gameObject.layer = 3;
            
            if (m_Input == null)
            {
                Debug.Log("인풋 추가 안했음");
            }
            EventBind();
        }

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public abstract void Start();

        // Update is called once per frame
        public virtual void Update()
        {
            m_StateMachine.Update();
        }
        public virtual void FixedUpdate()
        {
            m_StateMachine.FixedUpdate();
        }
        public virtual void LateUpdate()
        {
            m_StateMachine.LateUpdate();
        }

        protected virtual void EventBind()
        {
            if (showInitialDebug)
            {
                Debug.Log("인풋 초기화");
            }
            m_Input.Jump += (value) => m_JumpInput = value;
            m_Input.Aim += (value) => m_AimInput = value;
            m_Input.Shift += (value) => m_ShiftInput = value;
        }
        protected abstract void SetupStateMachine();

        protected void At(IState from, IState to, IPredicate condition) => m_StateMachine.AddTransition(from, to, condition);
        protected void Any(IState to, IPredicate condition) => m_StateMachine.AddAnyTransition(to, condition);

    }
}
