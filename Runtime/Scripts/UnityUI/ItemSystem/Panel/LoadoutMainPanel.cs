using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Application.Mapper;
using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public class LoadoutMainPanel : MonoBehaviour, IContainerViewResolver
    {
        VisualElement _Root;
        ILoadoutProvider _LoadoutProvider;

        VisualElement _ContentsContainer;
        VisualElement _ItemLayer;

        ItemInteractionController _InteractionController;

        Dictionary<IItemContainer, ContainerBaseView> _ContainerViews = new ();
        Dictionary<ItemInstance, ItemView> _itemViews = new();


        void Awake()
        {
            var doc = GetComponent<UIDocument>();
            _Root = doc.rootVisualElement.Q<VisualElement>("main-root");
            Initialize();
            GameplayHub.Instance.Register(this);
        }

        void Initialize()
        {
            _ContentsContainer = _Root.Q<VisualElement>("contents-container");

            _ItemLayer = _Root.Q<VisualElement>("item-layer");
            _ItemLayer.pickingMode = PickingMode.Ignore;
            _ItemLayer.style.position = Position.Absolute;
            _ItemLayer.style.top = 0;
            _ItemLayer.style.bottom = 0;
            _ItemLayer.style.left = 0;
            _ItemLayer.style.right = 0;

            HideUI();
        }
        #region API
        public void Bind(ILoadoutProvider provider)
        {
            if (_LoadoutProvider == provider) return;
            _LoadoutProvider = provider;

            _InteractionController = new ItemInteractionController(this, _LoadoutProvider);

            if (_ContentsContainer == null) _ContentsContainer = _Root.Q<VisualElement>("contents-container");

            BindEvents();

            BuildContainerView();

            //RefreshLayout();
        }

        public void ShowUI()
        {
            _Root.style.visibility = Visibility.Visible;
            //_Root.style.display = DisplayStyle.Flex;
        }

        public void HideUI()
        {
            _Root.style.visibility = Visibility.Hidden;
            //_Root.style.display = DisplayStyle.None;
        }
        public void Rebuild()
        {
            _ItemLayer.Clear();
            _itemViews.Clear();

            BuildItemView();
        }
        #endregion

        void BindEvents()
        {
            var context = _LoadoutProvider.GetContext();

            context.OnItemAdded += HandleItemAdded;
            context.OnItemRemoved += HandleItemRemoved;
            context.OnItemMoved += HandleItemMoved;
        }

        // dirty 플래그를 통해서 호출하도록 하기¿
        void BuildContainerView()
        {
            _ContainerViews.Clear();
            _ContentsContainer.Clear();

            foreach (var root in _LoadoutProvider.GetContext().GetRootContainers())
            {
                ContainerBaseView view = GameplayHub.Instance.Get<ViewFactory>().CreateContainerView(root.Value, _InteractionController);
                if (view == null) continue;
                _ContainerViews[root.Value] = view;

                _ContentsContainer.Add(view);
                view.Bind(root.Value);
            }
        }
        void BuildItemView()
        {
            foreach (var item in _LoadoutProvider.GetContext().GetItemsAll())
            {
                
                var itemView = GameplayHub.Instance.Get<ViewFactory>().CreateItemView(_InteractionController);
                if (itemView == null) continue;

                itemView.Bind(item);
                _itemViews[item] = itemView;

                _ItemLayer.Add(itemView);

                var placement = item.Owner.GetPlacement(item);

                PlaceItem(itemView, placement);
            }
        }

        #region Handle Item
        void HandleItemAdded(ItemInstance item, IItemContainer target)
        {
            // 아이템 뷰 생성
            var itemView = GameplayHub.Instance.Get<ViewFactory>().CreateItemView(_InteractionController);
            itemView.Bind(item);
            _itemViews.Add(item, itemView);

            // 아이템 뷰 등록
            _ItemLayer.Add(itemView);
            var placement = target.GetPlacement(item);

            PlaceItem(itemView, placement);
        }
        void HandleItemRemoved(ItemInstance item)
        {
            var view = _itemViews[item];
            view.RemoveFromHierarchy();
        }
        void HandleItemMoved(ItemInstance item, IItemContainer target)
        {
            var view = _itemViews[item];

            // _ContainerViews[target] 이렇게 container view를 찾아서 월드포지션을 찾는 방법으로 개선해야함
            var placement = target.GetPlacement(item);
            PlaceItem(view, placement);
        }
        #endregion

        void PlaceItem(ItemView itemView, ItemPlacement placement)
        {
            var container = itemView.GetItem().Owner;
            var containerView = _ContainerViews[container];
            var panelPos = containerView.PlacementToPanel(placement);

            Vector2 totalPos = _ItemLayer.WorldToLocal(panelPos);
            Debug.Log($"PlaceItem: {itemView.GetItem().Definition.DisplayName} to {totalPos}");

            itemView.style.left = totalPos.x;
            itemView.style.top = totalPos.y;
        }

        #region IContainerViewResolver
        public ReadOnlyDictionary<IItemContainer, ContainerBaseView> GetContainerViews()
        {
            return new ReadOnlyDictionary<IItemContainer, ContainerBaseView>(_ContainerViews);
        }

        public void RefreshItem(ItemInstance item)
        {
            var view = _itemViews[item];
            var container = item.Owner;
            var placement = container.GetPlacement(item);
            PlaceItem(view, placement);
        }
        #endregion
    }
    public interface IContainerViewResolver
    {
        //ContainerBaseView ResolveContainer(Vector2 worldPos);
        ReadOnlyDictionary<IItemContainer, ContainerBaseView> GetContainerViews();
        void RefreshItem(ItemInstance item);
    }
}
