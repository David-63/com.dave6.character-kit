using Dave6.CharacterKit.AnimHandler;
using Dave6.CharacterKit.Item;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Dave6.CharacterKit.RigControl
{
    /// <summary>
    /// UpperBody Layer의 모든 내용을 캐싱하고 논리 로직을 처리함
    /// </summary>
    public class RigController : MonoBehaviour
    {
        PlayerCharacter m_PlayerCharacter;
        AnimatorEventProxy m_AnimProxy;
        Rig m_AimRig;
        IWeaponIkProvider m_IkItem;

        public float aimRigWeight;
        public float ikWeight;

        public bool isReloading;

        void Awake()
        {
            m_AimRig = GetComponentInChildren<Rig>();
        }

        public void RegisterRigController(PlayerCharacter playerCharacter, AnimatorEventProxy animProxy)
        {
            m_PlayerCharacter = playerCharacter;
            m_AnimProxy = animProxy;
        }

        void LateUpdate()
        {
            UpdateAimRig();
            UpdateHandIkWeight();
            UpdateWeaponPose();
        }
        void UpdateAimRig()
        {
            if (m_PlayerCharacter.focusInput)
            {
                SetAimRigWeight(1);
            }
            else
            {
                SetAimRigWeight(0);
            }
        }

        void UpdateHandIkWeight()
        {
            if (!m_PlayerCharacter.equipHandler.HasFirearm()
            || m_PlayerCharacter.combatHandler.reloading)
            {
                SetHandIkWeight(0);
                return;
            }

            if (m_PlayerCharacter.IsAim())
            {
                SetHandIkWeight(0.8f);
            }
            else
            {
                SetHandIkWeight(0);
            }
        }

        void UpdateWeaponPose()
        {
            if (!m_PlayerCharacter.equipHandler.HasFirearm()) return;
            
            if (m_PlayerCharacter.combatHandler.reloading)
            {
                SetWeaponPose(EWeaponPose.Hand);
                return;
            }

            if (m_PlayerCharacter.IsAim())
            {
                SetWeaponPose(EWeaponPose.Combat);
            }
            else
            {
                SetWeaponPose(EWeaponPose.Holster);
            }
        }

        #region IK API
        public void BindIK(IWeaponIkProvider activeItem)
        {
            m_IkItem = activeItem;
            if (activeItem == null)
            {
                m_AnimProxy.BindIk(null);
                return;
            }

            m_AnimProxy.BindIk(activeItem.ikTransforms);
        }
        public void SetWeaponPose(EWeaponPose targetPose)
        {
            if (m_IkItem == null) return;
            m_IkItem.SetWeaponPose(targetPose);
        }


        #endregion
        #region AmountFunc
        void SetAimRigWeight(float amount)
        {
            aimRigWeight = amount;
            m_AimRig.weight = aimRigWeight;
            //m_AimRig.weight = amount;
        }
        void SetHandIkWeight(float amount)
        {
            ikWeight = amount;
            m_AnimProxy.SetHandIkAmountAll(ikWeight);
            //m_AnimProxy.SetHandIkAmountAll(amount);
        }
        
        #endregion
    }
}