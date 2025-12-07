using UnityEngine;


namespace Dave6.CharacterKit.Movement
{
    /// <summary>
    /// Represents a movement profile for a player.
    /// </summary>
    [CreateAssetMenu(fileName = "MovementProfile", menuName = "DaveAssets/Profile/MovementProfile")]
    public class MovementProfile : ScriptableObject
    {
        [Header("Player")]
        [Tooltip("Jog speed of the character in m/s")]
        public float JogSpeed = 3.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.5f;
        [Tooltip("Strafe speed of the character in m/s")]
        public float StrafeSpeed = 2.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 5.0f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.1f, 1.5f)]
        public float RotationSmoothTime = 0.12f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float AirborneGravity = -15.0f;
        public float GroundGravity = -4.0f;
        public float TerminalVelocity = 53.0f; // 중력 가속 제한속도
    }
}