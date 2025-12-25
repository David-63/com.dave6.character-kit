using System.Collections.Generic;
using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    public enum eEquipSlotType
    {
        PrimaryWeapon,
        SecondaryWeapon,
        MeleeWeapon,
        Head,
        Chest,
        Leg,
        Charm,
        ConsumableA,
        ConsumableB,
        None,
    }
    /// <summary>
    /// 아이템은 4개의 레이어로 구성해야함
    /// 
    /// - 아이템 정의          (Definition)
    /// - 순수 아이템 데이터    (Owned Item)<- 인벤토리에 저장됨
    /// - 기능을 수행하는 아이템 (Equipped / Active Item) <- 런타임중에 기능을 수행하는 아이템
    /// - 월드에 드랍된 아이템   (World / Pickup Item)<- 상호작용이 가능한 실체 아이템
    /// 
    /// </summary>
    public class Inventory
    {
        List<OwnedItem> m_OwnedItems = new();
        public IReadOnlyList<OwnedItem> ownedItems => m_OwnedItems;

        Dictionary<eEquipSlotType, EquippedItem> m_EquippedItems = new();
        public IReadOnlyDictionary<eEquipSlotType, EquippedItem> euippedItems => m_EquippedItems;

        internal bool AddOwned(WorldItem pickupItem)
        {
            Debug.Log($"Added: {pickupItem.definition.displayName}");
            m_OwnedItems.Add(new OwnedItem(pickupItem.definition, pickupItem.stack));
            return true;
        }

        internal bool RemoveOwned(OwnedItem item, Vector3 dropPosition)
        {
            if (!m_OwnedItems.Remove(item)) return false;

            Debug.Log($"드랍한 아이템: {item.definition.displayName}");

            Object.Instantiate(item.definition.worldPrefab, dropPosition, Quaternion.identity);
            return true;
        }

        internal bool AttachToSlot(eEquipSlotType slot, OwnedItem item)
        {
            // 아이템 무결성 체크
            if (!m_OwnedItems.Contains(item)) return false;
            // 이미 사용중인 아이템
            if (IsEquipped(item, out _)) return false;

            m_EquippedItems[slot] = new EquippedItem(item);

            return true;
        }
        internal bool DetachFromSlot(eEquipSlotType slot)
        {
            if (m_EquippedItems.TryGetValue(slot, out var prev))
            {
                m_EquippedItems.Remove(slot);
                return true;
            }
            return false;
        }

        public OwnedItem GetItem(int index)
        {
            if (index < 0 || index >= m_OwnedItems.Count) return null;


            return m_OwnedItems[index];
        }

        public bool IsSlotEmpty(eEquipSlotType slot)
        {
            if (m_EquippedItems.ContainsKey(slot)) return false;

            return true;
        }
        public bool IsEquipped(OwnedItem item, out eEquipSlotType slot)
        {
            foreach (var kv in m_EquippedItems)
            {
                if (kv.Value.ownedItem == item)
                {
                    slot = kv.Key;
                    return true;
                }
            }
            slot = eEquipSlotType.None;
            return false;
        }
    }
}
