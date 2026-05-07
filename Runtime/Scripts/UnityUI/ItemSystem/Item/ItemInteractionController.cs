using System;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Application.Mapper;
using UnityEngine;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public class ItemInteractionController
    {
        #region Dependency
        public ILoadoutProvider _LoadoutProvider;
        public IContainerViewResolver _Resolver;
        public IInteractor _Interactor;
        #endregion

        #region State
        ItemView _FocusedItem;
        public ItemView SelectedItem => _FocusedItem;
        public bool HasSelection => _FocusedItem != null;
        public event Action<ItemView> OnFocusChanged;
        #endregion

        public ItemInteractionController(IContainerViewResolver resolver, ILoadoutProvider loadoutProvider, IInteractor interactor)
        {
            _Resolver = resolver;
            _LoadoutProvider = loadoutProvider;
            _Interactor = interactor;
        }


        public void SetFocusItem(ItemView itemView)
        {
            if (_FocusedItem == itemView) return;

            _FocusedItem = itemView;
            OnFocusChanged?.Invoke(itemView);
        }
        public void ClearFocus()
        {
            if (_FocusedItem == null) return;

            _FocusedItem = null;
            OnFocusChanged?.Invoke(null);
        }
        #region Action API
        public void HandleMove(ItemView itemView)
        {
            var item = itemView.GetItem();

            var source = item.Owner;
            var sourcePlacement = source.GetPlacement(item);

            var targetView = ResolveContainerView(itemView.GetItemWorldArea());
            if (targetView == null)
            {
                _Resolver.RefreshItem(item); // 드롭 실패 시 원래 위치로 돌아가도록
                return;
            }

            var placement = targetView.ResolvePlacement(itemView.GetLeftTop());

            var request = new DropRequest
            {
                Item = item,
                Source = source,
                SourcePlacement = sourcePlacement,
                Target = targetView.GetContainer(),
                TargetPlacement = placement,
            };

            TryMove(request);
        }
        public void DropSelectedItem()
        {
            var itemView = _FocusedItem;
            var result = _LoadoutProvider.Remove(itemView.GetItem());
            if (!result.Success) return;

            GameplayHub.Instance.Get<ItemFactory>().CreateWorldItem(itemView.GetItem(), _Interactor.Origin.position);
            _FocusedItem = null;
        }
        #endregion

        #region internal
        void TryMove(DropRequest request)
        {
            var result = _LoadoutProvider.Move(request.Item, request.Target, request.TargetPlacement);

            if (!result.Success)
            {
                _Resolver.RefreshItem(request.Item);
            }
        }

        ContainerBaseView ResolveContainerView(Rect area)
        {
            var containerViews = _Resolver.GetContainerViews();

            foreach (var view in containerViews.Values)
            {
                if (view.OverlapView(area)) return view;
            }
            return null;
        }
        #endregion
    }
}
