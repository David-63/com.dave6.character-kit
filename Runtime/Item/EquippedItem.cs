using UnityEngine;

namespace Dave6.CharacterKit.Item
{

    /// <summary>
    /// 플레이어에게 기능을 제공하는 장착된 아이템
    /// </summary>
    public class EquippedItem
    {
        OwnedItem m_OwnedItem;
        public OwnedItem ownedItem => m_OwnedItem;
        ItemDefinition m_ItemDefinition;
        public ItemDefinition itemDefinition => m_ItemDefinition;

        public EquippedItem(OwnedItem ownedItem)
        {
            m_OwnedItem = ownedItem;
            m_ItemDefinition = ownedItem.definition;
        }
    }
}
