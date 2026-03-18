using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    public interface IMoveMode
    {
        float GetYawLerpFactor(PlayerMoverContext ctx, float deltaTime);
        float GetYawSpeed(PlayerMoverContext ctx);

        float ResolveFacing(PlayerMoverContext ctx, in MoverFrameInput inputValue);
        Vector3 ResolveMoveDirection(PlayerMoverContext ctx, in MoverFrameInput inputValue);
    }
}