using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Application.Mapper;
using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public class LoadoutMainPanel : MonoBehaviour, IContainerViewResolver
    {
        #region VisualElement
        VisualElement _Root;
        VisualElement _ContentsLayer;
        VisualElement _ItemLayer;
        #endregion

        #region Dependency
        ILoadoutProvider _LoadoutProvider;
        ItemInteractionController _InteractionController;
        #endregion

        #region Views
        Dictionary<ExtensionRole, CollectionView> _CollectionViews = new();
        Dictionary<ItemInstance, ItemView> _itemViews = new();

        ItemView _CurrentSelected;
        #endregion

        #region Lifesycle
        void Awake()
        {
            var doc = GetComponent<UIDocument>();
            _Root = doc.rootVisualElement.Q<VisualElement>("main-root");
            Initialize();
            GameplayHub.Instance.Register(this);
        }

        void Initialize()
        {
            _ContentsLayer = _Root.Q<VisualElement>("contents-layer");
            _ItemLayer = _Root.Q<VisualElement>("item-layer");
            
            _ItemLayer.pickingMode = PickingMode.Ignore;    // 이거 지워도 괜찬을지도?
            _Root.RegisterCallback<PointerDownEvent>(OnBackgroundClick);

            HideUI();
        }
        #endregion
        #region Binding
        public void Bind(ILoadoutProvider provider, IInteractor interactor)
        {
            if (_LoadoutProvider == provider) return;
            _LoadoutProvider = provider;

            _InteractionController = new ItemInteractionController(this, _LoadoutProvider, interactor);
            _InteractionController.OnFocusChanged += HandleFocusChanged;

            BindEvents();
            BuildCollection();
        }
        #endregion

        #region Events
        void BindEvents()
        {
            var context = _LoadoutProvider.GetContext();

            context.OnItemAdded += HandleItemAdded;
            context.OnItemRemoved += HandleItemRemoved;
            context.OnItemMoved += HandleItemMoved;
        }
        void OnBackgroundClick(PointerDownEvent evt)
        {
            if (evt.target is ItemView) return;

            _InteractionController.ClearFocus();
        }
        #endregion

        #region Build
        void BuildCollection()
        {
            _ContentsLayer.Clear();
            _CollectionViews.Clear();

            foreach (var collection in _LoadoutProvider.GetContext().GetCollections())
            {
                CollectionView view = GameplayHub.Instance.Get<ViewFactory>().CreateCollectionView();
                if (view == null) continue;
                _CollectionViews[collection.Key] = view;
                _ContentsLayer.Add(view);
                view.Bind(collection.Value);
            }
        }
        #endregion

        #region Item Event
        void HandleItemAdded(ItemInstance item, IItemContainer target)
        {
            // 아이템 뷰 생성
            var itemView = CreateItemView(item);
            var placement = target.GetPlacement(item);

            PlaceItem(itemView, placement);
        }
        void HandleItemRemoved(ItemInstance item, IItemContainer from)
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
        void HandleFocusChanged(ItemView itemView)
        {
            if (_CurrentSelected != null)
            {
                _CurrentSelected.RemoveFromClassList("s-item-selected");
            }
            _CurrentSelected = itemView;
            if (_CurrentSelected != null)
            {
                _CurrentSelected.AddToClassList("s-item-selected");
            }
        }
        #endregion

        #region Item View
        ItemView CreateItemView(ItemInstance item)
        {
            var view = GameplayHub.Instance.Get<ViewFactory>().CreateItemView(_InteractionController);
            view.Bind(item);
            _itemViews[item] = view;
            _ItemLayer.Add(view);
            return view;
        }
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
        #endregion

        #region UI Control API

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
        #endregion
        public void Rebuild()
        {
            _ItemLayer.Clear();
            _itemViews.Clear();

            BuildItemView();
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



        #region ViewResolver
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
