using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Application.Mapper;
using UnityEngine;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public class ItemInteractionController
    {
        public ILoadoutProvider _LoadoutProvider;
        public IContainerViewResolver _Resolver;
        public IInteractor _Interactor;


        public ItemView SelectedItem { get; private set; }

        public void SetFocusItem(ItemView itemView) => SelectedItem = itemView;

        public ItemInteractionController(IContainerViewResolver resolver, ILoadoutProvider loadoutProvider, IInteractor interactor)
        {
            _Resolver = resolver;
            _LoadoutProvider = loadoutProvider;
            _Interactor = interactor;
        }


        public void HandleMove(ItemView itemView)
        {
            var item = itemView.GetItem();

            var sourceContainer = item.Owner;
            var sourcePlacement = sourceContainer.GetPlacement(item);

            var targetContainerView = ResolveContainerView(itemView.GetItemWorldArea());
            if (targetContainerView == null)
            {
                _Resolver.RefreshItem(item); // 드롭 실패 시 원래 위치로 돌아가도록
                return;
            }

            var placement = targetContainerView.ResolvePlacement(itemView.GetLeftTop());


            var targetContainer = targetContainerView.GetContainer();

            var request = new DropRequest
            {
                Item = item,
                Source = sourceContainer,
                SourcePlacement = sourcePlacement,
                Target = targetContainer,
                TargetPlacement = placement,
            };

            TryMove(request);
        }
        void TryMove(DropRequest request)
        {
            var result = _LoadoutProvider.Move(request.Item, request.Target, request.TargetPlacement);

            if (!result.Success)
            {
                Debug.Log("드롭 실패");
                _Resolver.RefreshItem(request.Item);

                switch (result.Error)
                {
                    case ContainerError.InvalidTarget:
                        Debug.Log("InvalidTarget");
                        break;
                        
                    case ContainerError.InvalidItem:
                        Debug.Log("InvalidItem");
                        break;
                        
                    case ContainerError.NoSource:
                        Debug.Log("NoSource");
                        break;

                    case ContainerError.CannotAdd:
                        Debug.Log("CannotAdd");
                        break;
                        
                    case ContainerError.AddFailed:
                        Debug.Log("AddFailed");
                        break;

                    case ContainerError.RemoveFailed:
                        Debug.Log("RemoveFailed");
                        break;
                        
                }
                return;
            }
        }

        ContainerBaseView ResolveContainerView(Rect area)
        {
            var containerViews = _Resolver.GetContainerViews();

            foreach (var view in containerViews.Values)
            {
                if (view.OverlapView(area))
                {
                    return view;
                }
            }
            Debug.Log("드롭할 컨테이너 없음");
            return null;
        }

        public void DropSelectedItem()
        {
            var itemView = SelectedItem;
            var result = _LoadoutProvider.Remove(itemView.GetItem());
            if (!result.Success) return;

            GameplayHub.Instance.Get<ItemFactory>().CreateWorldItem(itemView.GetItem(), _Interactor.Origin.position);
            SelectedItem = null;
        }
    }
}
