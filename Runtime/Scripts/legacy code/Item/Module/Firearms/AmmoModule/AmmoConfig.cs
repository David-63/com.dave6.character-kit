using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    [CreateAssetMenu(fileName = "AmmoConfig", menuName = "DaveAssets/Item/Firearm/Ammo Config")]
    public class AmmoConfig : ScriptableObject
    {
        public GameObject projectilePrefab;
        public int maxCapacity;
        public float fireRate;
    }
}
