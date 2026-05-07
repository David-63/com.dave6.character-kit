using System;
using System.Collections.Generic;
using Dave6.StatSystem2.Domain;

namespace Dave6.CharacterKit.ItemStat
{
    [Serializable]
    public class ItemStatDefinition
    {
        public string ItemId;
        public List<StatModifier> Modifiers = new();
        public ItemStatDefinition(string itemId, List<StatModifier> modifiers)
        {
            ItemId = itemId;
            Modifiers = modifiers;
        }
    }
}