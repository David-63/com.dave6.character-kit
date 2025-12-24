using UnityEngine;

namespace Dave6.CharacterKit.Item
{

    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "DaveAssets/Item/ItemDefinition")]
    public class ItemDefinition : ScriptableObject
    {
        public Sprite icon;
        public string displayName;
        public GameObject worldPrefab;
    
    }
}
