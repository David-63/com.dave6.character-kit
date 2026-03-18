using System.Collections.Generic;
using Dave6.CharacterKit.AnimHandler;
using Dave6.ObjectPoolingSystem;
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
    public class Firearm : MonoBehaviour, IActiveItem, IWeaponIkProvider, IProjectileBuilder
    {
        #region IK 필드
        ParentConstraint m_ParentConstraint;
        [SerializeField] IkTransforms m_IkTransforms;
        public IkTransforms ikTransforms => m_IkTransforms;
        [SerializeField] RuntimeAnimatorController m_OverrideAnimator;
        public RuntimeAnimatorController overrideAnimator => m_OverrideAnimator;
        #endregion

        Ammunition m_Ammunition;
        public FirearmContext firearmContext;


        public Transform actionSocket { get; set; }

        public EEquipSlotType slotContext {get; private set;}
        [SerializeField] SocketProfile socketProfile;






        void Awake()
        {
            m_ParentConstraint = GetComponent<ParentConstraint>();

            m_Ammunition = GetComponent<Ammunition>();
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

        #region Projectile Build
        [SerializeField] Transform muzzle;
        public Transform GetMuzzle() => muzzle;

        [SerializeField] int boundCount = 2;
        [SerializeField] int pierceCount = 0;

        public void BuildProjectile(ProjectileMover projectile, Vector3 targetPoint)
        {
            projectile.transform.position = muzzle.position;
            projectile.ResetConfiguration();
            projectile.SetDirection((targetPoint - muzzle.position).normalized);

            var container = projectile.hitBehaviours;
            container.ClearConfiguration();
            container.GetOrCreate(() => new DamageOnHit());

            if (boundCount > 0)
            {
                RicochetModifier ricochet = new RicochetModifier(boundCount);
                ricochet.Apply(container);
                TargetAssistModifier assist = new TargetAssistModifier();
                assist.Apply(container);
            }
            if (pierceCount > 0)
            {
                PierceModifier pierce = new PierceModifier(pierceCount);
                pierce.Apply(container);
            }

            // 이팩트 생성
            GameObject muzzleFlash = ObjectPoolService.Instance.Get(firearmContext.muzzleFlashPrefab);
            GameObject shootSound = ObjectPoolService.Instance.Get(firearmContext.shootSoundPrefab);

            muzzleFlash.transform.SetParent(muzzle);
            muzzleFlash.transform.position = muzzle.position;
            shootSound.transform.SetParent(muzzle);

        }
        #endregion

        #region Ammunition
        public bool CanFire()
        {
            return m_Ammunition.CanFire();
        }
        public bool TryConsume()
        {
            return m_Ammunition.TryConsume();
        }
        public void RefillAmmo()
        {
            m_Ammunition.RefillAmmo();
        }

        public float GetFireRate() => m_Ammunition.ammoConfig.fireRate;

        public GameObject GetProjectilePrefab() => m_Ammunition.ammoConfig.projectilePrefab;

        #endregion
    }
}
