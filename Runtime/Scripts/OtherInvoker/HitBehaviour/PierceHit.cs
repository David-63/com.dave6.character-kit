using Dave6.SurfaceReactionSystem;

namespace Dave6.CharacterKit
{
    public class PierceHit : IHitBehaviour
    {
        public int order =>3;
        int m_MaxPierce;
        int m_CurPierce;

        public void AddPierce(int value)
        {
            m_MaxPierce += value;
        }
        public void RemovePierce(int value)
        {
            m_MaxPierce -= value;
        }
        public bool IsEmpty => m_MaxPierce <= 0;
        public void Clear()
        {
            m_CurPierce = 0;
        }
        public void Reset()
        {
            m_MaxPierce = 0;
            m_CurPierce = 0;
        }
        public bool ProcessHit(ref HitContext context)
        {
            if (m_CurPierce >= m_MaxPierce) return false;
            m_CurPierce++;

            return true;
        }
    }
    
    public class PierceModifier : IHitModifier
    {
        int m_Value;
        public PierceModifier(int value)
        {
            m_Value = value;
        }

        public void Apply(HitBehaviourContainer container)
        {
            var pierce = container.GetOrCreate(() => new PierceHit());
            pierce.AddPierce(m_Value);
        }
        public void Remove(HitBehaviourContainer container)
        {
            var pierce = container.Get<PierceHit>();
            if (pierce == null) return;

            pierce.RemovePierce(m_Value);

            if (pierce.IsEmpty)
            {
                container.Remove<PierceHit>();
            }
        }
    }
}