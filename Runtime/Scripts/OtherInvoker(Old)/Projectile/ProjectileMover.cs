using System.Collections;
using System.Collections.Generic;
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
        public LayerMask hitLayer;
        public LayerMask targetLayer;
        [SerializeField] float m_MoveSpeed = 200f;
        [SerializeField] float m_ExistDuration = 3f;
        #endregion

        #region 스텟 세팅 필드
        [Header("스텟 세팅")]
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;
        public IStatController actor { get; private set; }
        [SerializeField] StatTag healthStatTag;
        Timer m_LifeTime;
        #endregion

        #region 비주얼 필드
        [SerializeField] ImpactType m_ImpactType;
        public ImpactType impactType => m_ImpactType;
        TrailRenderer trail;
        #endregion

        #region 풀 관리 필드
        PoolableObject m_Poolable;
        bool m_Spawned;
        bool m_MustStop;
        #endregion

        #region 투사체 제어 필드
        Vector3 m_Direction;

        public HitBehaviourContainer hitBehaviours {get; private set;}
        #endregion

        void Awake()
        {
            m_Poolable = GetComponent<PoolableObject>();

            m_LifeTime = new Countdown(m_ExistDuration);
            m_LifeTime.OnTimerStop += RequestRelease;

            trail = GetComponent<TrailRenderer>();

            hitBehaviours = new();
        }
        void OnDestroy()
        {
            if (m_LifeTime != null)
            {
                m_LifeTime.OnTimerStop -= RequestRelease;
                m_LifeTime.Stop();
            }
        }

        #region Pooling
        void RequestRelease()
        {
            if (!m_Spawned) return;

            m_Spawned = false;
            m_Poolable.Release();
        }
        public void OnSpawned()
        {
            m_Spawned = true;
            m_MustStop = false;

            m_Direction = transform.forward;

            m_LifeTime.RestartTimer();

            trail.emitting = false;
            trail.Clear();
            StartCoroutine(EnableTrailNextFrame());

            hitBehaviours.ClearRuntimeState();
        }

        public void OnDespawned()
        {
            m_LifeTime.Stop();
            actor = null;
        }
        #endregion

        public void BindOwner(IStatController actorEntity)
        {
            actor = actorEntity;
        }
        public void SetDirection(Vector3 direction)
        {
            m_Direction = direction.normalized;
            transform.rotation = Quaternion.LookRotation(m_Direction);
        }

        void Update()
        {
            if (!m_Spawned) return;
            if (m_MustStop) return;
            ProjectileMove();
        }

        void ProjectileMove()
        {
            Vector3 start = transform.position;
            Vector3 displacement = m_Direction * m_MoveSpeed * Time.deltaTime;
            float distance = displacement.magnitude;

            if (!Physics.Raycast(start, displacement.normalized, out RaycastHit hit, distance, hitLayer))
            {
                transform.position = start + displacement;
                return;
            }

            var hitContext = new HitContext
            {
                hit = hit,
                hitTarget = hit.collider.gameObject,
                position = hit.point,
                direction = m_Direction,
                remainingDistance = distance - hit.distance,
                projectile = this,
                shouldContinue = true
            };

            // hitBehaviour 체인
            foreach (var behaviour in hitBehaviours.orderedBehaviours)
            {
                if (!behaviour.ProcessHit(ref hitContext))
                {
                    hitContext.shouldContinue = false;
                    break;
                }
            }

            m_Direction = hitContext.direction;

            if (!hitContext.shouldContinue)
            {
                StopProjectile();
                return;
            }

            transform.position = hitContext.position + m_Direction * hitContext.remainingDistance;
        }

        void StopProjectile()
        {
            m_MustStop = true;
            trail.emitting = false;
        }

        public void Invoke(IStatReceiver target)
        {
            // 상대 스탯 가져오기
            target.StatHandler.TryGetStat(healthStatTag, out var targetHealth);

            // 본인 스텟을 상대 스탯에 때려박기
            actor.StatHandler.CreateEffectInstance(effectDefinition, targetHealth);
        }

        IEnumerator EnableTrailNextFrame()
        {
            yield return null; // 다음 프레임
            trail.emitting = true;
        }

        public void ResetConfiguration()
        {
            hitBehaviours.ClearConfiguration();
            hitBehaviours.GetOrCreate(() => new DamageOnHit());
        }
    }
}

/*
이런식으로 사용하도록
    for each activeBehaviour
        behaviour.ProcessHit(context)
        if context.shouldStop → break
*/

/*
            if (Physics.Raycast(start, displacement.normalized, out RaycastHit hit, distance, m_HitLayer))
            {
                // 투사체 제어
                m_MustStop = true;
                transform.position = hit.point;

                // 트레일 제어
                trail.emitting = false;

                // 스텟 적용
                if (hit.collider.TryGetComponent<IStatReceiver>(out var entity))
                {
                    entity.Accept(this);
                }

                // 이팩트 적용
                var context = new ImpactContext(hit.collider.gameObject, hit.point, hit.normal, impactType);
                SurfaceReactionService.instance.ProcessImpact(context);

            }
            else
            {
                transform.position = end;
            }


*/