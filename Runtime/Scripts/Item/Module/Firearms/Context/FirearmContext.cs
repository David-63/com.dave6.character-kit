using System.Collections.Generic;
using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    [CreateAssetMenu(fileName = "FirearmContext", menuName = "DaveAssets/Item/Module/Firearm Context")]
    public class FirearmContext : ScriptableObject
    {
        public GameObject shootSoundPrefab;
        public GameObject muzzleFlashPrefab;
    }
}
