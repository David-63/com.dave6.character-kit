using Dave6.CharacterKit.AnimHandler;
using UnityEngine;
using UnityEngine.Animations;

namespace Dave6.CharacterKit.Item
{
    public enum EWeaponPose
    {
        Holster,
        Hand,
        Combat,
    }
    public class Firearm : MonoBehaviour, IActiveItem, IWeaponIkProvider
    {
        ParentConstraint m_ParentConstraint;
        public Transform actionSocket { get; set; }

        public EEquipSlotType slotContext {get; private set;}

        public int maxCapacity;
        public bool isReloading;

        [SerializeField] SocketProfile socketProfile;
        [SerializeField] FirearmProfile firearmProfile;

        [SerializeField] IkTransforms m_IkTransforms;
        public IkTransforms ikTransforms => m_IkTransforms;

        [SerializeField] RuntimeAnimatorController m_OverrideAnimator;
        public RuntimeAnimatorController overrideAnimator => m_OverrideAnimator;

        void Awake()
        {
            m_ParentConstraint = GetComponent<ParentConstraint>();
        }


        public void CancelAction()
        {
            
        }

        public bool CanPerformAction()
        {
            return false;
        }

        public void Attach(Transform socket)
        {
            actionSocket = socket;
            transform.localPosition = socketProfile.offset.offsetPos;
            transform.localRotation = Quaternion.Euler(socketProfile.offset.offsetRot);
        }

        public void Equip(EEquipSlotType slot)
        {
            slotContext = slot;
        }

        public void PerformAction()
        {
            
        }

        public void Unequip()
        {
            Destroy(gameObject);
        }

        public void BindWeaponPoseIK(Transform holster, Transform hand, Transform combat)
        {
            m_ParentConstraint.AddSource(new ConstraintSource { sourceTransform = holster, weight = 1f});
            m_ParentConstraint.AddSource(new ConstraintSource { sourceTransform = hand, weight = 0f});
            m_ParentConstraint.AddSource(new ConstraintSource { sourceTransform = combat, weight = 0f});

            m_ParentConstraint.SetTranslationOffset((int)EWeaponPose.Hand, socketProfile.handOffset.offsetPos);
            m_ParentConstraint.SetRotationOffset((int)EWeaponPose.Hand, socketProfile.handOffset.offsetRot);
            m_ParentConstraint.SetTranslationOffset((int)EWeaponPose.Combat, socketProfile.offset.offsetPos);
            m_ParentConstraint.SetRotationOffset((int)EWeaponPose.Combat, socketProfile.offset.offsetRot);
        }

        public void SetWeaponPose(EWeaponPose pose)
        {
            ConstraintSource holsterSource = m_ParentConstraint.GetSource((int)EWeaponPose.Holster);
            holsterSource.weight = pose == EWeaponPose.Holster ? 1f : 0f;
            m_ParentConstraint.SetSource((int)EWeaponPose.Holster, holsterSource);

            ConstraintSource handSource = m_ParentConstraint.GetSource((int)EWeaponPose.Hand);
            handSource.weight = pose == EWeaponPose.Hand ? 1f : 0f;
            m_ParentConstraint.SetSource((int)EWeaponPose.Hand, handSource);

            ConstraintSource combatSource = m_ParentConstraint.GetSource((int)EWeaponPose.Combat);
            combatSource.weight = pose == EWeaponPose.Combat ? 1f : 0f;
            m_ParentConstraint.SetSource((int)EWeaponPose.Combat, combatSource);

            m_ParentConstraint.SetTranslationOffset((int)EWeaponPose.Hand, socketProfile.handOffset.offsetPos);
            m_ParentConstraint.SetRotationOffset((int)EWeaponPose.Hand, socketProfile.handOffset.offsetRot);
            m_ParentConstraint.SetTranslationOffset((int)EWeaponPose.Combat, socketProfile.offset.offsetPos);
            m_ParentConstraint.SetRotationOffset((int)EWeaponPose.Combat, socketProfile.offset.offsetRot);
        }
    }
}
