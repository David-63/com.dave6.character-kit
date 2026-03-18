using Dave6.CharacterKit.Handler.Mover;
using Dave6.Foundation.GameLogic.State;
using Dave6.ThirdPersonCamera;
using UnityEngine;

namespace Dave6.CharacterKit.Player.States
{
    public class StrafeState : BaseState<PlayerController>
    {
        Vector2 _CurrentMoveDirection; // freelook <-> strafe 애니메이션이 매끄럽지 않다면 상위 ctx 변수로 승격시키기

        ThirdPersonPreset StrafePreset;
        bool _PrevGrounded;
        bool _Landing;
        float _FallSpeed01;
        public StrafeState(PlayerController controller) : base(controller)
        {
            StrafePreset.fov = 70f;
            StrafePreset.sideLength = 0.85f;
            StrafePreset.distance = 1f;
        }

        public override void OnEnter()
        {
            _Controller.Mover.SetMoveDirectionPolicy(new CameraBasedMove());
            _Controller.CameraSystem.StartTransition(StrafePreset);
            _Controller.AnimHandler.UpdateUseStrafe(true);
        }
        public override void OnExit()
        {
            _Controller.AnimHandler.UpdateUseStrafe(false);
        }

        public override void Update()
        {
            HandleJump();
            HandleMovement();
            HandleDirection();
            HandleGroundCheck();
        }
        void HandleJump()
        {
            if (_Controller.InputCtx.jump)
            {
                _Controller.Mover.TryJump();
            }
        }
        void HandleMovement()
        {
            _Controller.Mover.SetMoveInput(_Controller.InputCtx.move);

            float applySpeed = _Controller.InputCtx.HasMoveInput() ? _Controller.Mover.MaxSpeed * 0.4f : 0f;

            if (_Landing)
            {
                applySpeed = 0f;
                _FallSpeed01 -= Time.deltaTime;
                if (_FallSpeed01 <= 0f)
                {
                    _Landing = false;
                    _FallSpeed01 = 0f;
                }
            }
            _Controller.Mover.SetTargetSpeed(applySpeed);
            _Controller.AnimHandler.UpdateMoveSpeed(_Controller.Mover.GetMoveSpeed01());
        }
        void HandleDirection()
        {
            var targetDir = _Controller.InputCtx.move.normalized;
            float Responsiveness = 10f; // 반응 속도, 12~25 사이 취향
            float lerpFactor = 1f - Mathf.Exp(-Responsiveness * Time.deltaTime);
            float threshold = 0.001f;

            _CurrentMoveDirection = Vector2.Lerp(_CurrentMoveDirection, targetDir, lerpFactor);
            if ((_CurrentMoveDirection - targetDir).sqrMagnitude < threshold)
            {
                _CurrentMoveDirection = targetDir;
            }

            _Controller.AnimHandler.UpdateDirection(_CurrentMoveDirection);
        }
        void HandleGroundCheck()
        {
            var grounded = _Controller.Mover.IsGrounded;
            if (grounded != _PrevGrounded)
            {
                _Controller.AnimHandler.UpdateLandVerticalSpeed(_Controller.Mover.VerticalSpeed);
                if (grounded)
                {
                    _Landing = true;
                    float fallIntensity = Mathf.InverseLerp(-4f, -14f, _Controller.Mover.VerticalSpeed);
                    _FallSpeed01 = Mathf.Lerp(0.1f, 1f, fallIntensity);
                }
            }
            _Controller.AnimHandler.UpdateGrounded(_Controller.Mover.IsGrounded, _Controller.Mover.VerticalSpeed);
            _PrevGrounded = grounded;
        }
    }
}