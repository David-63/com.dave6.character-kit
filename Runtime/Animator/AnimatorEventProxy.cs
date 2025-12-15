using System;
using UnityEngine;

namespace Dave6.CharacterKit.AnimHandler
{
    public class AnimatorEventProxy : MonoBehaviour
    {
        public event Action<AnimationEvent> onAttackFinishEvent;

        public void OnAttackFinish(AnimationEvent animationEvent)
        {
            onAttackFinishEvent?.Invoke(animationEvent);
        }
    }
}
