using System;
using UnityEngine;

namespace Dave6.CharacterKit.AnimHandler
{
    /// <summary>
    /// 애니메이터에 값을 넣어주고
    /// controller를 통해 애니메이션 관련 기능을 제공
    /// </summary>
    public class AnimatorHandler
    {
        Animator m_Animator;
        BasicPlayerController m_Controller;
        string m_CurrentAnimation = "";
        public bool attackReady = true;


        public AnimatorHandler(BasicPlayerController controller, Animator animator)
        {
            m_Animator = animator;
            m_Controller = controller;
        }

        public void ResetAnimHandler()
        {
            m_Animator.SetFloat("moveSpeed", 0);
            m_Animator.SetFloat("lastMoveSpeed", 0);
            m_Animator.SetFloat("verticalSpeed", 0);
            m_Animator.SetFloat("lastVerticalSpeed", 0);
            m_Animator.SetBool("isGrounded", true);
            m_Animator.SetBool("hasMoveInput", false);
            m_Animator.SetBool("useStrafe", false);
            m_Animator.SetBool("useShift", false);
            m_Animator.SetFloat("directionX", 0);
            m_Animator.SetFloat("directionY", 0);
        }

        public void UpdateMoveSpeed()
        {
            m_Animator.SetFloat("moveSpeed", m_Controller.horizontalSpeed);
        }
        public void UpdateLastMoveSpeed()
        {
            m_Animator.SetFloat("lastMoveSpeed", m_Controller.horizontalSpeed);
        }
        public void UpdateVerticalSpeed()
        {
            m_Animator.SetFloat("verticalSpeed", m_Controller.verticalSpeed);
        }
        public void UpdateLastVerticalSpeed()
        {
            m_Animator.SetFloat("lastVerticalSpeed", m_Controller.verticalSpeed);
        }
        public void UpdateGrounded()
        {
            m_Animator.SetBool("isGrounded", m_Controller.mover.isGrounded);
        }
        public void UpdateHasMovementInput()
        {
            m_Animator.SetBool("hasMoveInput", m_Controller.HasMovementInput());
        }


        /// <summary>
        /// 1. 액션 토큰 필요함
        /// 고유 ID를 발급하고 wait이 끝났을때 id가 유효한지 체크
        /// 
        /// </summary>
        /// <param name="animation"></param>
        /// <param name="corssfade"></param>
        /// <param name="duration"></param>

        public void ChangeAnimation(string animation, float corssfade = 0.2f)
        {
            if (m_CurrentAnimation == animation) return;
            m_CurrentAnimation = animation;
            m_Animator.CrossFade(animation, corssfade);
        }
        public void ClearCurrentAnimation()
        {
            m_CurrentAnimation = "";
        }
        public void OnAttackAnimationEnd(AnimationEvent animationEvent)
        {
            attackReady = true;
            m_Controller.attacking = false;
            ClearCurrentAnimation();
        }
        public void OnAttackImpulse(AnimationEvent animationEvent)
        {
            var playerController = m_Controller as PlayerController;
            playerController.combatHandler.AddAttackImpulse();
        }
        public void UpdateUseStrafe(bool useStrafe)
        {
            m_Animator.SetBool("useStrafe", useStrafe);
        }
        public void UpdateUseShift(bool useShift)
        {
            m_Animator.SetBool("useShift", useShift);
        }
        public void UpdateDirectionX(float x)
        {
            m_Animator.SetFloat("directionX", x);
        }

        public void UpdateDirectionY(float z)
        {
            m_Animator.SetFloat("directionY", z);
        }

    }
}
