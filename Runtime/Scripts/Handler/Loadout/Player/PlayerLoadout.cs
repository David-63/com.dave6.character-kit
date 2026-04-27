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

        void HandleExtension(ContainerAction action)
        {
            // var fromCollection = action.From != null ? _Context.GetCollection(action.From) : null;
            // var toCollection   = action.To   != null ? _Context.GetCollection(action.To)   : null;

            // bool wasEquipment = fromCollection != null && IsEquipment(fromCollection);
            // bool isEquipment  = toCollection   != null && IsEquipment(toCollection);

            bool wasEquipment = action.From != null && IsItemInEquipmentBefore(action);
            bool isEquipment = action.To != null && IsItemInEquipment(action.Item);


            // 들어옴
            if (!wasEquipment && isEquipment)
            {
                foreach (var collection in _Context.GetCollections())
                {
                    collection.Value.AddExtension(action.Item);
                }
            }

            // 나감
            if (wasEquipment && !isEquipment)
            {
                foreach (var collection in _Context.GetCollections())
                {
                    collection.Value.RemoveExtension(action.Item);
                }
            }
        }
        bool IsItemInEquipment(ItemInstance item)
        {
            var owner = item.Owner;
            while (owner != null)
            {
                var collection = _Context.GetCollection(owner);
                var role = _Context.GetRole(collection);
                if (role == ExtensionRole.Equipment) return true;

                var parentItem = owner.Owner;
                if (parentItem == null) break;
                owner = parentItem.Owner;
            }
            return false;
        }
        bool IsItemInEquipmentBefore(ContainerAction action)
        {
            var owner = action.From;
            while (owner != null)
            {
                var collection = _Context.GetCollection(owner);
                var role = _Context.GetRole(collection);
                if (role == ExtensionRole.Equipment) return true;

                var parentItem = owner.Owner;
                if (parentItem == null) break;
                owner = parentItem.Owner;
            }
            return false;
        }
        bool IsEquipment(ContainerCollection collection)
        {
            foreach (var kv in _Context.GetCollections())
            {
                if (kv.Value == collection)
                    return kv.Key == ExtensionRole.Equipment;
            }
            return false;
        }
        public ContainerResult Move(ItemInstance item, IItemContainer target, ItemPlacement placement)
        {
            var result = _Service.Move(item, target, placement);
            if (!result.Success) return result;
            HandleExtension(result.Action);
            _Context.NotifyItemMoved(item, target);
            return result;
        }
        public ContainerResult Add(ItemInstance item, ExtensionRole role)
        {
            bool suceess = _Context.TryGetCollection(role, out var collection);
            if (!suceess) return ContainerResult.Fail(ContainerError.InvalidTarget);
            var result = _Service.Add(item, collection);
            if (!result.Success) return result;
            HandleExtension(result.Action);
            _Context.NotifyItemAdded(item, result.Action.To);
            return result;
        }
        public ContainerResult Add(ItemInstance item, IItemContainer target, ItemPlacement placement = null)
        {
            var result = _Service.Add(item, target, placement);
            if (!result.Success) return result;
            HandleExtension(result.Action);
            _Context.NotifyItemAdded(item, result.Action.To);
            return result;
        }
        public ContainerResult Remove(ItemInstance item)
        {
            var result = _Service.Remove(item);
            if (!result.Success) return result;
            HandleExtension(result.Action);
            _Context.NotifyItemRemoved(item);
            return result;
        }
    }
}