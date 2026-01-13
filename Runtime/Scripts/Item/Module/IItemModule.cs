namespace Dave6.CharacterKit.Item
{
    // 모듈 초안
    public interface IItemModule
    {
        void OnAttach();
        void OnDetach();
        void OnTrigger();
    }

    public enum ItemTirgger
    {
        OnEquip,
        OnUnequip,
        Firearm,
        OnHit,
        OnDamageTaken,
        OnTick,
    }
}
