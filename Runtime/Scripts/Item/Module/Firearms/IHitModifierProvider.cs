using System.Collections.Generic;

namespace Dave6.CharacterKit.Item
{
    public interface IHitModifierProvider
    {
        IEnumerable<IHitModifier> GetHitModifiers();
    }
}
