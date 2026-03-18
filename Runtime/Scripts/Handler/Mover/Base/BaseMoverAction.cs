using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    /// <summary>
    /// Pure Logic
    /// </summary>
    public class BaseMoverAction
    {
        protected readonly BaseMoverConfig _BaseConfig;
        public BaseMoverAction(BaseMoverConfig config) { _BaseConfig = config; }

        public void CalculateSpeed(BaseMoverContext ctx, BaseMoverConfig config, float deltaTime)
        {
            float speedOffset = 0.1f;

            if (ctx.IsGrounded)
            {
                if (Mathf.Abs(ctx.BaseSpeed - ctx.TargetSpeed) > speedOffset)
                {
                    ctx.BaseSpeed = Mathf.Lerp(ctx.BaseSpeed, ctx.TargetSpeed, deltaTime * config.SpeedChangeRate);
                    ctx.BaseSpeed = Mathf.Round(ctx.BaseSpeed * 1000f) / 1000f;
                }
                else
                {
                    ctx.BaseSpeed = ctx.TargetSpeed;
                }
            }
            else
            {
                float airPenalty = 0.5f;
                ctx.BaseSpeed = Mathf.Lerp(ctx.BaseSpeed, 0, deltaTime * airPenalty);
                ctx.BaseSpeed = Mathf.Round(ctx.BaseSpeed * 1000f) / 1000f;
            }

            if (Mathf.Abs(ctx.ImpulseSpeed) > speedOffset)
            {
                ctx.ImpulseSpeed = Mathf.Lerp(ctx.ImpulseSpeed, 0, deltaTime * config.SpeedChangeRate);
            }
            else
            {
                ctx.ImpulseSpeed = 0;
            }

            ctx.HorizontalSpeed = ctx.BaseSpeed + ctx.ImpulseSpeed;
        }
        public Vector3 GetVelocity(BaseMoverContext ctx)
        {
            return ctx.MoveDirection * ctx.HorizontalSpeed + Vector3.up * ctx.VerticalSpeed;
        }
        public void CalculateGravity(BaseMoverContext ctx, BaseMoverConfig config, float deltaTime)
        {
            if (ctx.IsGrounded)
            {
                // stop our velocity dropping infinitely when grounded
                if (ctx.VerticalSpeed < 0f)
                {
                    ctx.VerticalSpeed = -4f;
                }
            }
            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (ctx.VerticalSpeed > config.TerminalVelocity)
            {
                ctx.VerticalSpeed += config.AirborneGravity * deltaTime;
            }
        }

        #region 회전
        public float CalculateMoveDirectionYaw(BaseMoverContext ctx)
        {
            return Mathf.Atan2(ctx.MoveDirection.x, ctx.MoveDirection.z) * Mathf.Rad2Deg;
        }
        #endregion

    }
}