using UnityEngine;

namespace Dave6.CharacterKit.Item
{

    /// <summary>
    /// 플레이어에게 기능을 제공하는 장착된 아이템
    /// </summary>
    public class EquippedItem
    {
        public OwnedItem ownedItem { get; private set; }
        public ItemDefinition definition { get; private set; }

        public EquippedItem(OwnedItem ownedItem)
        {
            this.ownedItem = ownedItem;
            definition = ownedItem.definition;
        }
    }
}
