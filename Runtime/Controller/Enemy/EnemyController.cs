using Dave6.StatSystem;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit
{
    public class EnemyController : MonoBehaviour, IStatController, IStatReceiver, ITargetable
    {
        [SerializeField] StatDatabase m_StatDatabase;
        public StatDatabase statDatabase => m_StatDatabase;

        StatHandler m_StatHandler;
        public StatHandler statHandler => m_StatHandler;

        public ResourceStat health { get; set; }

        [SerializeField] GameObject projectilePrefab;

        Timer m_AttackTimer;
        [SerializeField] float attackDelay = 3f;


        [SerializeField] Transform m_TargetTransform;
        public Transform targetTransform => m_TargetTransform;

        void Awake()
        {
            gameObject.layer = 6;
            InitializeStat();
            m_AttackTimer = new Countdown(attackDelay);
            m_AttackTimer.OnTimerStop += DoFire;
        }
        void OnDestroy()
        {
            if (m_AttackTimer != null)
            {
                m_AttackTimer.OnTimerStop -= DoFire;
                m_AttackTimer.Stop();
            }
        }

        void Start()
        {
            health = m_StatHandler.GetHealthStat();
            m_AttackTimer.Start();
        }

        public void InitializeStat()
        {
            m_StatHandler = new StatHandler(m_StatDatabase);
            m_StatHandler.InitializeStat();
        }

        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }
        public void CheckHealth()
        {
            if (health.currentValue <= 0)
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
