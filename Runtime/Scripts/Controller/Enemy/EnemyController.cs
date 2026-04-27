using Dave6.CharacterKit.Handler.Mover;
using Dave6.Foundation.GameLogic.State;
using Dave6.StatSystem;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class EnemyController : MonoBehaviour, IStatController, IStatReceiver, ITargetable
    {
        #region 인터페이스 구현 필드 | (스텟, 타겟팅 요소)
        [Header("스탯 세팅")]
        [SerializeField] StatDatabase _StatDatabase;
        public StatDatabase StatDatabase => _StatDatabase;
        public StatHandler StatHandler { get; private set; }
        [SerializeField] StatTag _HealthStatTag;
        [SerializeField] StatTag _MoveSpeedStatTag;
        public ResourceStat MyHealth { get; set; }

        [Header("Targetable 트랜스폼 세팅")]
        [SerializeField] Transform _TargetTransform;
        public Transform TargetTransform => _TargetTransform;
        #endregion

        #region 컨트롤 필드
        BaseMover _EnemyMover;
        StateMachine _CoreStateMachine;

        #endregion

        void Awake()
        {
            gameObject.layer = 6;
            Init_StatHandler();
            Init_Control();
        }
        void OnDestroy()
        {
            MyHealth.onCurrentValueChanged -= CheckHealth;
        }

        void Start()
        {
        }

        void Update()
        {
            StatHandler.OnUpdate();
        }

        #region 초기화
        public void Init_StatHandler()
        {
            StatHandler = new StatHandler(_StatDatabase);
            StatHandler.TryGetStat(_HealthStatTag, out var healthStat);
            MyHealth = healthStat as ResourceStat;
            MyHealth.onCurrentValueChanged += CheckHealth;
        }
        void Init_Control()
        {
            _EnemyMover = GetComponent<BaseMover>();
            _CoreStateMachine = new();
        }
        #endregion

        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }
        public void CheckHealth()
        {
            Debug.Log($"enemy Helth: {MyHealth.currentValue}/{MyHealth.finalValue}");
            if (MyHealth.currentValue <= 0)
            {
                Destroy(gameObject);
            }
        }



    }

}
