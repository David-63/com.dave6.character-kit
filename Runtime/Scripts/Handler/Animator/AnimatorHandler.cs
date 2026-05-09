using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Dave6.CharacterKit.AnimHandler
{
    /// <summary>
    /// 애니메이터에 값을 넣어주고
    /// controller를 통해 애니메이션 관련 기능을 제공
    /// </summary>
    public class AnimatorHandler : MonoBehaviour
    {
        Animator _Animator;
        AnimatorEventProxy _AnimatorEventProxy;
        string _CurrentAnimation = "";
        bool _PrevHasMoveInput;

        public bool UseFocus {get; private set;}

        public event Action OnAttackImpulseAction;
        public event Action OnAttackFinishedAction;
        public event Action OnReloadFinishedAction;

        #region 초기화
        public void RegisterAnimator(Animator animator, AnimatorEventProxy animProxy)
        {
            _Animator = animator;
            _AnimatorEventProxy = animProxy;

            _AnimatorEventProxy.OnAttackFinishEvent += OnAttackAnimationEnd;
            _AnimatorEventProxy.OnReloadFinishEvent += OnReloadAnimationEnd;
        }

        public void ResetAnimHandler()
        {
            _Animator.SetFloat("moveSpeed", 0);
            _Animator.SetFloat("lastMoveSpeed", 0);
            _Animator.SetFloat("verticalSpeed", 0);
            _Animator.SetFloat("lastVerticalSpeed", 0);
            _Animator.SetBool("isGrounded", true);
            _Animator.SetBool("hasMoveInput", false);
            _Animator.SetBool("useFocus", false);
            _Animator.SetBool("useShift", false);
            _Animator.SetBool("isAim", false);
            _Animator.SetFloat("directionX", 0);
            _Animator.SetFloat("directionY", 0);
        }
        #endregion

        public void UpdateMoveInput(float speed, bool hasMoveInput)
        {
            _Animator.SetFloat("moveSpeed", speed);
            _Animator.SetBool("hasMoveInput", hasMoveInput);
        }
        public void EvaluateMoveInputTransition(float speed, bool hasMoveInput)
        {
            _Animator.SetFloat("moveSpeed", speed);
            _Animator.SetBool("hasMoveInput", hasMoveInput);

            if (_PrevHasMoveInput && !hasMoveInput)
            {
                OnMoveInputReleased(speed);
            }

            _PrevHasMoveInput = hasMoveInput;
        }
        public void OnMoveInputReleased(float speed)
        {
            _Animator.SetFloat("lastMoveSpeed", speed);
        }




        public void UpdateUseStrafe(bool useFocus)
        {
            this.UseFocus = useFocus;
            _Animator.SetBool("useFocus", useFocus);
        }
        public void UpdateUseShift(bool useShift)
        {
            _Animator.SetBool("useShift", useShift);
        }
        public void UpdateIsAim(bool isAim)
        {
            _Animator.SetBool("isAim", isAim);
        }
        public void UpdateDirection(Vector3 direction)
        {
            _Animator.SetFloat("directionX", direction.x);
            _Animator.SetFloat("directionY", direction.y);
        }

        /// <summary>
        /// 1. 액션 토큰 필요함
        /// 고유 ID를 발급하고 wait이 끝났을때 id가 유효한지 체크
        /// </summary>
        /// <param name="animation">키 값</param>
        /// <param name="corssfade">전환 시간</param>
        public void ChangeAnimation(string animation, bool allowSameAnim = true)
        {
            float corssfade = 0.2f;
            if (!allowSameAnim)
            {
                if (_CurrentAnimation == animation) return;
            }
            _CurrentAnimation = animation;
            _Animator.CrossFade(animation, corssfade);
        }
        public void ClearCurrentAnimation()
        {
            _CurrentAnimation = "";
        }

        #region 상태 제어용 애니메이션 세팅

        #endregion

        #region 애니메이션 이벤트 콜백
        public void OnAttackImpulse(AnimationEvent animationEvent)
        {
            OnAttackImpulseAction?.Invoke();
        }
        public void OnAttackAnimationEnd(AnimationEvent animationEvent)
        {
            OnAttackFinishedAction?.Invoke();
            ClearCurrentAnimation();
        }
        public void OnReloadAnimationEnd(AnimationEvent animationEvent)
        {
            OnReloadFinishedAction?.Invoke();
            ClearCurrentAnimation();
        }
        #endregion

        // public void BindAnimator(IWeaponIkProvider nextWeaponIK)
        // {
        //     if (nextWeaponIK.overrideAnimator != null)
        //     {
        //         _Animator.runtimeAnimatorController = nextWeaponIK.overrideAnimator;
        //     }
        // }
        public void UpdateMoveSpeed(float speed)
        {
            _Animator.SetFloat("moveSpeed", speed);
        }
        public void UpdateVerticalSpeed(float speed)
        {
            _Animator.SetFloat("verticalSpeed", speed);
        }
        public void UpdateGrounded(bool isGrounded, float speed)
        {
            _Animator.SetBool("isGrounded", isGrounded);
            _Animator.SetFloat("verticalSpeed", speed);
        }
        public void UpdateLandVerticalSpeed(float speed)
        {
            _Animator.SetFloat("lastVerticalSpeed", speed);
        }


    }

    public class AnimContext
    {
        public string m_CurrentAnimation = "";
        public bool m_PrevGrounded;
        public bool m_PrevHasMoveInput;
    }
}
