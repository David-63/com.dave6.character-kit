using Dave6.CharacterKit.States;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;
using UnityUtils;

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

        public override void Awake()
        {
            base.Awake();
            InitializeStat();
        }

        public override void Start()
        {
            SetupStateMachine();
            m_Input.EnablePlayerAction();

            m_StateMachine.SetState(m_StateMachine.GetStateByType(typeof(FreeLookState)));
        }

        protected override void SetupStateMachine()
        {
            if (showInitialDebug)
            {
                Debug.Log("상태 초기화");
            }
            // FSM 생성 및 상태 정의
            m_StateMachine = new GameStateMachine();
            var freeLook = new FreeLookState(this);
            var strafeMove = new StrafeMoveState(this);
            At(freeLook, strafeMove, new FuncPredicate(() => aimInput));
            At(strafeMove, freeLook, new FuncPredicate(() => !aimInput));
        }

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
    }
}
