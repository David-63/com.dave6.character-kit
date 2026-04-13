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
    public class PlayerLoadout : BaseLoadout, IProvider, ILoadoutProvider
    {
        [SerializeField] RootContainerConfigAsset _Config;

        public ContainerService GetService() => _Service;
        public LoadoutRootContext GetContext() => _Context;

        void Awake()
        {
            _Service = new();
            _Context = _Config.CreateContext();
            GameplayHub.Instance.Register(this);
        }

        public IEnumerable<(RootContainerRole, IItemContainer)> GetRootContainerPairs()
        {
            foreach (var kv in _Context.GetRootContainers())
            {
                yield return (kv.Key, kv.Value);
            }
        }

        public ContainerResult Move(ItemInstance item, IItemContainer target, ItemPlacement placement)
        {
            return _Service.Move(_Context, item, target, placement);
        }
        public ContainerResult Add(ItemInstance item, RootContainerRole role)
        {
            bool success = _Context.TryGetRoot(role, out var container);
            if (!success) return ContainerResult.Fail(ContainerError.InvalidTarget);
            return _Service.Add(_Context, item, container);
        }
        public ContainerResult Add(ItemInstance item, IItemContainer target, ItemPlacement placement = null)
        {
            return _Service.Add(_Context, item, target, placement);
        }
        public ContainerResult Remove(ItemInstance item)
        {
            return _Service.Remove(_Context, item);
        }
    }
}