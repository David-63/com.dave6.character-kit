using UnityEngine;

namespace Dave6.CharacterKit
{
    public struct HitContext
    {
        // 충돌 정보
        public RaycastHit hit;              // 필수
        public GameObject hitTarget;

        // 투사체 상태
        public Vector3 position;
        public Vector3 direction;           // 필수

        // 이동 관련
        public float remainingDistance;

        // 실행 제어
        public bool shouldContinue;

        // 참조
        public ProjectileMover projectile;  // 필수


        public bool ricocheted;
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