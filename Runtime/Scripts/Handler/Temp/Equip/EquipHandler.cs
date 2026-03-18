// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Dave6.CharacterKit.Item;
// using Dave6.CharacterKit.RigControl;
// using Dave6.StatSystem.Stat;
// using UnityEngine;

// namespace Dave6.CharacterKit.EquipHandle
// {
//     public enum EActiveItemSocket
//     {
//         WeaponSocket,
//         Sidearm,
//         Back,
//     }
//     [Serializable]
//     public struct ActiveItemSocketEntry
//     {
//         public EEquipSlotType socketType;
//         public Transform socketTransfrom;
//     }
//     public class EquipHandler : MonoBehaviour
//     {
//         PlayerCharacter m_Controller;
//         Inventory1 m_Inventory;
//         public OwnedItem selectedItem {get; private set;}
//         int m_CurrentIndex = -1;

//         Transform m_HandSocket;
//         Transform m_CombatSocket;
//         [SerializeField] List<ActiveItemSocketEntry> m_ActiveItemSockets;
//         Dictionary<EEquipSlotType, Transform> m_SocketMap = new();
//         Dictionary<EEquipSlotType, IActiveItem> m_ActiveItemMap = new();

//         public IActiveItem selectedFirearm;

//         void Awake()
//         {
//             if (!TryGetSocket<HandSocket>(out m_HandSocket))
//                 Debug.LogError("HandSocket 없음");

//             if (!TryGetSocket<CombatSocket>(out m_CombatSocket))
//                 Debug.LogError("CombatSocket 없음");
//         }

//         public void Initialize(PlayerCharacter controller, Inventory1 inventory)
//         {
//             m_Controller = controller;
//             m_Inventory = inventory;

//             foreach (var entry in m_ActiveItemSockets)
//             {
//                 m_SocketMap[entry.socketType] = entry.socketTransfrom;
//             }

//             m_Controller.GetInputReader().WeaponSwitchToggleChanged += OnWeaponSwitchToggle;
//         }

//         public void OnDestroy()
//         {
//             m_Controller.GetInputReader().WeaponSwitchToggleChanged -= OnWeaponSwitchToggle;
//         }

//         public void OnUpdate()
//         {
//             HandleSelectionItem();

//             if (m_Controller.equipInputTap)
//             {
//                 TryToggleSelectedItem();
//             }
//             if (m_Controller.dropInputTap)
//             {
//                 TryDropSelected();
//             }
//         }

//         /// <summary>
//         /// 인덱싱으로 아이템 선택 함수
//         /// </summary>
//         public void HandleSelectionItem()
//         {
//             var items = m_Inventory.ownedItems;

//             if (items.Count == 0)
//             {
//                 selectedItem = null;
//                 m_CurrentIndex = -1;
//                 return;
//             }
//             if (m_Controller.inputScroll == 0) return;
//             int prevIndex = m_CurrentIndex;

//             if (m_CurrentIndex < 0)
//             {
//                 m_CurrentIndex = 0;
//             }
//             else if (m_Controller.inputScroll > 0)
//             {
//                 // select next
//                 m_CurrentIndex = (m_CurrentIndex + 1) % items.Count;
//             }
//             else if (m_Controller.inputScroll < 0)
//             {
//                 // select prev
//                 m_CurrentIndex = (m_CurrentIndex - 1 + items.Count) % items.Count;
//             }
//             if (prevIndex != m_CurrentIndex)
//             {
//                 Debug.Log($"Current select index: {m_CurrentIndex}");
//                 selectedItem = items[m_CurrentIndex];
//                 Debug.Log($"Selected: {selectedItem.definition.displayName}");
//             }
//         }
//         /// <summary>
//         /// 선택된 아이템 드랍
//         /// </summary>
//         /// <returns></returns>
//         public bool TryDropSelected()
//         {
//             if (selectedItem == null) return false;

//             if (m_Inventory.IsEquipped(selectedItem, out var slot))
//             {
//                 m_Inventory.DetachFromSlot(slot);
//             }

//             m_Inventory.RemoveOwned(selectedItem, m_Controller.transform.position);

