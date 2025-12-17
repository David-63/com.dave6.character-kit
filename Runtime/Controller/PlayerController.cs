using Dave6.CharacterKit.Combat;
using Dave6.CharacterKit.States;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class PlayerController : BasicPlayerController, IEntity, IStatReceiver
    {
        #region stat field
        [SerializeField] StatDatabase m_StatDatabase;
        public StatDatabase statDatabase => m_StatDatabase;

        StatHandler m_StatHandler;
        public StatHandler statHandler => m_StatHandler;

        public ResourceStat health { get; set; }
        #endregion

        MinimumStateMachine m_ActionStateMachine;

        CombatHandler m_CombatHandler;
        public CombatHandler combatHandler => m_CombatHandler;

        public override void Awake()
        {
            base.Awake();
            InitializeStat();
            m_CombatHandler = GetComponent<CombatHandler>();
            m_CombatHandler.RegisterCombat(this);
        }

        public override void Start()
        {
            m_Input.EnablePlayerAction();
            // 애니메이션 이벤트 바인딩
            m_AnimEventProxy.onAttackFinishEvent += animatorHandler.OnAttackAnimationEnd;

            // 상태 처리
            SetupStateMachine();

            m_LocomotionStateMachine.SetState(m_LocomotionStateMachine.GetStateByType(typeof(FreeLookState)));
            m_ActionStateMachine.SetState(m_ActionStateMachine.GetStateByType(typeof(ActionIdleState)));
        }

        public override void Update()
        {
            m_LocomotionStateMachine.Update();

            m_Mover.OnUpdate();

            m_ActionStateMachine.Update();
        }

        public override void FixedUpdate()
        {
            m_LocomotionStateMachine.FixedUpdate();
            m_ActionStateMachine.FixedUpdate();
        }
        public override void LateUpdate()
        {
            m_LocomotionStateMachine.LateUpdate();

            m_CameraHandler.OnLateUpdate();
            m_CombatHandler.OnLateUpdate();

            m_ActionStateMachine.LateUpdate();

            ClearTapInput();
        }

        protected override void SetupStateMachine()
        {
            if (showInitialDebug)
            {
                Debug.Log("상태 초기화");
            }

            // Locomotion
            m_LocomotionStateMachine = new();
            var freeLook = new FreeLookState(this);
            var strafeMove = new StrafeMoveState(this);
            m_LocomotionStateMachine.At(freeLook, strafeMove, new FuncPredicate(() => aimInput));
            m_LocomotionStateMachine.At(strafeMove, freeLook, new FuncPredicate(() => !aimInput));

            // Action
            m_ActionStateMachine = new();
            var actionIdle = new ActionIdleState(this);
            var actionMelee = new ActionMeleeState(this);
            var actionRange = new ActionRangeState(this);

            // 공격 진입
            m_ActionStateMachine.At(actionIdle, actionMelee, new FuncPredicate(() => attackInputTap && !aimInput));
            m_ActionStateMachine.At(actionIdle, actionRange, new FuncPredicate(() => aimInput));

            
            // 공격 전환
            m_ActionStateMachine.At(actionMelee, actionRange, new FuncPredicate(() => aimInput));
            m_ActionStateMachine.At(actionRange, actionMelee, new FuncPredicate(() => !aimInput && attackInputTap));

            
            // 공격 해제
            m_ActionStateMachine.At(actionMelee, actionIdle, new FuncPredicate(() => ConsumeExitMelee()));

            ///%%%
            m_ActionStateMachine.At(actionRange, actionIdle, new FuncPredicate(() => !aimInput && ConsumeExitRange()));

        }

        #region Stat System
        public void InitializeStat()
        {
            m_StatHandler = new StatHandler(m_StatDatabase);
            m_StatHandler.InitializeStat();

            if (showInitialDebug)
            {
                foreach (var stat in m_StatHandler.stats)
                {
                    Debug.Log($"{stat.Key}");
                }
            }
        }

        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }
        #endregion

        #region 상태 제어
        public bool enterAttackFlag;
        public bool exitMeleeFlag;
        public bool exitRangeFlag;

        /*
            Idle 진입 조건
            1. 공격이 끝나야함
        */
        bool ConsumeExitMelee()
        {
            if (!exitMeleeFlag) return false;
            exitMeleeFlag = false;
            return true;
        }
        bool ConsumeExitRange()
        {
            if (!exitRangeFlag) return false;
            exitRangeFlag = false;
            return true;
        }
        #endregion

        public GameObject InstantiatePrefab(GameObject obj)
        {
            return Instantiate(obj);
        }
        public GameObject InstantiatePrefabSetParent(GameObject obj)
        {
            return Instantiate(obj, transform);
        }
        public GameObject InstantiatePrefab(GameObject obj, Vector3 position, Quaternion rotation)
        {
            return Instantiate(obj, position, rotation);
        }
    }
}
