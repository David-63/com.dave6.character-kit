using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
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

        public void Initialize(VisualTreeAsset template)
        {
            //Clear();
            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = 0;
            if (template != null)
            {
                template.CloneTree(this);
            }

            _CollectionRoot = this.Q<VisualElement>("collection-root");
            if (_CollectionRoot == null)
            {
                Debug.LogError("root 못찾음");
                return;
            }
            _RoleLabel = this.Q<Label>("role-label");
            _ExtensionLayer = this.Q<VisualElement>("extension-layer");
        }

        public void Bind(ContainerCollection collection)
        {
            UnbindEvents();

            _Collection = collection;

            _RoleLabel.text = collection.Role.ToString();
            BindEvents();

            IntialContainerViews();
        }
        #region API
        public Dictionary<IItemContainer, ContainerBaseView> GetContainerViews() => _ContainerViews;
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
        void IntialContainerViews()
        {
            _ContainerViews.Clear();
            _ExtensionLayer.Clear();

            foreach (var container in _Collection.AllContainers)
            {
                ContainerBaseView view = GameplayHub.Instance.Get<ViewFactory>().CreateContainerView(container);
                if (view == null) continue;
                _ContainerViews[container] = view;

                view.SetSourceLabel("Base");

                _ExtensionLayer.Add(view);
                view.Bind(container);
            }
        }
        void HandleContainerAdded(IItemContainer container, ContainerCollection collection)
        {
            var view = GameplayHub.Instance.Get<ViewFactory>().CreateContainerView(container);
            if (view == null) return;

            _ContainerViews[container] = view;
            var source = _Collection.GetSource(container);
            var sourceName = source != null ? source.Definition.DisplayName : "Base";
            view.SetSourceLabel(sourceName);
            _ExtensionLayer.Add(view);
            view.Bind(container);
        }
        void HandleContainerRemoved(IItemContainer container, ContainerCollection collection)
        {
            if (!_ContainerViews.TryGetValue(container, out var view)) return;

            view.RemoveFromHierarchy();
            _ContainerViews.Remove(container);
        }
    }
}