//             selectedItem = null;
//             m_CurrentIndex = -1;

//             return true;
//         }

//         /// <summary>
//         /// 선택한 아이템 장착
//         /// </summary>
//         /// <returns></returns>
//         public bool TryToggleSelectedItem()
//         {
//             if (selectedItem == null) return false;

//             bool success = TryEquip(selectedItem);
//             if (success)
//             {
//                 selectedItem = null;
//                 m_CurrentIndex = -1;
//                 int i = 1;
//                 foreach (var equip in m_Inventory.euippedItems)
//                 {
//                     EquippedItem equipItem = equip.Value;
//                     Debug.Log($"장착된 아이템{i}/Slot{equip.Key} : {equipItem.definition.displayName}");
//                     i++;
//                 }
//             }
//             if (m_Inventory.euippedItems.Count <= 0)
//             {
//                 Debug.Log($"장착된 아이템 없음");
//             }

//             return success;
//         }


//         /// <summary>
//         /// 선택한 아이템이
//         /// </summary>
//         /// <returns></returns>
//         bool TryEquip(OwnedItem selectItem)
//         {
//             // 선택한 아이템이 장착한 아이템과 같으면 장비 해제
//             if (m_Inventory.IsEquipped(selectItem, out var usedSlot))
//             {
//                 return m_Inventory.DetachFromSlot(usedSlot);
//             }

//             // 목표 슬롯 찾기
//             var targetSlot = ResolveSlot(selectItem);
//             if (targetSlot == EEquipSlotType.None) return false;

//             // 비여있는 슬롯이 아니라면 해제 먼저 진행
//             var targetSlotItem = m_Inventory.GetEquippedItem(targetSlot);
//             if (targetSlotItem != null)
//             {
//                 m_Inventory.DetachFromSlot(targetSlot);
//             }

//             return m_Inventory.AttachToSlot(targetSlot, selectItem);
//         }

//         EEquipSlotType ResolveSlot(OwnedItem item)
//         {
//             var slots = item.definition.allowedSlots;

//             // 1. 빈 슬롯 우선 찾기
//             foreach(var slot in slots)
//             {
//                 if (m_Inventory.IsSlotEmpty(slot))
//                 {
//                     return slot;
//                 }
//             }

//             // 2. 빈 슬롯이 없으면 스왑 대상 선택
//             return slots.First();
//         }

//         /// <summary>
//         /// 여기서 장착한 아이템 생성함
//         /// </summary>
//         public void EquipItem(EEquipSlotType slot, EquippedItem equippedItem)
//         {
//             // 스텟 반영
//             // StatValue 타입일 경우에만 반영
//             if (equippedItem.definition.affectMode == EStatAffectMode.StatValueType)
//             {
//                 foreach (var option in equippedItem.definition.statValueOptions)
//                 {
//                     m_Controller.statHandler.TryGetStat(option.tag, out var stat);
//                     Debug.Log($"기존 {option.tag}값: {stat.finalValue}");
//                     m_Controller.statHandler.AddBaseContribution(option.tag, 
//                         new BaseContribution(option.valueType, option.magnitude, equippedItem));
//                     Debug.Log($"{option.tag}값 변경: {stat.finalValue}");
//                 }
//             }
//             // active 있는 경우
//             if (equippedItem.definition.activePrefab != null)
//             {
//                 // 오브젝트 생성 및 스크립트 가져오기
//                 GameObject itemObj = m_Controller.InstantiatePrefabSetParent(equippedItem.definition.activePrefab);
//                 IActiveItem activeItem = itemObj.GetComponent<IActiveItem>();

//                 // 슬롯 바인딩
//                 activeItem.Equip(slot);
//                 m_ActiveItemMap[slot] = activeItem; // 이미 있는데 할당할 경우 문제 생길 수 있음


