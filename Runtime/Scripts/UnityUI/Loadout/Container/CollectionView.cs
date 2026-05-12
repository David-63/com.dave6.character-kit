using System;
using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    /// <summary>
    /// Collection  단위 UI Root
    /// ContainerView 관리 (생성/삭제/정렬)
    /// container source label 표시
    /// extension 변화 반영
    /// </summary>
    [UxmlElement]
    public partial class CollectionView : VisualElement
    {
        ContainerCollection _Collection;
        Dictionary<IItemContainer, ContainerBaseView> _ContainerViews = new();

        VisualElement _CollectionRoot;
        Label _RoleLabel;
        VisualElement _ExtensionLayer;

        public event Action<IItemContainer> OnContainerAdded;
        public event Action<IItemContainer> OnContainerRemoved;

        public void Initialize(VisualTreeAsset template)
        {
            //Clear();
            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = 0;
            if (template == null) throw new ArgumentNullException(nameof(template));
            template.CloneTree(this);

            _CollectionRoot = this.Q<VisualElement>("collection-root");
            if (_CollectionRoot == null) throw new InvalidOperationException("Collection root not found");

            _RoleLabel = this.Q<Label>("role-label");
            _ExtensionLayer = this.Q<VisualElement>("extension-layer");
        }

        public void Bind(ContainerCollection collection)
        {
            UnbindEvents();

            _Collection = collection;

            _RoleLabel.text = collection.Role.ToString();
            BindEvents();

            Rebuild();
        }
        #region API
        public IReadOnlyDictionary<IItemContainer, ContainerBaseView> GetContainerViews() => _ContainerViews;
        public void Rebuild()
        {
            _ContainerViews.Clear();
            _ExtensionLayer.Clear();

            foreach (var container in _Collection.AllContainers)
            {
                CreateContainerView(container);
            }
        }
        #endregion
        void BindEvents()
        {
            _Collection.OnContainerAdded += HandleContainerAdded;
            _Collection.OnContainerRemoved += HandleContainerRemoved;
        }
        void UnbindEvents()
        {
            if (_Collection == null) return;

            _Collection.OnContainerAdded -= HandleContainerAdded;
            _Collection.OnContainerRemoved -= HandleContainerRemoved;
        }
        void HandleContainerAdded(IItemContainer container, ContainerCollection collection)
        {
            CreateContainerView(container);

            // 여기서 아이템 뷰 추가
            OnContainerAdded?.Invoke(container);
        }
        void HandleContainerRemoved(IItemContainer container, ContainerCollection collection)
        {
            if (!_ContainerViews.TryGetValue(container, out var view)) return;
            view.RemoveFromHierarchy();
            _ContainerViews.Remove(container);

            // 여기서 아이템 뷰 제거
            OnContainerRemoved?.Invoke(container);
        }

        void CreateContainerView(IItemContainer container)
        {
            var factory = GameplayHub.Instance.Get<ViewFactory>();
            var view = factory.CreateContainerView(container);

            if (view == null) return;
            _ContainerViews[container] = view;
            var source = _Collection.GetSource(container);
            view.SetSourceLabel(source != null ? source.Definition.DisplayName : "Base");
            _ExtensionLayer.Add(view);
            view.Bind(container);
        }
    }
}
