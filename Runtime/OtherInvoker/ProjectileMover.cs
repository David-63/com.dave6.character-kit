using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit
{
    public class ProjectileMover : MonoBehaviour, IStatInvoker
    {
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;

        [SerializeField] float m_MoveSpeed = 200f;
        [SerializeField] float m_ExistDuration = 1f;
        IStatController m_Actor;
        public IStatController actor => m_Actor;
        Vector3 m_PreviousPosition;
        Timer m_LifeTime;

        void Awake()
        {
            m_LifeTime = new Countdown(m_ExistDuration);
            m_LifeTime.OnTimerStop += Disappear;
        }
        void OnDestroy()
        {
            if (m_LifeTime != null)
            {
                m_LifeTime.OnTimerStop -= Disappear;
                m_LifeTime.Stop();
            }
        }

        void Disappear()
        {
            Destroy(gameObject);
        }

        void Start()
        {
            // 위치 초기화
            m_PreviousPosition = transform.position;
            m_LifeTime.Start();
        }

        public void Initialize(IStatController actorEntity)
        {
            m_Actor = actorEntity;
        }
        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
        }
        void LateUpdate()
        {
            Vector3 rayDirection = transform.position - m_PreviousPosition;
            float rayDistance = rayDirection.magnitude;
            RaycastHit hit;
            if (Physics.Raycast(m_PreviousPosition, rayDirection.normalized, out hit, rayDistance))
            {
                // 여기서 충돌 로직 처리
                if (hit.collider.TryGetComponent<IStatReceiver>(out var entity))
                {
                    entity.Accept(this);
                }
            }

            m_PreviousPosition = transform.position;
        }


        public void Invoke<T>(T target) where T : Component, IStatReceiver
        {
            // 상대 스탯 가져오기
            IStatController entity = target as IStatController;
            var stat = entity.statHandler.GetHealthStat();

            // 본인 스텟을 상대 스탯에 때려박기
            m_Actor.statHandler.ApplyEffect(effectDefinition, stat);
            Debug.Log($"target Helth: {stat.currentValue}/{stat.finalValue}");

            entity.CheckHealth();

            m_LifeTime.Stop();
        }
    }
}
