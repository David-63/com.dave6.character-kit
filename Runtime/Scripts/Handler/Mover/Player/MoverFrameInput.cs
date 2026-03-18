using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    public readonly struct MoverFrameInput
    {
        public readonly float DeltaTime;
        public readonly float ReferenceYaw;
        public readonly Vector3 CameraForward;

        public MoverFrameInput(float deltaTime, float referenceYaw, Vector3 cameraForward)
        {
            DeltaTime = deltaTime;
            ReferenceYaw = referenceYaw;
            CameraForward = cameraForward;
        }
    }
}