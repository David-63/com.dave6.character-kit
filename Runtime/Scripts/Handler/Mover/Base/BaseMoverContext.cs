using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    /// <summary>
    /// Plain class
    /// 씬/유니티 연관 없이 존재
    /// </summary>
    public class BaseMoverContext
    {
        // 상태
        public bool IsGrounded;

        // 속력
        public float BaseSpeed;
        public float TargetSpeed;
        public float ImpulseSpeed;

        // 속도 성분
        public float HorizontalSpeed;
        public float VerticalSpeed;

        // 방향
        public Vector2 MoveInput;           // 로컬 입력 값
        public Vector3 MoveDirection;       // 월드 이동 방향 (회전과 독립함)
        public Vector3 LastMoveDirection;   // 월드 이동 방향 (회전과 독립함)
    }
}