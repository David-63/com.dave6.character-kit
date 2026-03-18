using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    /// <summary>
    /// Mover 규칙
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerMoverConfig", menuName = "DaveAssets/Character/Mover/Player Mover Config")]
    public class PlayerMoverConfig : BaseMoverConfig
    {
        public float JumpDuration = 0.2f;
        [Header("회전")]
        [Tooltip("방향 회전에 걸리는 속도")]
        public float DirectionRotateSpeed = 20.0f;      // 이것도 안씀

    }
}