//                 if (activeItem is IWeaponIkProvider initialWeapon)
//                 {
//                     initialWeapon.BindWeaponPoseIK(m_SocketMap[activeItem.slotContext], m_HandSocket, m_CombatSocket);
//                 }
//                 if (selectedFirearm == null)
//                 {
//                     OnWeaponSwitchToggle();
//                 }
//             }
//         }
//         public void UnequipItem(EEquipSlotType slot, EquippedItem equippedItem)
//         {
//             // 스텟 반영 제거
//             if (equippedItem.definition.affectMode == EStatAffectMode.StatValueType)
//             {
//                 foreach (var option in equippedItem.definition.statValueOptions)
//                 {
//                     m_Controller.statHandler.TryGetStat(option.tag, out var stat);
//                     Debug.Log($"기존 {option.tag}값: {stat.finalValue}");
//                     m_Controller.statHandler.RemoveBaseContribution(option.tag, equippedItem);
//                     Debug.Log($"{option.tag}값 변경: {stat.finalValue}");
//                 }
//             }

//             if (m_ActiveItemMap.TryGetValue(slot, out var activeItem))
//             {
//                 if (selectedFirearm == activeItem) selectedFirearm = null;


//                 activeItem.Unequip();
//                 m_Controller.rigController.BindIK(null);

//                 m_ActiveItemMap.Remove(slot);
//             }
//         }


//         public void OnWeaponSwitchToggle()
//         {
//             var hasPrimary = m_ActiveItemMap.ContainsKey(EEquipSlotType.PrimaryWeapon);
//             var hasSecondary = m_ActiveItemMap.ContainsKey(EEquipSlotType.SecondaryWeapon);

//             // 둘 다 없으면 종료
//             if (!hasPrimary && !hasSecondary) return;

//             var nextWeapon = FindNextWeapon(hasPrimary, hasSecondary);

//             if (nextWeapon == null) return;

//             // apply
//             ApplyWeaponSelection(nextWeapon);
//         }

//         /// <summary>
//         /// 찾기만 하자
//         /// </summary>
//         IActiveItem FindNextWeapon(bool hasPrimary, bool hasSecondary)
//         {
//             // 교체
//             if (hasPrimary && hasSecondary && selectedFirearm != null)
//             {
//                 return GetSwitchedWeapon();
//             }

//             // 남아있는거 반환
//             var nextSlot = hasPrimary ? EEquipSlotType.PrimaryWeapon : EEquipSlotType.SecondaryWeapon;
//             return m_ActiveItemMap[nextSlot];
//         }
//         /// <summary>
//         /// 기존거 집어놓고 지금있는거 반환
//         /// </summary>
//         IActiveItem GetSwitchedWeapon()
//         {
//             var prevSlot = selectedFirearm.slotContext;
//             var nextSlot = prevSlot == EEquipSlotType.PrimaryWeapon ? EEquipSlotType.SecondaryWeapon : EEquipSlotType.PrimaryWeapon;

//             return m_ActiveItemMap[nextSlot];
//         }

//         /// <summary>
//         /// 이 부분을 RigController가 수행
//         /// </summary>
//         /// <param name="nextWeapon"></param>
//         void ApplyWeaponSelection(IActiveItem nextWeapon)
//         {
//             if (selectedFirearm == nextWeapon) return;

//             if (selectedFirearm is IWeaponIkProvider prevWeaponIK)
//             {
//                 prevWeaponIK.SetWeaponPose(EWeaponPose.Holster);
//             }

//             selectedFirearm = nextWeapon;

//             if (selectedFirearm is IWeaponIkProvider nextWeaponIK)
//             {

//                 m_Controller.rigController.BindIK(nextWeaponIK);
//                 m_Controller.animatorHandler.BindAnimator(nextWeaponIK);

//                 if (m_Controller.IsAim())
//                 {
//                     m_Controller.rigController.SetWeaponPose(EWeaponPose.Combat);
//                 }
//                 else
//                 {
//                     m_Controller.rigController.SetWeaponPose(EWeaponPose.Hand);
//                 }
//             }
//         }

//         #region Equipment API
//         public bool HasFirearm()
//         {
//             return selectedFirearm != null;
//         }
//         #endregion


//         bool TryGetSocket<T>(out Transform socket) where T : Component
//         {
//             var comp = GetComponentInChildren<T>(true);
//             if (comp == null)
//             {
//                 socket = null;
//                 return false;
//             }

//             socket = comp.transform;
//             return true;
//         }
//     }
// }
