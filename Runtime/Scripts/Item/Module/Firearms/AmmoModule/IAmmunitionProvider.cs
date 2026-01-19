namespace Dave6.CharacterKit.Item
{
    public interface IAmmunitionProvider
    {
        bool CanFire();
        bool TryConsume();
        void RefillAmmo();
    }
}
