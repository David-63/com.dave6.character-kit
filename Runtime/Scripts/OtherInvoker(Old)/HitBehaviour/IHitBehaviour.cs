namespace Dave6.CharacterKit
{
    public interface IHitBehaviour
    {
        int order {get;}
        void Clear();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns>투사체 유지 여부</returns>
        bool ProcessHit(ref HitContext context);
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