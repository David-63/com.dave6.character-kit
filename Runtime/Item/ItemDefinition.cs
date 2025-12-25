using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    public enum eItemCategory
    {
        Weapon,
        Armour,
        Consumable,
    }
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "DaveAssets/Item/ItemDefinition")]
    public class ItemDefinition : ScriptableObject
    {
        public Sprite icon;
        public string displayName;
        public GameObject worldPrefab;
        public eItemCategory category;
        public eEquipSlotType[] allowedSlots;
    }
}
