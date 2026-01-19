using UnityEngine;

namespace Dave6.CharacterKit
{
    public class TargetAssist : IHitBehaviour
    {
        public int order => 2;
        float m_SearchRadius;
        float m_MaxAngle;

        public TargetAssist(float radius, float maxAngle)
        {
            m_SearchRadius = radius;
            m_MaxAngle = maxAngle;
        }
        public void Clear() {}

        public bool ProcessHit(ref HitContext context)
        {
            if (!context.ricocheted) return true;

            Vector3 origin = context.position;
            Vector3 forward = context.direction;

            // 기존 방향
            Debug.DrawRay(context.position, context.direction * 5f, Color.red, 0.5f);
            // 탐색 반경
            

            Collider[] hits = Physics.OverlapSphere(origin, m_SearchRadius, context.projectile.targetLayer);
            Transform bestTarget = null;
            float baseAngle = m_MaxAngle;

            foreach (var collider in hits)
            {
                Vector3 dirToTarget = (collider.transform.position - origin).normalized;
                float angle = Vector3.Angle(forward, dirToTarget);

                if (angle < baseAngle)
                {
                    baseAngle = angle;
                    bestTarget = collider.transform;
                }
            }

            if (bestTarget != null)
            {
                context.direction = (bestTarget.position - origin).normalized;
                Debug.DrawLine(context.position, bestTarget.position, Color.green, 0.5f);
            }
            context.ricocheted = false;

            return true;
        }
    }
    public class TargetAssistModifier : IHitModifier
    {
        float m_Radius;
        float m_MaxAngle;

        public TargetAssistModifier(float radius = 8f, float maxAngle = 45f)
        {
            m_Radius = radius;
            m_MaxAngle = maxAngle;
        }

        public void Apply(HitBehaviourContainer container)
        {
            container.GetOrCreate(() => new TargetAssist(m_Radius, m_MaxAngle));
        }

        public void Remove(HitBehaviourContainer container)
        {
            container.Remove<TargetAssist>();
        }
    }
}