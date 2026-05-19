using System;
using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Application.Mapper;
using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Loadout
{
    /// <summary>
    /// Facade 패턴?
    /// </summary>
    public class PlayerLoadout : BaseLoadout, ILoadoutProvider
    {
        [SerializeField] ContainerCollectionConfigAsset _Config;

        public ContainerService GetService() => _Service;
        public LoadoutRootContext GetContext() => _Context;

        void Awake()
        {
            _Service = new();
            _Context = _Config.CreateContext();
            GameplayHub.Instance.Register(this);
        }
        public IEnumerable<(ExtensionRole, IItemContainer)> GetRootContainerPairs()
        {
            foreach (var kv in _Context.GetCollections())
            {
                foreach (var container in kv.Value.AllContainers)
                {
                    yield return (kv.Key, container);
                }
            }
        }

        void RefreshExtension(ContainerAction action)
        {
            bool was = _Context.WasEquipped(action);
            bool now = _Context.IsEquipped(action.Item);

            if (!was && now)
            {
                foreach (var collection in _Context.GetCollections())
                {
                    var affected = collection.Value.AttachExtension(action.Item);
                    _Context.NotifyItemsInvalidated(affected);
                }
            }

            if (was && !now)
            {
                foreach (var collection in _Context.GetCollections())
                {
                    var evicted = collection.Value.DetachExtension(action.Item);
                    _Context.NotifyItemsInvalidated(evicted);
                }
            }
        }
        public ContainerResult Move(ItemInstance item, IItemContainer target, ItemPlacement placement)
        {
            return Commit(_Service.Move(item, target, placement));
        }
        public ContainerResult Add(ItemInstance item, ExtensionRole role)
        {
            bool suceess = _Context.TryGetCollection(role, out var collection);
            if (!suceess) return ContainerResult.Fail(ContainerError.InvalidTarget);
            return Commit(_Service.Add(item, collection));
        }
        public ContainerResult Add(ItemInstance item, IItemContainer target, ItemPlacement placement = null)
        {
            return Commit(_Service.Add(item, target, placement));
        }
        public ContainerResult Remove(ItemInstance item)
        {
            return Commit(_Service.Remove(item));
        }
        public ContainerResult Commit(ContainerResult result)
        {
            if (!result.Success) return result;
            RefreshExtension(result.Action);
            _Context.NotifyItemChanged(result.Action.Item, result);
            return result;
        }
    }
}