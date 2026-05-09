using Dave6.CharacterKit.AnimHandler;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Input;
using Dave6.CharacterKit.Handler.Combat;
using Dave6.CharacterKit.Handler.Interactor;
using Dave6.CharacterKit.Handler.Mover;
using Dave6.CharacterKit.Inputs;
using Dave6.CharacterKit.Player.States;
using Dave6.Foundation.GameLogic.State;
using Dave6.ThirdPersonCamera;
using UnityEngine;
using UnityUtils;

namespace Dave6.CharacterKit.Player
{
    public class PlayerController : MonoBehaviour, IInputReceiver
    {
        [SerializeField] bool _DebugActionState = false;
        // 카메라 제어
        public ThirdPersonCameraController CameraSystem {get; private set;}
        [SerializeField] Transform _CameraFollowTarget;

        // 입력 제어
        public PlayerInputContext InputCtx {get; private set;}

        // 움직임 제어
        public PlayerMover Mover {get; private set;}
        public PlayerCombat Combat {get; private set;}

        // 애니메이션 제어
        AnimatorEventProxy _AnimEventProxy;
        public AnimatorHandler AnimHandler {get; private set;}

        // 상태 제어
        StateMachine _LocomotionSM;
        StateMachine _ActionSM;

        // Interactor 제어
        public PlayerInteractor Interactor {get; private set;}

        /// <summary>
        ///  초기화 진행은 partal 클래스로 나눠서
        ///  초기화 전용 파일을 따로 만들 예정
        /// </summary>
        void Awake()
        {
            PlayerSpawner.Instance.SetPlayer(gameObject);
            var playerInput = FindFirstObjectByType<PlayerInputRouter>();
            playerInput.SetTarget(this);
            gameObject.layer = 3;
            CameraSystem = FindAnyObjectByType<ThirdPersonCameraController>();
            InputCtx = new();

            // 컴포넌트 바인딩
            // Mover
            Mover = GetComponent<PlayerMover>();
            // Combat
            Combat = GetComponent<PlayerCombat>();
            Combat.BindInput(InputCtx);

            // Anim
            if (this.TryGetComponentInChildren<AnimatorEventProxy>(out var proxy))
            {
                _AnimEventProxy = proxy;
            }
            if (this.TryGetComponentInChildren<Animator>(out var animator))
            {
                AnimHandler = GetComponent<AnimatorHandler>();
                AnimHandler.RegisterAnimator(animator, _AnimEventProxy);
            }
            // 애니메이션 이벤트 바인딩
            AnimHandler.OnReloadFinishedAction += Combat.HandleReloadEnd;
            AnimHandler.OnAttackFinishedAction += Combat.HandleAttackEnd;

            // Interactor
            Interactor = GetComponent<PlayerInteractor>();
            Interactor.BindCamera(CameraSystem);
            Interactor.BindInput(InputCtx);
        }

        void Start()
        {
            CameraSystem.SetFollowTarget(_CameraFollowTarget);
            SetupStateMachine();
        }

        void SetupStateMachine()
        {
            _LocomotionSM = new();

            var freelook = new FreelookState(this);
            var strafe = new StrafeState(this);

            _LocomotionSM.At(freelook, strafe, new FuncPredicate(() => IsInFocus()));
            _LocomotionSM.At(strafe, freelook, new FuncPredicate(() => !IsInFocus()));
            _LocomotionSM.SetState(_LocomotionSM.GetStateByType(typeof(FreelookState)));

            _ActionSM = new();

            var idle = new ActionIdleState(this);
            var melee = new ActionMeleeState(this);
            var range = new ActionRangeState(this);
            var reload = new ActionReloadState(this);
            var interact = new ActionInteractState(this);

            _ActionSM.Any(melee, new FuncPredicate(() => Combat.ShouldEnterMelee()));
            _ActionSM.Any(range, new FuncPredicate(() => Combat.ShouldEnterRange()));
            _ActionSM.Any(reload, new FuncPredicate(() => Combat.ShouldEnterReload()));
            _ActionSM.Any(interact, new FuncPredicate(() => Interactor.ShouldEnterInteract()));

            if (_DebugActionState)
            {
                _ActionSM.SetDebug(_DebugActionState);
            }

            // 종료 조건
            _ActionSM.At(melee, idle, new FuncPredicate(() => true));
            _ActionSM.At(range, idle, new FuncPredicate(() => true));
            _ActionSM.At(reload, idle, new FuncPredicate(() => true));
            _ActionSM.At(interact, idle, new FuncPredicate(() => true));

            _ActionSM.SetState(_ActionSM.GetStateByType(typeof(ActionIdleState)));
        }

        void Update()
        {
            CameraSystem.OnUpdate(InputCtx.look);

            MoverFrameInput frameInput = new MoverFrameInput(Time.deltaTime, CameraSystem.ReferenceYaw, CameraSystem.CameraForward);
            Mover.OnUpdate(in frameInput);
            CameraSystem.SetMoveSpeed01(Mover.GetMoveSpeed01());
            Interactor.OnUpdate();

            _LocomotionSM.Update();
            Combat.OnUpdate();
            _ActionSM.Update();

            ClearInput();
        }

        void FixedUpdate()
        {
            _LocomotionSM.FixedUpdate();
        }

        void LateUpdate()
        {
            _LocomotionSM.LateUpdate();
        }

        #region 인풋 로직
        bool IsInFocus() => InputCtx.focus;

        void ClearInput()
        {
            InputCtx.shiftTap = false;

            InputCtx.reloadTap = false;
            InputCtx.attackTap = false;

            InputCtx.interactTap = false;
        }

        public void OnMove(Vector2 value)
        {
            InputCtx.move = value;
        }

        public void OnLook(Vector2 value)
        {
            InputCtx.look = value;
        }

        public void OnAction(ActionType type, bool isPressed)
        {
            switch (type)
            {
                case ActionType.Jump:
                InputCtx.jump = isPressed;
                break;
                case ActionType.Shift:
                InputCtx.shift = isPressed;
                break;
                case ActionType.Focus:
                InputCtx.focus = isPressed;
                break;
                case ActionType.Attack:
                InputCtx.attack = isPressed;
                break;
                case ActionType.Reload:
                InputCtx.reload = isPressed;
                break;
                case ActionType.Interact:
                InputCtx.interact = isPressed;
                break;
            }
        }

        public void OnTap(ActionType type)
        {
            switch (type)
            {
                case ActionType.Shift:
                InputCtx.shiftTap = true;
                break;
                case ActionType.Attack:
                InputCtx.attackTap = true;
                break;
                case ActionType.Reload:
                InputCtx.reloadTap = true;
                break;
                case ActionType.Interact:
                InputCtx.interactTap = true;
                break;
            }
        }
        #endregion
    }
}
