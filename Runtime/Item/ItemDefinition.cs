using System;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    public enum EItemCategory
    {
        Weapon,
        Armor,
        Consumable,
    }

    public enum EEquipSlotType
    {
        // Weapon
        PrimaryWeapon,
        SecondaryWeapon,
        MeleeWeapon,

        // Armour
        Head,
        Chest,
        Leg,
        Charm,

        // Consumable
        ConsumableA,
        ConsumableB,
        None,
    }
    /// <summary>
    /// 아이템이 스탯에 영향을 주는 방식
    /// StatValueType: 스탯 값에 직접 영향을 줌
    /// ValueOperationType: 값 연산에 영향을 줌
    /// </summary>
    [Serializable]    
    public enum StatAffectMode
    {
        StatValueType,
        ValueOperationType,
    }

    [Serializable]
    public class StatValueOption
    {
        public StatTag tag;
        public StatValueType valueType;
        public float magnitude;
    }
    [Serializable]
    public class ValueOperationOption
    {
        public StatTag tag;
        public ValueOperationType operationType;
        public float magnitude;
    }
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "DaveAssets/Item/ItemDefinition")]
    public class ItemDefinition : ScriptableObject
    {
        public Sprite icon;
        public string displayName;
        public GameObject worldPrefab;
        
        public EItemCategory category;
        public EEquipSlotType[] allowedSlots;


        public StatAffectMode affectMode;
        public StatValueOption[] statValueOptions;
        public ValueOperationOption[] valueOperationOptions;
    }
}
