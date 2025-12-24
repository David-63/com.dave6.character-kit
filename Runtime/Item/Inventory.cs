using System.Collections.Generic;
using UnityEngine;

namespace Dave6.CharacterKit.Item
{
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
        List<OwnedItem> ownedItems = new();
        public IReadOnlyList<OwnedItem> items => ownedItems;

        public bool Pickup(WorldItem pickupItem)
        {
            Debug.Log($"Added: {pickupItem.definition.displayName}");
            ownedItems.Add(new OwnedItem(pickupItem.definition, pickupItem.stack));
            return true;
        }

        public bool Drop(OwnedItem item, Vector3 dropPosition)
        {
            if (!ownedItems.Remove(item)) return false;

            Object.Instantiate(item.definition.worldPrefab, dropPosition, Quaternion.identity);
            return true;
        }

        public OwnedItem GetItem(int index)
        {
            if (index < 0 || index >= ownedItems.Count) return null;


            return ownedItems[index];
        }

        bool Select()
        {
            return true;
        }

        bool Use(int idx)
        {
            return true;
        }
    }
}
