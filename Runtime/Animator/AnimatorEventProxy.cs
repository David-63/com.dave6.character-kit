using System;
using UnityEngine;

namespace Dave6.CharacterKit.AnimHandler
{
    public class AnimatorEventProxy : MonoBehaviour
    {
        public event Action<AnimationEvent> onAttackFinishEvent;
        public event Action<AnimationEvent> onAttackImpulseEvent;

        public void OnAttackFinish(AnimationEvent animationEvent)
        {
            onAttackFinishEvent?.Invoke(animationEvent);
        }
        public void OnAttackImpulse(AnimationEvent animationEvent)
        {
            onAttackImpulseEvent?.Invoke(animationEvent);
        }
    }
}
