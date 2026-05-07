using System.Collections.Generic;

namespace Dave6.CharacterKit.ItemStat
{
    public class ItemStatDatabase
    {
        Dictionary<string, ItemStatDefinition> _Definitions = new();

        public void Register(ItemStatDefinition definition)
        {
            _Definitions[definition.ItemId] = definition;
        }
        public bool TryGet(string itemId, out ItemStatDefinition definition) => _Definitions.TryGetValue(itemId, out definition);
    }
}