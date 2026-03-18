using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    /// <summary>
    /// Mover 상태
    /// </summary>
    public class PlayerMoverContext : BaseMoverContext
    {
        // 플래그
        public bool WantJump;
        public bool WantStop;

        // 회전 값
        public float FacingTargetYaw;                             // moveMode가 결정한 바라보는 목표 yaw
        public float FacingCurYaw;                                // 보간된 실제 yaw
        public float FacingCurPitch;

        public float LastTargetYaw;                         // 회전 고정용 스냅샷


        // 캐싱
        public Vector3 MoveDirVelocity;                       // smoothDamp에 사용

        public Quaternion CharacterAim; // 아직 안씀
    }
}