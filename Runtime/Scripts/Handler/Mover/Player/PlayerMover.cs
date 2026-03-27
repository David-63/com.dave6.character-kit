using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.Handler.Mover
{
    public class PlayerMover : BaseMover
    {
        // 상수
        PlayerMoverConfig _Config => (PlayerMoverConfig)_BaseConfig;
        // 내부에서만 쓰는 변수
        PlayerMoverContext _Context => (PlayerMoverContext)_BaseContext;
        // 퓨어 로직
        PlayerMoverAction _Action => (PlayerMoverAction)_BaseAction;

        Countdown _JumpTimer;

        #region 외부 공개 필드
        public float MaxSpeed => _Config.MaxMoveSpeed;
        public float VerticalSpeed => _Context.VerticalSpeed;
        public float GetMoveSpeed01() => _Context.HorizontalSpeed / _Config.MaxMoveSpeed;

        #endregion

        void Awake()
        {
            // 기초 초기화
            EnsureSetup();
            RecalculateColliderDimensions();

            // 다른 도메인 활성화
            _BaseContext = new PlayerMoverContext();
            _BaseAction = new PlayerMoverAction(_Config);

            // 점프 타이머 생성
            if (_JumpTimer == null)
            {
                _JumpTimer = new Countdown(_Config.JumpDuration);
            }
            // 카메라는 주입받거나, GetComponent로 가져오기
        }

        public void OnUpdate(in MoverFrameInput inputValue)
        {
            ApplyGravity();                             // 
            CheckForGround();                           // 

            // 동작 체크
            _Action.ResolveIntent(_Context);

            _Action.ResolveFacing(_Context, in inputValue);
            _Action.UpdateFacing(_Context, inputValue.DeltaTime);

            _Action.ResolveMoveDirection(_Context, inputValue);

            UpdateFinalSpeed();                         // 수평 속력 갱신
            ApplyRotation();                            // 회전 적용

            ApplyMovement();                            // 최종 적용
        }

        void ApplyRotation()
        {
            transform.rotation = Quaternion.Euler(0, _Context.FacingCurYaw, 0);
        }

        #region Mover API

        public bool TryJump()
        {
            if (!_JumpTimer.IsFinished) return false;

            _JumpTimer.RestartTimer();
            _Context.WantJump = true;

            return true;
        }

        public void SetTargetSpeed(float speed) => _Context.TargetSpeed = speed;

        // 방향
        public void SetMoveInput(Vector2 input) => _Context.MoveInput = input;

        // 회전
        public void SetMoveDirectionPolicy(IMoveMode directionResolve)
        {
            _Action.SetDirectionResolve(directionResolve);
        }

        #endregion
    }
}