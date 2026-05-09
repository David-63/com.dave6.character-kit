using System;
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
    /// <summary>
    /// Loadout UI Root
    /// - CollectionView 생성/관리
    /// - ItemView 생성/배치
    /// - Selection / Interaction 처리
    /// </summary>
    public class LoadoutMain : MonoBehaviour, IContainerViewResolver
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

        #region State
        bool _Initialized;
        bool _ViewReady;
        IVisualElementScheduledItem _ScheduledPlacement;
        #endregion

        #region Views
        Dictionary<ExtensionRole, CollectionView> _CollectionViews = new();
        Dictionary<ItemInstance, ItemView> _ItemViews = new();
        ItemView _CurrentSelected;
        ItemInstance _SelectedItem;
        #endregion

        #region Events
        public event Action<ItemInstance> OnInspectRequested;
        #endregion

        #region Lifecycle
        void Awake()
        {
            InitializeUI();
            GameplayHub.Instance.Register(this);
        }
        void OnDisable()
        {
            UnbindContextEvents();
            if (_InteractionController != null) _InteractionController.OnFocusChanged -= HandleFocusChanged;
        }
        #endregion

        #region Initialize
        void InitializeUI()
        {
            if (_Initialized) return;
            var doc = GetComponent<UIDocument>();
            _Root = doc.rootVisualElement.Q<VisualElement>("main-root");
            _ContentsLayer = _Root.Q<VisualElement>("contents-layer");
            _ItemLayer = _Root.Q<VisualElement>("item-layer");
            
            _ItemLayer.pickingMode = PickingMode.Ignore;
            _Root.RegisterCallback<PointerDownEvent>(HandleBackgroundClick);

            HideUI();
            _Initialized = true;
        }
        #endregion
        #region Binding
        public void Bind(ILoadoutProvider provider, IInteractor interactor)
        {
            if (_LoadoutProvider == provider) return;
            UnbindContextEvents();
            _LoadoutProvider = provider;

            if (_InteractionController != null) _InteractionController.OnFocusChanged -= HandleFocusChanged;
            _InteractionController = new ItemInteractionController(this, _LoadoutProvider, interactor);
            _InteractionController.OnFocusChanged += HandleFocusChanged;

            BindContextEvent();
            BuildViews();
        }
        void BindContextEvent()
        {
            var context = _LoadoutProvider.GetContext();

            context.OnItemAdded += HandleItemAdded;
            context.OnItemMoved += HandleItemMoved;
            context.OnItemRemoved += HandleItemRemoved;
        }
        void UnbindContextEvents()
        {
            if (_LoadoutProvider == null) return;
            var context = _LoadoutProvider.GetContext();

            context.OnItemAdded -= HandleItemAdded;
            context.OnItemMoved -= HandleItemMoved;
            context.OnItemRemoved -= HandleItemRemoved;
        }
        #endregion

        #region Build
        void BuildViews()
        {
            _ViewReady = false;
            BuildCollectionViews();
            BuildItemViews();

            _ScheduledPlacement?.Pause();
            _ScheduledPlacement = _Root.schedule.Execute(() =>
            {
                RebuildItemPlacement();
                _ViewReady = true;
            });
        }
        void BuildCollectionViews()
        {
            _ContentsLayer.Clear();
            _CollectionViews.Clear();
            foreach (var pair in _LoadoutProvider.GetContext().GetCollections())
            {
                var role = pair.Key;
                var collection = pair.Value;

                var view = GameplayHub.Instance.Get<ViewFactory>().CreateCollectionView();
                if (view == null) continue;

                view.Bind(collection);
                _CollectionViews[role] = view;
                _ContentsLayer.Add(view);
            }
        }
        void BuildItemViews()
        {
            _ItemLayer.Clear();
            _ItemViews.Clear();

            foreach (var item in _LoadoutProvider.GetContext().GetItemsAll())
            {
                CreateItem(item);
            }
        }
        void RebuildItemPlacement()
        {
            foreach (var item in _ItemViews.Keys)
            {
                RefreshItem(item);
            }
        }
        #endregion

        #region Item Event
        void HandleItemAdded(ItemInstance item, IItemContainer target)
        {
            if (!_ViewReady)
            {
                Debug.LogWarning($"View not ready: {item.Definition.DisplayName}");
                return;
            }
            CreateItem(item);
            _Root.schedule.Execute(() =>
            {
                RefreshItem(item);
            });
        }
        void HandleItemRemoved(ItemInstance item, IItemContainer from)
        {
            if (!_ItemViews.TryGetValue(item, out var view)) return;
            view.RemoveFromHierarchy();
            _ItemViews.Remove(item);
        }

        void HandleItemMoved(ItemInstance item, IItemContainer target)
        {
            _Root.schedule.Execute(() =>
            {
                RefreshItem(item);
            });
        }
        #endregion
        #region Selection
        void HandleFocusChanged(ItemView itemView)
        {
            if (_CurrentSelected != null)
            {
                _CurrentSelected.RemoveFromClassList("s-item-selected");
            }
            _CurrentSelected = itemView;
            _SelectedItem = itemView != null ? itemView.GetItem() : null;
            if (_CurrentSelected != null)
            {
                _CurrentSelected.AddToClassList("s-item-selected");
            }
        }
        void HandleBackgroundClick(PointerDownEvent evt)
        {
            if (evt.target is ItemView) return;

            _InteractionController.ClearFocus();
        }
        #endregion

        #region Item View
        void CreateItem(ItemInstance item)
        {
            if (_ItemViews.ContainsKey(item)) return;
            var view = GameplayHub.Instance.Get<ViewFactory>().CreateItemView(_InteractionController);
            view.Bind(item);
            _ItemViews[item] = view;
            _ItemLayer.Add(view);
        }

        void PlaceItem(ItemView itemView, ItemPlacement placement)
        {
            var item = itemView.GetItem();
            var container = item.Owner;
            if (container == null)
            {
                throw new InvalidOperationException($"Owner missing: {item.Definition.DisplayName}");
            }

            if (!TryResolveContainerView(container, out var containerView))
            {
                throw new InvalidOperationException($"ContainerView missing: {item.Definition.DisplayName}");
            }

            var panelPos = containerView.PlacementToPanel(placement);
            Vector2 localPos = _ItemLayer.WorldToLocal(panelPos);

            itemView.style.left = localPos.x;
            itemView.style.top = localPos.y;
        }
        #endregion

        #region View Resolver
        bool TryResolveContainerView(IItemContainer container, out ContainerBaseView containerView)
        {
            foreach (var collectionView in _CollectionViews.Values)
            {
                var views = collectionView.GetContainerViews();
                if (views.TryGetValue(container, out containerView)) return true;
            }

            containerView = null;
            return false;
        }
        public ReadOnlyDictionary<IItemContainer, ContainerBaseView> GetContainerViews()
        {
            var dict = new Dictionary<IItemContainer, ContainerBaseView>();
            foreach (var collectionView in _CollectionViews.Values)
            {
                foreach (var pair in collectionView.GetContainerViews())
                {
                    dict[pair.Key] = pair.Value;
                }
            }

            return new ReadOnlyDictionary<IItemContainer, ContainerBaseView>(dict);
        }

        public void RefreshItem(ItemInstance item)
        {
            if (!_ItemViews.TryGetValue(item, out var view)) return;
            var container = item.Owner;
            if (container == null) return;
            var placement = container.GetPlacement(item);
            PlaceItem(view, placement);
        }
        #endregion

        #region Public API

        public void ShowUI()
        {
            //_Root.style.display = DisplayStyle.Flex;  // 이거 사용하면 ui 계산 순서가 어긋남
            _Root.style.visibility = Visibility.Visible;
            RebuildItemPlacement();
        }

        public void HideUI()
        {
            //_Root.style.display = DisplayStyle.None;
            _Root.style.visibility = Visibility.Hidden;
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
        public void RequestInspect()
        {
            if (_SelectedItem == null) return;
            OnInspectRequested?.Invoke(_SelectedItem);
        }
        public void Rebuild()
        {
            BuildViews();
        }
        #endregion
    }
    public interface IContainerViewResolver
    {
        ReadOnlyDictionary<IItemContainer, ContainerBaseView> GetContainerViews();
        void RefreshItem(ItemInstance item);
    }
}
