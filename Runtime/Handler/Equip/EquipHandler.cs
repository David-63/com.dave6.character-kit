using System;
using Dave6.CharacterKit.Item;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class EquipHandler
    {
        PlayerController m_Controller;
        Inventory m_Inventory;

        OwnedItem m_SelectedItem;
        public OwnedItem selectedItem => m_SelectedItem;
        int m_CurrentIndex = -1;


        public EquipHandler(PlayerController controller, Inventory inventory)
        {
            m_Controller = controller;
            m_Inventory = inventory;
        }

        public void OnUpdate()
        {
            HandleSelectionItem();

            if (m_Controller.equipInputTap)
            {
                TryToggleSelectedItem();
            }
            if (m_Controller.dropInputTap)
            {
                TryDropSelected();
            }
        }

        public void HandleSelectionItem()
        {
            var items = m_Inventory.ownedItems;

            if (items.Count == 0)
            {
                m_SelectedItem = null;
                m_CurrentIndex = -1;
                return;
            }
            if (m_Controller.inputScroll == 0) return;
            int prevIndex = m_CurrentIndex;

            if (m_CurrentIndex < 0)
            {
                m_CurrentIndex = 0;
            }
            else if (m_Controller.inputScroll > 0)
            {
                // select next
                m_CurrentIndex = (m_CurrentIndex + 1) % items.Count;
            }
            else if (m_Controller.inputScroll < 0)
            {
                // select prev
                m_CurrentIndex = (m_CurrentIndex - 1 + items.Count) % items.Count;
            }
            if (prevIndex != m_CurrentIndex)
            {
                Debug.Log($"Current select index: {m_CurrentIndex}");
                m_SelectedItem = items[m_CurrentIndex];
                Debug.Log($"Selected: {m_SelectedItem.definition.displayName}");
            }
        }
        public bool TryDropSelected()
        {
            if (m_SelectedItem == null) return false;

            if (m_Inventory.IsEquipped(m_SelectedItem, out var slot))
            {
                m_Inventory.DetachFromSlot(slot);
            }

            m_Inventory.RemoveOwned(m_SelectedItem, m_Controller.transform.position);

            m_SelectedItem = null;
            m_CurrentIndex = -1;

            return true;
        }

        public bool TryToggleSelectedItem()
        {
            if (m_SelectedItem == null) return false;

            bool success = TryEquip(m_SelectedItem);
            if (success)
            {
                m_SelectedItem = null;
                m_CurrentIndex = -1;
                int i = 1;
                foreach (var equip in m_Inventory.euippedItems)
                {
                    EquippedItem equipItem = equip.Value;
                    Debug.Log($"장착된 아이템{i}/Slot{equip.Key} : {equipItem.itemDefinition.displayName}");
                    i++;
                }
            }
            if (m_Inventory.euippedItems.Count <= 0)
            {
                Debug.Log($"장착된 아이템 없음");
            }

            return success;
        }


        /// <summary>
        /// 선택한 아이템이
        /// </summary>
        /// <returns></returns>
        bool TryEquip(OwnedItem selectItem)
        {
            // 선택된 아이템이 이미 장착되있으면 해제
            if (m_Inventory.IsEquipped(selectItem, out var usedSlot))
            {
                m_Inventory.DetachFromSlot(usedSlot);
                return true;
            }

            // 목표 슬롯 찾기
            var targetSlot = ResolveSlot(selectItem);
            if (targetSlot == eEquipSlotType.None) return false;
            

            // 비여있는 슬롯이 아니라면 해제 먼저 진행
            if (!m_Inventory.IsSlotEmpty(targetSlot))
            {
                m_Inventory.DetachFromSlot(targetSlot);
            }

            return m_Inventory.AttachToSlot(targetSlot, selectItem);
        }

        eEquipSlotType ResolveSlot(OwnedItem item)
        {
            var slots = item.definition.allowedSlots;

            // 1. 빈 슬롯 우선 찾기
            foreach(var slot in slots)
            {
                if (m_Inventory.IsSlotEmpty(slot))
                {
                    return slot;
                }
            }

            // 2. 빈 슬롯이 없으면 스왑 대상 선택
            return ResolveSlot(slots);
        }

        eEquipSlotType ResolveSlot(eEquipSlotType[] slots)
        {
            return slots[^1];
        }
    }
}
