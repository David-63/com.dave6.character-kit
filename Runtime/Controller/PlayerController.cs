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

        GameStateMachine m_ActionStateMachine;

        CombatHandler m_CombatHandler;
        public CombatHandler combatHandler => m_CombatHandler;

        [SerializeField] GameObject m_HitColliderPrefab;
        public GameObject hitColliderPrefab => m_HitColliderPrefab;

        public override void Awake()
        {
            base.Awake();
            InitializeStat();
            m_CombatHandler = new();
        }

        public override void Start()
        {
            SetupStateMachine();
            m_Input.EnablePlayerAction();

            m_StateMachine.SetState(m_StateMachine.GetStateByType(typeof(FreeLookState)));
            m_ActionStateMachine.SetState(m_ActionStateMachine.GetStateByType(typeof(ActionIdleState)));
        }

        public override void Update()
        {
            base.Update();
            m_ActionStateMachine.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            m_ActionStateMachine.FixedUpdate();
        }
        public override void LateUpdate()
        {
            base.LateUpdate();
            m_ActionStateMachine.LateUpdate();
        }

        protected override void SetupStateMachine()
        {
            if (showInitialDebug)
            {
                Debug.Log("상태 초기화");
            }

            // Locomotion
            m_StateMachine = new();
            var freeLook = new FreeLookState(this);
            var strafeMove = new StrafeMoveState(this);
            At(m_StateMachine, freeLook, strafeMove, new FuncPredicate(() => aimInput));
            At(m_StateMachine, strafeMove, freeLook, new FuncPredicate(() => !aimInput));

            // Action
            m_ActionStateMachine = new();
            var actionIdle = new ActionIdleState(this);
            var actionMelee = new ActionMeleeState(this);
            var actionRange = new ActionRangeState(this);

            // 공격 진입
            
            At(m_ActionStateMachine, actionIdle, actionMelee, new FuncPredicate(() => attackInputTap && !aimInput));
            At(m_ActionStateMachine, actionIdle, actionRange, new FuncPredicate(() => aimInput));

            
            
            
            // 공격 전환
            
            At(m_ActionStateMachine, actionMelee, actionRange, new FuncPredicate(() => aimInput));
            At(m_ActionStateMachine, actionRange, actionMelee, new FuncPredicate(() => !aimInput && attackInputTap));

            
            // 공격 해제

            At(m_ActionStateMachine, actionMelee, actionIdle, new FuncPredicate(() => ConsumeExitMelee()));

            ///%%%
            At(m_ActionStateMachine, actionRange, actionIdle, new FuncPredicate(() => !aimInput && ConsumeExitRange())); // 이거 문제있음

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

        public GameObject CreateGameObject(GameObject obj)
        {
            return Instantiate(obj, transform);
        }
    }
}
