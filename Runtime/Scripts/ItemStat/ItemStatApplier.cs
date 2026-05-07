using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.ItemSystem.Domain.Item;
using Dave6.StatSystem2.Application;
using Dave6.StatSystem2.Domain;
using UnityEngine;

namespace Dave6.CharacterKit.ItemStat
{
    /// <summary>
    /// 이건 매니저급 책임을 가져야할듯
    /// </summary>
    public class ItemStatApplier : MonoBehaviour
    {
        ItemStatDatabase _StatDatabase;
        public ItemStatDatabase StatDatabase => _StatDatabase;

        [SerializeField] List<ItemStatDefinitionAsset> _StatDefinitionAssets;

        void Awake()
        {
            GameplayHub.Instance.Register(this);

            _StatDatabase = new ItemStatDatabase();
            foreach (var asset in _StatDefinitionAssets)
            {
                _StatDatabase.Register(new ItemStatDefinition(asset.ItemId, asset.Modifiers));
            }
        }

        public void ApplyItem(StatController entity, ItemInstance item)
        {
            RemoveItem(entity, item); // 중복 적용 방지 위해 기존에 적용된 아이템 제거 후 다시 적용
            if (!_StatDatabase.TryGet(item.Definition.ItemId, out var statDef)) return;

            Debug.Log($"Applying stats for item {item.Definition.DisplayName}");

            foreach (var modifier in statDef.Modifiers)
            {
                var runtimeModifier = new StatModifier(item, modifier.Tag, modifier.Type, modifier.Value);
                entity.ApplyModifier(runtimeModifier);
            }
        }
        public void RemoveItem(StatController entity, ItemInstance item)
        {
            entity.RemoveSource(item);
        }
    }
}