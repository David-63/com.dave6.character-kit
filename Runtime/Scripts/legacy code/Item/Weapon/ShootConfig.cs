using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    [CreateAssetMenu(fileName = "Shoot Config", menuName = "DaveAssets/Item/FireArm/Shoot Config")]
    public class ShootConfig : ScriptableObject
    {
        public LayerMask hitMask;
        public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
        public float FireRate = 650;        
    }
}
