using System;
using UnityEngine;

namespace Dave6.CharacterKit.AnimHandler
{
    [Serializable]
    public class IkTransforms
    {
        public Transform leftHand;
        public Transform rightHand;
        public Transform leftElbow;
        public Transform rightElbow;
    }
    public class AnimatorEventProxy : MonoBehaviour
    {
        Transform m_LeftHandIKTaret;
        Transform m_RightHandIKTaret;
        Transform m_LeftElbowIKTaret;
        Transform m_RightElbowIKTaret;
        public float m_HandIKAmount_L {get;set;}
        public float m_HandIKAmount_R {get;set;}
        public float m_ElbowIKAmount_L {get;set;}
        public float m_ElbowIKAmount_R {get;set;}

        Animator m_Animator;


        public event Action<AnimationEvent> onAttackFinishEvent;
        public event Action<AnimationEvent> onAttackImpulseEvent;
        public event Action<AnimationEvent> onReloadFinishEvent;


        void Awake()
        {
            m_Animator = GetComponent<Animator>();
        }

        public void OnAttackFinish(AnimationEvent animationEvent)
        {
            onAttackFinishEvent?.Invoke(animationEvent);
        }
        public void OnAttackImpulse(AnimationEvent animationEvent)
        {
            onAttackImpulseEvent?.Invoke(animationEvent);
        }
        public void OnReloadFinish(AnimationEvent animationEvent)
        {
            onReloadFinishEvent?.Invoke(animationEvent);
        }

        public void BindIk(IkTransforms ikTransforms)
        {
            if (ikTransforms == null)
            {
                m_LeftHandIKTaret = null;
                m_RightHandIKTaret = null;
                m_LeftElbowIKTaret = null;
                m_RightElbowIKTaret = null;
                return;
            }
            m_LeftHandIKTaret = ikTransforms.leftHand;
            m_RightHandIKTaret = ikTransforms.rightHand;
            m_LeftElbowIKTaret = ikTransforms.leftElbow;
            m_RightElbowIKTaret = ikTransforms.rightElbow;
        }
        public void SetHandIkAmountAll(float amount)
        {
            m_HandIKAmount_L = amount;
            m_HandIKAmount_R = amount;
            m_ElbowIKAmount_L = amount;
            m_ElbowIKAmount_R = amount;
        }
        public void SetLeftIkAmount(float amount)
        {
            m_HandIKAmount_L = amount;
            m_ElbowIKAmount_L = amount;
            
        }
        public void SetRightIkAmount(float amount)
        {
            m_HandIKAmount_R = amount;            
            m_ElbowIKAmount_R = amount;
        }


        void OnAnimatorIK(int layerIndex)
        {
            if (m_LeftHandIKTaret != null)
            {
                m_Animator.SetIKPosition(AvatarIKGoal.LeftHand, m_LeftHandIKTaret.position);
                m_Animator.SetIKRotation(AvatarIKGoal.LeftHand, m_LeftHandIKTaret.rotation);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, m_HandIKAmount_L);
                m_Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, m_HandIKAmount_L);
            }
            if (m_RightHandIKTaret != null)
            {
                m_Animator.SetIKPosition(AvatarIKGoal.RightHand, m_RightHandIKTaret.position);
                m_Animator.SetIKRotation(AvatarIKGoal.RightHand, m_RightHandIKTaret.rotation);
                m_Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, m_HandIKAmount_R);
                m_Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, m_HandIKAmount_R);
            }
            if (m_LeftElbowIKTaret != null)
            {
                m_Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, m_LeftElbowIKTaret.position);
                m_Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, m_ElbowIKAmount_L);
            }
            if (m_RightElbowIKTaret != null)
            {
                m_Animator.SetIKHintPosition(AvatarIKHint.RightElbow, m_RightElbowIKTaret.position);
                m_Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, m_ElbowIKAmount_R);
            }
        }
    }
}
