using System;
using System.Linq;
using Dave6.CharacterKit.Item;
using Dave6.StatSystem.Stat;
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
                    Debug.Log($"장착된 아이템{i}/Slot{equip.Key} : {equipItem.definition.displayName}");
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
            // 선택한 아이템이 장착한 아이템과 같으면 장비 해제
            if (m_Inventory.IsEquipped(selectItem, out var usedSlot))
            {
                return m_Inventory.DetachFromSlot(usedSlot);
            }

            // 목표 슬롯 찾기
            var targetSlot = ResolveSlot(selectItem);
            if (targetSlot == EEquipSlotType.None) return false;

            // 비여있는 슬롯이 아니라면 해제 먼저 진행
            var targetSlotItem = m_Inventory.GetEquippedItem(targetSlot);
            if (targetSlotItem != null)
            {
                m_Inventory.DetachFromSlot(targetSlot);
            }

            return m_Inventory.AttachToSlot(targetSlot, selectItem);
        }

        EEquipSlotType ResolveSlot(OwnedItem item)
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
            return slots.First();
        }

        public void EquipItem(EquippedItem equippedItem)
        {
            // 무기는 나중에 예외처리할 예정
            if (equippedItem.definition.category == EItemCategory.Weapon) return;

            // 스텟 반영
            // StatValue 타입일 경우에만 반영
            if (equippedItem.definition.affectMode == StatAffectMode.StatValueType)
            {
                foreach (var option in equippedItem.definition.statValueOptions)
                {
                    m_Controller.statHandler.TryGetStat(option.tag, out var stat);
                    Debug.Log($"기존 {option.tag}값: {stat.finalValue}");
                    m_Controller.statHandler.AddBaseContribution(option.tag, 
                        new BaseContribution(option.valueType, option.magnitude, equippedItem));
                    Debug.Log($"{option.tag}값 변경: {stat.finalValue}");
                }
            }
        }
        public void UnequipItem(EquippedItem equippedItem)
        {
            // 스텟 반영 제거
            if (equippedItem.definition.affectMode == StatAffectMode.StatValueType)
            {
                foreach (var option in equippedItem.definition.statValueOptions)
                {
                    m_Controller.statHandler.TryGetStat(option.tag, out var stat);
                    Debug.Log($"기존 {option.tag}값: {stat.finalValue}");
                    m_Controller.statHandler.RemoveBaseContribution(option.tag, equippedItem);
                    Debug.Log($"{option.tag}값 변경: {stat.finalValue}");
                }
            }
        }
    }
}
