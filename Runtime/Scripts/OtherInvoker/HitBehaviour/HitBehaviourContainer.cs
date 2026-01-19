using System;
using System.Collections.Generic;
using System.Linq;

namespace Dave6.CharacterKit
{
    public interface IHitModifier
    {
        void Apply(HitBehaviourContainer container);
        void Remove(HitBehaviourContainer container);
    }

    public class HitBehaviourContainer
    {
        Dictionary<Type, IHitBehaviour> m_Behaviours = new();
        public IEnumerable<IHitBehaviour> orderedBehaviours
        {
            get
            {
                return m_Behaviours.Values.OrderBy(b => b.order);
            }
        }

        public T Get<T>() where T : class, IHitBehaviour
        {
            m_Behaviours.TryGetValue(typeof(T), out var behaviour);
            return behaviour as T;
        }

        public T GetOrCreate<T>(Func<T> factory) where T : class, IHitBehaviour
        {
            if (!m_Behaviours.TryGetValue(typeof(T), out var behaviour))
            {
                behaviour = factory();
                m_Behaviours.Add(typeof(T), behaviour);
            }

            return behaviour as T;
        }

        public void Remove<T>() where T : IHitBehaviour
        {
            if (m_Behaviours.TryGetValue(typeof(T), out var behaviour))
            {
                behaviour.Clear();
                m_Behaviours.Remove(typeof(T));
            }
        }

        public IEnumerable<IHitBehaviour> All => m_Behaviours.Values;

        public void ClearRuntimeState()
        {
            foreach (var behaviour in m_Behaviours.Values)
            {
                behaviour.Clear();
            }
        }
        public void ClearConfiguration()
        {
            m_Behaviours.Clear();
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