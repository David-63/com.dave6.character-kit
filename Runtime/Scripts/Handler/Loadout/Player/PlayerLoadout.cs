using System.Collections.Generic;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Loadout
{
    public class PlayerLoadout : BaseLoadout, IContainerProvider
    {
        [SerializeField] RootContainerConfigAsset _Config;

        public IEnumerable<IItemContainer> GetRootContainers() => _Context.GetRootContainers();
        public RootContainerContext GetLoadoutContext() => _Context;

        void Awake()
        {
            _Service = new();
            // 나중에 LoadoutContext로 랩핑 필요할까?
            _Context = _Config.CreateContext();

        }

        // UI 조작 입력 처리
    }
}