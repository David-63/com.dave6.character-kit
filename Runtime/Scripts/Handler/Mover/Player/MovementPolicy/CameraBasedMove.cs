using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    public class CameraBasedMove : IMoveMode
    {
        public float GetYawLerpFactor(PlayerMoverContext ctx, float deltaTime)
        {
            float moreRotate = 5f;
            return deltaTime * moreRotate;
        }
        public float GetYawSpeed(PlayerMoverContext ctx)
        {
            float yawDelta = Mathf.Abs(Mathf.DeltaAngle(ctx.FacingCurYaw, ctx.FacingTargetYaw));
            float t = Mathf.Clamp01(yawDelta / 180f);

            float baseSpeed = 270f;
            float slowFactor = 0.8f;
            return Mathf.Lerp(baseSpeed, baseSpeed * slowFactor, t);
        }

        public float ResolveFacing(PlayerMoverContext ctx, in MoverFrameInput inputValue)
        {
            return inputValue.ReferenceYaw;
        }

        public Vector3 ResolveMoveDirection(PlayerMoverContext ctx, in MoverFrameInput inputValue)
        {
            Vector3 cameraForward = inputValue.CameraForward;
            cameraForward.y = 0f;
            cameraForward.Normalize();

            if (cameraForward.sqrMagnitude > 0.0001f)
            {
                Vector3 cameraRight = Vector3.Cross(Vector3.up, cameraForward);
                ctx.LastMoveDirection = cameraForward * ctx.MoveInput.y + cameraRight * ctx.MoveInput.x;
            }

            return ctx.LastMoveDirection;
        }

    }
}