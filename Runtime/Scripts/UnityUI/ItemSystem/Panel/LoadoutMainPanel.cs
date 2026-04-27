using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Application.Mapper;
using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEditor.UIElements;
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

        Dictionary<ExtensionRole, ContainerCollectionView> _CollectionViews = new();
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
        public void Bind(ILoadoutProvider provider, IInteractor interactor)
        {
            if (_LoadoutProvider == provider) return;
            _LoadoutProvider = provider;

            _InteractionController = new ItemInteractionController(this, _LoadoutProvider, interactor);

            if (_ContentsContainer == null) _ContentsContainer = _Root.Q<VisualElement>("contents-container");

            BindEvents();

            InitialCollectionView();
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
        public ItemView GetSelectedItem()
        {
            return _InteractionController.SelectedItem;
        }
        public void RequestDrop()
        {
            if (_InteractionController.SelectedItem == null) return;

            _InteractionController.DropSelectedItem();
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
        void InitialCollectionView()
        {
            _ContentsContainer.Clear();
            _CollectionViews.Clear();

            foreach (var collection in _LoadoutProvider.GetContext().GetCollections())
            {
                ContainerCollectionView view = GameplayHub.Instance.Get<ViewFactory>().CreateCollectionView();
                if (view == null) continue;
                _CollectionViews[collection.Key] = view;
                _ContentsContainer.Add(view);
                view.Bind(collection.Value);
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
            _itemViews.Remove(item);
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
            var containerViews = GetContainerViews();

            if (!containerViews.TryGetValue(container, out var containerView))
            {
                Debug.LogError("ContainerView not found");
                return;
            }

            var panelPos = containerView.PlacementToPanel(placement);
            Vector2 totalPos = _ItemLayer.WorldToLocal(panelPos);
            Debug.Log($"PlaceItem: {itemView.GetItem().Definition.DisplayName} to {totalPos}");

            itemView.style.left = totalPos.x;
            itemView.style.top = totalPos.y;
        }

        #region IContainerViewResolver
        public ReadOnlyDictionary<IItemContainer, ContainerBaseView> GetContainerViews()
        {
            var dict = new Dictionary<IItemContainer, ContainerBaseView>();
            foreach (var collectionView in _CollectionViews.Values)
            {
                foreach (var kv in collectionView.GetContainerViews())
                {
                    dict[kv.Key] = kv.Value;
                }
            }

            return new ReadOnlyDictionary<IItemContainer, ContainerBaseView>(dict);
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
