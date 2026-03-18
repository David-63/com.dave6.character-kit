using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    public class BaseMoverConfig : ScriptableObject
    {
        public float MaxMoveSpeed = 5.5f;
        public float MaxFallSpeed = -14f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 2.0f;
        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 2.0f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float AirborneGravity = -15.0f;
        public float GroundGravity = -4.0f;
        public float TerminalVelocity = -53.0f; // 이것보다 빨리 떨어지면 안됨~
    }
}