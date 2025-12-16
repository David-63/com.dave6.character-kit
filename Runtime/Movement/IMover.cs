using UnityEngine;

namespace Dave6.CharacterKit.Movement
{
    public interface IMover
    {
        MovementProfile GetMovementProfile();

        void SetStrafeMode(bool shift);
        void SetFreeLookMode();
    }
}
