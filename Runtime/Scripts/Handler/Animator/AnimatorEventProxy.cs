using System;
using UnityEngine;

namespace Dave6.CharacterKit.AnimHandler
{
    [Serializable]
    public class IkTransforms
    {
        public Transform LeftHand;
        public Transform RightHand;
        public Transform LeftElbow;
        public Transform RightElbow;
    }
    public class AnimatorEventProxy : MonoBehaviour
    {
        Transform _LeftHandIKTaret;
        Transform _RightHandIKTaret;
        Transform _LeftElbowIKTaret;
        Transform _RightElbowIKTaret;
        public float HandIKAmount_L {get;set;}
        public float HandIKAmount_R {get;set;}
        public float ElbowIKAmount_L {get;set;}
        public float ElbowIKAmount_R {get;set;}

        Animator _Animator;

        public event Action<AnimationEvent> OnAttackFinishEvent;
        public event Action<AnimationEvent> OnAttackImpulseEvent;
        public event Action<AnimationEvent> OnReloadFinishEvent;

        void Awake()
        {
            _Animator = GetComponent<Animator>();
        }

        public void OnAttackFinish(AnimationEvent animationEvent)
        {
            OnAttackFinishEvent?.Invoke(animationEvent);
        }
        public void OnAttackImpulse(AnimationEvent animationEvent)
        {
            OnAttackImpulseEvent?.Invoke(animationEvent);
        }
        public void OnReloadFinish(AnimationEvent animationEvent)
        {
            OnReloadFinishEvent?.Invoke(animationEvent);
        }

        public void BindIk(IkTransforms ikTransforms)
        {
            if (ikTransforms == null)
            {
                _LeftHandIKTaret = null;
                _RightHandIKTaret = null;
                _LeftElbowIKTaret = null;
                _RightElbowIKTaret = null;
                return;
            }
            _LeftHandIKTaret = ikTransforms.LeftHand;
            _RightHandIKTaret = ikTransforms.RightHand;
            _LeftElbowIKTaret = ikTransforms.LeftElbow;
            _RightElbowIKTaret = ikTransforms.RightElbow;
        }
        public void SetHandIkAmountAll(float amount)
        {
            HandIKAmount_L = amount;
            HandIKAmount_R = amount;
            ElbowIKAmount_L = amount;
            ElbowIKAmount_R = amount;
        }
        public void SetLeftIkAmount(float amount)
        {
            HandIKAmount_L = amount;
            ElbowIKAmount_L = amount;
            
        }
        public void SetRightIkAmount(float amount)
        {
            HandIKAmount_R = amount;            
            ElbowIKAmount_R = amount;
        }


        void OnAnimatorIK(int layerIndex)
        {
            if (_LeftHandIKTaret != null)
            {
                _Animator.SetIKPosition(AvatarIKGoal.LeftHand, _LeftHandIKTaret.position);
                _Animator.SetIKRotation(AvatarIKGoal.LeftHand, _LeftHandIKTaret.rotation);
                _Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, HandIKAmount_L);
                _Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, HandIKAmount_L);
            }
            if (_RightHandIKTaret != null)
            {
                _Animator.SetIKPosition(AvatarIKGoal.RightHand, _RightHandIKTaret.position);
                _Animator.SetIKRotation(AvatarIKGoal.RightHand, _RightHandIKTaret.rotation);
                _Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, HandIKAmount_R);
                _Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, HandIKAmount_R);
            }
            if (_LeftElbowIKTaret != null)
            {
                _Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, _LeftElbowIKTaret.position);
                _Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, ElbowIKAmount_L);
            }
            if (_RightElbowIKTaret != null)
            {
                _Animator.SetIKHintPosition(AvatarIKHint.RightElbow, _RightElbowIKTaret.position);
                _Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, ElbowIKAmount_R);
            }
        }
    }
}
