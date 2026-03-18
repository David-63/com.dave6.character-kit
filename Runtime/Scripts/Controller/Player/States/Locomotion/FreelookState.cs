using Dave6.CharacterKit.Handler.Mover;
using Dave6.Foundation.GameLogic.State;
using Dave6.ThirdPersonCamera;
using UnityEngine;

namespace Dave6.CharacterKit.Player.States
{
    public class FreelookState : BaseState<PlayerController>
    {
        ThirdPersonPreset _FreelookPreset;
        bool _PrevGrounded;
        bool _Landing;
        float _FallSpeed01;

        public FreelookState(PlayerController controller) : base(controller)
        {
            _FreelookPreset.fov = 50f;
            _FreelookPreset.sideLength = 0.65f;
            _FreelookPreset.distance = 4f;
        }

        public override void OnEnter()
        {
            _Controller.CameraSystem.SetSway(0.45f, 2.0f);
            _Controller.Mover.SetMoveDirectionPolicy(new InputBasedMove());
            _Controller.CameraSystem.StartTransition(_FreelookPreset);
        }

        public override void Update()
        {
            HandleJump();
            HandleMovement();
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

            float targetSpeed = _Controller.InputCtx.shift ? _Controller.Mover.MaxSpeed : _Controller.Mover.MaxSpeed * 0.4f;
            float applySpeed = _Controller.InputCtx.HasMoveInput() ? targetSpeed : 0f;

            // 속도에 따라 조절
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