using Dave6.CharacterKit.AnimHandler;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.Handler.Combat;
using Dave6.CharacterKit.Handler.Loadout;
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
        // 카메라 제어
        public ThirdPersonCameraController CameraSystem {get; private set;}
        [SerializeField] Transform _CameraFollowTarget;

        // 입력 제어
        //[SerializeField] InputReader m_Input;
        public PlayerInputContext InputCtx {get; private set;}

        // 움직임 제어
        public PlayerMover Mover {get; private set;}
        public PlayerCombat Combat {get; private set;}

        // 애니메이션 제어
        AnimatorEventProxy _AnimEventProxy;
        public AnimatorHandler AnimHandler {get; private set;}
        public PlayerLoadout Loadout {get; private set;}


        // 상태 제어
        StateMachine _LocomotionSM;
        StateMachine _ActionSM;

        void Awake()
        {
            PlayerConnector.Instance.RegisterTarget(this);
            gameObject.layer = 3;
            // 카메라 바인딩
            CameraSystem = FindAnyObjectByType<ThirdPersonCameraController>();
            // 인풋 바인딩
            InputCtx = new();

            // 컴포넌트 바인딩
            // Mover
            Mover = GetComponent<Handler.Mover.PlayerMover>();
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

            // 필요하면. Stat, Loadout 도 추가
            Loadout = GetComponent<PlayerLoadout>();

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
            _ActionSM.SetDebug(true);

            var idle = new ActionIdleState(this);
            var melee = new ActionMeleeState(this);
            var range = new ActionRangeState(this);
            var reload = new ActionReloadState(this);

            _ActionSM.At(idle, melee, new FuncPredicate(() => Combat.IsMeleeState()));
            _ActionSM.At(idle, range, new FuncPredicate(() => Combat.IsRangeState()));
            _ActionSM.At(idle, reload, new FuncPredicate(() => Combat.CanReload()));

            _ActionSM.At(melee, range, new FuncPredicate(() => Combat.IsRangeState()));
            _ActionSM.At(range, melee, new FuncPredicate(() => Combat.IsMeleeState()));
            _ActionSM.At(melee, reload, new FuncPredicate(() => Combat.CanReload()));
            _ActionSM.At(range, reload, new FuncPredicate(() => Combat.CanReload()));

            _ActionSM.At(melee, idle, new FuncPredicate(() => Combat.ExitIs(EActionExitReason.LeaseExpired)));
            _ActionSM.At(range, idle, new FuncPredicate(() => !InputCtx.focus && Combat.ExitIs(EActionExitReason.LeaseExpired)));
            _ActionSM.At(reload, idle, new FuncPredicate(() => Combat.ReloadFinished()));

            _ActionSM.SetState(_ActionSM.GetStateByType(typeof(ActionIdleState)));
        }


        void Update()
        {
            //ReadInput();
            CameraSystem.OnUpdate(InputCtx.look);

            MoverFrameInput frameInput = new MoverFrameInput(Time.deltaTime, CameraSystem.ReferenceYaw, CameraSystem.CameraForward);
            Mover.OnUpdate(in frameInput);
            CameraSystem.SetMoveSpeed01(Mover.GetMoveSpeed01());
            //combat.OnUpdate();

            _LocomotionSM.Update();
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
    }
}
