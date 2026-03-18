using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    public class Ammunition : MonoBehaviour, IAmmunitionProvider
    {
        [SerializeField] AmmoConfig m_Config;
        public AmmoConfig ammoConfig => m_Config;
        int m_CurAmmo;

        void Awake()
        {
            m_CurAmmo = m_Config.maxCapacity;
        }

        public bool CanFire()
        {
            if (m_CurAmmo <= 0) return false;
            return true;
        }
        public bool TryConsume()
        {
            if (m_CurAmmo <= 0) return false;
            m_CurAmmo--;

            return true;
        }
        public void RefillAmmo()
        {
            m_CurAmmo = m_Config.maxCapacity;
        }
    }
}
