using Dave6.ItemSystem.Application.Item;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Factory
{
    public class ItemFactory : MonoBehaviour, IItemFactory
    {
        [SerializeField] ItemDatabaseAsset _DatabaseAsset;
        ItemDatabase _Database;

        void Awake()
        {
            GameplayHub.Instance.Register(this);
            _Database = new ItemDatabase(_DatabaseAsset);
        }

        public ItemInstance CreateInstance(string itemId)
        {
            var def = _Database.GetDefinition(itemId);
            return new ItemInstance(def);
        }
    }

}
