using Dave6.StatSystem.Interaction;
using Dave6.SurfaceReactionSystem;

namespace Dave6.CharacterKit
{
    public class DamageOnHit : IHitBehaviour
    {
        public int order => 0;
        public void Clear() {}
        public bool ProcessHit(ref HitContext context)
        {
            if (context.hitTarget.TryGetComponent<IStatReceiver>(out var entity))
            {
                entity.Accept(context.projectile);
            }
            
            var impactcontext = new ImpactContext(context.hitTarget, context.hit.point, context.hit.normal, context.projectile.impactType);
            SurfaceReactionService.instance.ProcessImpact(impactcontext);

            return true;
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