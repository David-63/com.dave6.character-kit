using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    public class InputBasedMove : IMoveMode
    {
        public float GetYawLerpFactor(PlayerMoverContext ctx, float deltaTime)
        {
            float moreRotate = 10f;
            return deltaTime * moreRotate;
        }

        // target cur yaw 값 비교해서 전환속도 차이 주면 될듯?
        public float GetYawSpeed(PlayerMoverContext ctx)
        {
            float yawDelta = Mathf.Abs(Mathf.DeltaAngle(ctx.FacingCurYaw, ctx.FacingTargetYaw));
            float t = Mathf.Clamp01(yawDelta / 180f);

            float minSpeed = 270f;
            float maxSpeed = 360f;

            return Mathf.Lerp(minSpeed, maxSpeed, t);
        }

        public float ResolveFacing(PlayerMoverContext ctx, in MoverFrameInput inputValue)
        {
            if (ctx.MoveInput.sqrMagnitude < 0.001f) return ctx.FacingCurYaw;

            return Mathf.Atan2(ctx.MoveInput.x, ctx.MoveInput.y) * Mathf.Rad2Deg + inputValue.ReferenceYaw;
        }

        public Vector3 ResolveMoveDirection(PlayerMoverContext ctx, in MoverFrameInput inputValue)
        {
            if (ctx.MoveInput.sqrMagnitude > 0.001f)
            {
                float rad = ctx.FacingCurYaw * Mathf.Deg2Rad;
                ctx.LastMoveDirection = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)).normalized;
            }

            return ctx.LastMoveDirection;
        }
    }
}