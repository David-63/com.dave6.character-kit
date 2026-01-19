using Dave6.CharacterKit.AnimHandler;
using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    public interface IWeaponIkProvider
    {
        IkTransforms ikTransforms { get; }
        RuntimeAnimatorController overrideAnimator { get; }

        void BindWeaponPoseIK(Transform holster, Transform hand, Transform combat);
        void SetWeaponPose(EWeaponPose pose);
    }
}
