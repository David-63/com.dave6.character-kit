using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    public interface IActiveItem
    {
        /// <summary>
        /// 장착 위치 기록
        /// </summary>
        Transform actionSocket { get; set; }    // 장착 위치
        EEquipSlotType slotContext { get; }

        bool CanPerformAction();                // 공격, 방어 등 가능한지
        void PerformAction();                   // 발사, 근접 공격 등
        void CancelAction();                    // 액션 취소 (재장전 등)

        void Equip(EEquipSlotType slot);        // 장비 슬롯에 등록
        void Unequip();                         // 캐릭터에서 제거
    }


}
