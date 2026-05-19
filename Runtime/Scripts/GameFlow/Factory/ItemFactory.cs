using Dave6.CharacterKit.Interactable;
using Dave6.ItemSystem.Application.Item;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Factory
{
    public class ItemFactory : MonoBehaviour, IItemFactory
    {
        [SerializeField] ItemDatabaseAsset _DefDatabaseAsset;
        ItemDatabase _Database;
        void Awake()
        {
            GameplayHub.Instance.Register(this);
            _Database = new ItemDatabase(_DefDatabaseAsset);
        }

        public ItemInstance CreateInstance(string itemId)
        {
            var entry = _Database.GetItemEntry(itemId);
            return new ItemInstance(entry.ItemDefinition);
        }
        public WorldItem CreateWorldItem(ItemInstance item, Vector3 position)
        {
            var def = item.Definition;
            var id = def.ItemId;
            var prefab = _Database.GetItemEntry(id).ItemDefinitionAsset.WorldPrefab;
            if (prefab == null)
            {
                Debug.LogError("WorldPrefab 없음");
                return null;
            }

            var go = Instantiate(prefab, position, Quaternion.identity);
            var worldItem = go.GetComponent<WorldItem>();
            if (worldItem == null)
            {
                Debug.LogError("WorldItem 컴포넌트 없음");
                return null;
            }
            worldItem.Initialize(item);

            return worldItem;
        }

        public ItemDefinitionAsset GetItemDefinitionAsset(string itemId)
        {
            return _Database.GetItemEntry(itemId).ItemDefinitionAsset;
        }
    }
}
