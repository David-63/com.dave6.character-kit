using Dave6.StatSystem;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit
{
    public class EnemyController : MonoBehaviour, IStatController, IStatReceiver, ITargetable
    {
        [Header("스텟 세팅")]
        [SerializeField] StatDatabase m_StatDatabase;
        public StatDatabase statDatabase => m_StatDatabase;
        public StatHandler statHandler { get; private set; }
        public ResourceStat myHealth { get; set; }

        [Header("공격 세팅")]
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] StatTag healthTag;
        [SerializeField] float attackDelay = 3f;
        Timer m_AttackTimer;

        [SerializeField] Transform m_TargetTransform;
        public Transform targetTransform => m_TargetTransform;

        void Awake()
        {
            gameObject.layer = 6;
            Init_StatHandler();
            m_AttackTimer = new Countdown(attackDelay);
            m_AttackTimer.OnTimerStop += DoFire;
        }
        void OnDestroy()
        {
            myHealth.onCurrentValueChanged -= CheckHealth;
            if (m_AttackTimer != null)
            {
                m_AttackTimer.OnTimerStop -= DoFire;
                m_AttackTimer.Stop();
            }
        }

        void Start()
        {
            m_AttackTimer.Start();
        }

        void Update()
        {
            statHandler.OnUpdate();
        }

        public void Init_StatHandler()
        {
            statHandler = new StatHandler(m_StatDatabase);
            statHandler.InitializeStat();
            statHandler.TryGetStat(healthTag, out var healthStat);
            myHealth = healthStat as ResourceStat;
            myHealth.onCurrentValueChanged += CheckHealth;
        }

        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }
        public void CheckHealth()
        {
            Debug.Log($"enemy Helth: {myHealth.currentValue}/{myHealth.finalValue}");
            if (myHealth.currentValue <= 0)
            {
                Destroy(gameObject);
            }
        }

        void DoFire()
        {
            GameObject projectileObj = Instantiate(projectilePrefab, m_TargetTransform.position, m_TargetTransform.rotation);
            projectileObj.GetComponent<ProjectileMover>().Initialize(this);
            m_AttackTimer.RestartTimer();
        }
    }

}
