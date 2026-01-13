using Dave6.ObjectPoolingSystem;
using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using Dave6.SurfaceReactionSystem;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit
{
    public class ProjectileMover : MonoBehaviour, IStatInvoker, IPoolable
    {
        #region 투사체 속성 필드
        [SerializeField] LayerMask m_HitLayer;
        [SerializeField] float m_MoveSpeed = 200f;
        [SerializeField] float m_ExistDuration = 3f;
        #endregion

        #region 스텟 세팅 필드
        [Header("스텟 세팅")]
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;
        public IStatController actor { get; private set; }
        [SerializeField] StatTag healthStatTag;
        Vector3 m_PreviousPosition;
        Timer m_LifeTime;
        #endregion
        #region 비주얼 필드
        [SerializeField] ImpactType impactType;
        #endregion
        #region 풀 관리 필드
        PoolableObject m_Poolable;
        bool m_Spawned;

        bool m_HasMoved;

        bool m_MustStop;
        #endregion

        void Awake()
        {
            m_Poolable = GetComponent<PoolableObject>();

            m_LifeTime = new Countdown(m_ExistDuration);
            m_LifeTime.OnTimerStop += RequestRelease;
        }
        void OnDestroy()
        {
            if (m_LifeTime != null)
            {
                m_LifeTime.OnTimerStop -= RequestRelease;
                m_LifeTime.Stop();
            }
        }

        void RequestRelease()
        {
            if (!m_Spawned) return;

            m_Spawned = false;
            m_Poolable.Release();
        }

        public void Initialize(IStatController actorEntity)
        {
            actor = actorEntity;
        }
        // Update is called once per frame
        void Update()
        {
            if (!m_Spawned) return;
            if (m_MustStop) return;
            transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
            m_HasMoved = true;
        }
        void LateUpdate()
        {
            if (!m_Spawned) return;
            if (!m_HasMoved) return;
            if (m_MustStop) return;
            ProjectileRaycast();
        }

        void ProjectileRaycast()
        {
            Vector3 rayDirection = transform.position - m_PreviousPosition;
            float rayDistance = rayDirection.magnitude;
            RaycastHit hit;
            if (Physics.Raycast(m_PreviousPosition, rayDirection.normalized, out hit, rayDistance, m_HitLayer))
            {
                // 여기서 충돌 로직 처리
                if (hit.collider.TryGetComponent<IStatReceiver>(out var entity))
                {
                    entity.Accept(this);
                }

                var context = new ImpactContext(hit.collider.gameObject, hit.point, hit.normal, impactType);
                SurfaceReactionService.instance.ProcessImpact(context);
                m_MustStop = true;
            }

            m_PreviousPosition = transform.position;
        }

        public void Invoke<T>(T target) where T : Component, IStatReceiver
        {
            // 상대 스탯 가져오기
            IStatController entity = target as IStatController;

            entity.statHandler.TryGetStat(healthStatTag, out var targetHealth);

            // 본인 스텟을 상대 스탯에 때려박기
            actor.statHandler.CreateEffectInstance(effectDefinition, targetHealth);
        }

        #region Pooling
        public void OnSpawned()
        {
            m_Spawned = true;
            m_MustStop = false;

            m_PreviousPosition = transform.position;

            m_LifeTime.RestartTimer();
        }

        public void OnDespawned()
        {
            m_LifeTime.Stop();
            actor = null;
            m_HasMoved = false;
        }
        #endregion
    }
}
