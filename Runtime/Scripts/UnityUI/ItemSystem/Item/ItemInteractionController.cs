using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Application.Mapper;
using UnityEngine;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public class ItemInteractionController
    {
        public ILoadoutProvider _LoadoutProvider;
        public IContainerViewResolver _Resolver;



        public ItemInteractionController(IContainerViewResolver resolver, ILoadoutProvider loadoutProvider)
        {
            _Resolver = resolver;
            _LoadoutProvider = loadoutProvider;
        }


        public void HandleDrop(ItemView itemView)
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

            TryDrop(request);
        }

        void TryDrop(DropRequest request)
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
    }
}
