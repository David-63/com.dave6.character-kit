using UnityEngine;

namespace Dave6.CharacterKit.Movement
{
    public interface IMover
    {
        void CalculateSpeed(float deltaTime);
        MovementProfile GetMovementProfile();

        void SetStrafeMode(bool shift);
        void SetFreeLookMode();

        void FreeLookRotate(float deltaTime);
        void StrafeMoveRotate(float deltaTime);
    }
}
