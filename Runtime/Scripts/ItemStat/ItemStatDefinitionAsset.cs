using System.Collections.Generic;
using Dave6.StatSystem2.Domain;
using UnityEngine;

namespace Dave6.CharacterKit.ItemStat
{
    [CreateAssetMenu(fileName = "ItemStatDefinition", menuName = "Dave6/ItemStat/ItemStatDefinition")]
    public class ItemStatDefinitionAsset : ScriptableObject
    {
        public string ItemId;
        public List<StatModifier> Modifiers;
    }
}