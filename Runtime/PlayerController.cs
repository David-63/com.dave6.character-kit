using System;
using Dave6.CharacterKit.Input;
using Dave6.CharacterKit.Movement;
using Dave6.CharacterKit.States;
using Dave6.StateMachine;
using UnityEngine;
using UnityEngine.Events;

namespace Dave6.CharacterKit
{
    public class PlayerController : MonoBehaviour
    {
        public bool showInitialDebug = false;
        [SerializeField] InputReader input;
        public InputReader GetInputReader() => input;
        CharacterMover mover;
        public CharacterMover GetMover() => mover;
        

        #region input messenger
        public Vector3 InputMove => new Vector3(input.InputMove.x, 0, input.InputMove.y);
        public Vector2 InputLook => input.InputLook;

        public bool JumpInput = false;
        public bool AimInput = false;
        public bool ShiftInput = false;
        #endregion


        #region movement value field
        float _targetSpeed { get; set; }
        public float TargetSpeed { get => _targetSpeed; set => _targetSpeed = value; }
        float _horizontalSpeed;
        public float HorizontalSpeed { get => _horizontalSpeed; set => _horizontalSpeed = value; }
        float _verticalSpeed;
        public float VerticalSpeed { get => _verticalSpeed; set => _verticalSpeed = value; }
        Vector3 _moveDirection;
        public Vector3 MoveDirection { get => _moveDirection; set => _moveDirection = value; }
        #endregion
        public bool HasMovementInput() => InputMove.x != 0 || InputMove.z != 0;

        GameStateMachine stateMachine;
        

        void Awake()
        {
            mover = GetComponent<CharacterMover>();
            
            if (input == null)
            {
                Debug.Log("인풋 추가 안했음");
            }

            SetupStateMachine();

            EventBind();
        }

        void Start()
        {
            input.EnablePlayerAction();
            stateMachine.SetState(stateMachine.GetStateByType(typeof(FreeLookState)));
        }

        void Update()
        {
            stateMachine.Update();
        }
        void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }
        void LateUpdate()
        {
            stateMachine.LateUpdate();
        }

        void EventBind()
        {
            if (showInitialDebug)
            {
                Debug.Log("인풋 초기화");
            }
            input.Jump += (value) => JumpInput = value;
            input.Aim += (value) => AimInput = value;
            input.Shift += (value) => ShiftInput = value;
        }

        void SetupStateMachine()
        {
            if (showInitialDebug)
            {
                Debug.Log("상태 초기화");
            }
            // FSM 생성 및 상태 정의
            stateMachine = new GameStateMachine();
            var freeLook = new FreeLookState(this);
            var strafeMove = new StrafeMoveState(this);
            At(freeLook, strafeMove, new FuncPredicate(() => AimInput));
            At(strafeMove, freeLook, new FuncPredicate(() => !AimInput));
        }
        void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);


    }
}