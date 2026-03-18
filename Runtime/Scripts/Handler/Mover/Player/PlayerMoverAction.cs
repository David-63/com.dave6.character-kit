using System;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Mover
{
    public class PlayerMoverAction : BaseMoverAction
    {
        PlayerMoverConfig _Config => (PlayerMoverConfig)_BaseConfig;
        IMoveMode _MoveMode;
        public PlayerMoverAction(BaseMoverConfig config) : base(config) { }

        #region 세팅
        public void SetDirectionResolve(IMoveMode directionResolve)
        {
            _MoveMode = directionResolve;
        }
        #endregion

        public void ResolveIntent(PlayerMoverContext ctx)
        {
            ResolveJump(ctx);
        }

        void ResolveJump(PlayerMoverContext ctx)
        {
            if (ctx.WantJump && ctx.IsGrounded)
            {
                ctx.VerticalSpeed = Mathf.Sqrt(_Config.JumpHeight * -2f * _Config.AirborneGravity);
                ctx.WantJump = false;
            }
        }

        internal void ResolveFacing(PlayerMoverContext ctx, in MoverFrameInput inputValue)
        {
            ctx.FacingTargetYaw = _MoveMode.ResolveFacing(ctx, inputValue);
        }

        internal void UpdateFacing(PlayerMoverContext ctx, float deltaTime)
        {
            float yawSpeed = _MoveMode.GetYawSpeed(ctx);
            ctx.FacingCurYaw = Mathf.MoveTowardsAngle(ctx.FacingCurYaw, ctx.FacingTargetYaw, yawSpeed * deltaTime);
        }

        internal void ResolveMoveDirection(PlayerMoverContext ctx, in MoverFrameInput inputValue)
        {
            if (!ctx.IsGrounded) return;

            Vector3 rawDir = _MoveMode.ResolveMoveDirection(ctx, inputValue);
            ctx.MoveDirection = Vector3.SmoothDamp(ctx.MoveDirection, rawDir, ref ctx.MoveDirVelocity, 0.08f);
        }
    }
}