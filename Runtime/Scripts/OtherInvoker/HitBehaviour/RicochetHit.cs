using Dave6.StatSystem.Interaction;
using Dave6.SurfaceReactionSystem;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class RicochetHit : IHitBehaviour
    {
        public int order => 1;
        int m_MaxBounce;
        int m_CurBounce;

        public void AddBounce(int value)
        {
            m_MaxBounce += value;
        }
        public void RemoveBounce(int value)
        {
            m_MaxBounce -= value;
        }

        public bool IsEmpty => m_MaxBounce <= 0;

        public void Clear()
        {
            m_CurBounce = 0;
        }

        public void Reset()
        {
            m_MaxBounce = 0;
            m_CurBounce = 0;
        }
        public bool ProcessHit(ref HitContext context)
        {
            if (m_CurBounce >= m_MaxBounce) return false;

            if (context.hitTarget.TryGetComponent<IStatReceiver>(out _)) return true;
            m_CurBounce++;

            Vector3 reflected = Vector3.Reflect(context.direction, context.hit.normal).normalized;
            context.position = context.hit.point + reflected * 0.01f;
            context.direction = reflected;

            context.ricocheted = true;

            return true;
        }
    }

    public class RicochetModifier : IHitModifier
    {
        int m_Value;
        public RicochetModifier(int value)
        {
            m_Value = value;
        }

        public void Apply(HitBehaviourContainer container)
        {
            var ricochet = container.GetOrCreate(() => new RicochetHit());
            ricochet.AddBounce(m_Value);
        }
        public void Remove(HitBehaviourContainer container)
        {
            var ricochet = container.Get<RicochetHit>();
            if (ricochet == null) return;

            ricochet.RemoveBounce(m_Value);

            if (ricochet.IsEmpty)
            {
                container.Remove<RicochetHit>();
            }
        }
    }
}