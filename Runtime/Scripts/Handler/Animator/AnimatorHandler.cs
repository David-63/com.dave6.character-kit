using System;
using Dave6.CharacterKit.Item;
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
        Animator m_Animator;
        AnimatorEventProxy m_AnimatorEventProxy;
        string m_CurrentAnimation = "";

        bool m_PrevGrounded;
        bool m_PrevHasMoveInput;

        public bool useFocus {get; private set;}

        public event Action onAttackImpulse;
        public event Action onAttackFinished;
        public event Action onReloadFinished;

        #region 초기화
        public void RegisterAnimator(Animator animator, AnimatorEventProxy animProxy)
        {
            m_Animator = animator;
            m_AnimatorEventProxy = animProxy;
        }

        public void ResetAnimHandler()
        {
            m_Animator.SetFloat("moveSpeed", 0);
            m_Animator.SetFloat("lastMoveSpeed", 0);
            m_Animator.SetFloat("verticalSpeed", 0);
            m_Animator.SetFloat("lastVerticalSpeed", 0);
            m_Animator.SetBool("isGrounded", true);
            m_Animator.SetBool("hasMoveInput", false);
            m_Animator.SetBool("useFocus", false);
            m_Animator.SetBool("useShift", false);
            m_Animator.SetBool("isAim", false);
            m_Animator.SetFloat("directionX", 0);
            m_Animator.SetFloat("directionY", 0);
        }
        #endregion

        public void UpdateMoveInput(float speed, bool hasMoveInput)
        {
            m_Animator.SetFloat("moveSpeed", speed);
            m_Animator.SetBool("hasMoveInput", hasMoveInput);
        }
        public void EvaluateMoveInputTransition(float speed, bool hasMoveInput)
        {
            m_Animator.SetFloat("moveSpeed", speed);
            m_Animator.SetBool("hasMoveInput", hasMoveInput);

            if (m_PrevHasMoveInput && !hasMoveInput)
            {
                OnMoveInputReleased(speed);
            }

            m_PrevHasMoveInput = hasMoveInput;
        }
        public void OnMoveInputReleased(float speed)
        {
            m_Animator.SetFloat("lastMoveSpeed", speed);
        }

        public void UpdateVerticalSpeed(float speed)
        {
            m_Animator.SetFloat("verticalSpeed", speed);
        }

        public void UpdateGrounded(bool isGrounded, float speed)
        {
            m_Animator.SetBool("isGrounded", isGrounded);

            if (!m_PrevGrounded && isGrounded)
            {
                m_Animator.SetFloat("lastVerticalSpeed", speed);
            }
            m_PrevGrounded = isGrounded;
        }
        public void UpdateUseStrafe(bool useFocus)
        {
            this.useFocus = useFocus;
            m_Animator.SetBool("useFocus", useFocus);
        }
        public void UpdateUseShift(bool useShift)
        {
            m_Animator.SetBool("useShift", useShift);
        }
        public void UpdateIsAim(bool isAim)
        {
            m_Animator.SetBool("isAim", isAim);
        }
        public void UpdateDirection(Vector3 direction)
        {
            m_Animator.SetFloat("directionX", direction.x);
            m_Animator.SetFloat("directionY", direction.z);
        }


        /// <summary>
        /// 1. 액션 토큰 필요함
        /// 고유 ID를 발급하고 wait이 끝났을때 id가 유효한지 체크
        /// </summary>
        /// <param name="animation">키 값</param>
        /// <param name="corssfade">전환 시간</param>
        public void ChangeAnimation(string animation, float corssfade = 0.2f, bool allowSameAnim = false)
        {
            if (!allowSameAnim)
            {
                if (m_CurrentAnimation == animation) return;
            }
            m_CurrentAnimation = animation;
            m_Animator.CrossFade(animation, corssfade);
        }
        public void ClearCurrentAnimation()
        {
            m_CurrentAnimation = "";
        }

        #region 상태 제어용 애니메이션 세팅

        #endregion

        #region 애니메이션 이벤트 콜백
        public void OnAttackImpulse(AnimationEvent animationEvent)
        {
            onAttackImpulse?.Invoke();
        }
        public void OnAttackAnimationEnd(AnimationEvent animationEvent)
        {
            onAttackFinished?.Invoke();
            ClearCurrentAnimation();
        }
        public void OnReloadAnimationEnd(AnimationEvent animationEvent)
        {
            onReloadFinished?.Invoke();
            ClearCurrentAnimation();
        }
        #endregion

        public void BindAnimator(IWeaponIkProvider nextWeaponIK)
        {
            if (nextWeaponIK.overrideAnimator != null)
            {
                m_Animator.runtimeAnimatorController = nextWeaponIK.overrideAnimator;
            }
        }

    }
}
