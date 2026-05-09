using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public abstract partial class ContainerBaseView : VisualElement
    {

        #region Visual
        VisualElement _Root;
        Label _SourceLabel;
        protected VisualElement _VisualArea;
        #endregion
        public virtual void Initialize(VisualTreeAsset template)
        {
            Clear();
            // style.flexGrow = 1;
            // style.flexShrink = 1;
            // style.flexBasis = 0;
            template.CloneTree(this);

            _Root = this.Q<VisualElement>("container-root");
            _SourceLabel = this.Q<Label>("source-label");
            _VisualArea = this.Q<VisualElement>("area");
        }

        protected IItemContainer _Container;
        protected ContainerArea _ContainerArea;
        public ContainerArea GetArea() => _ContainerArea;
        public void SetSourceLabel(string text)
        {
            if (_SourceLabel != null)
            {
                _SourceLabel.text = text;
            }
        }

        public virtual void Bind(IItemContainer container)
        {
            _Container = container;

            if (_ContainerArea != null)
            {
                _VisualArea.Remove(_ContainerArea);
            }

            BuildArea();
        }
        protected abstract void BuildArea();

        public abstract ItemPlacement ResolvePlacement(Vector2 panelPos);
        public abstract Vector2 PlacementToPanel(ItemPlacement placement);

        public abstract bool OverlapView(Rect area);
        public IItemContainer GetContainer() => _Container;
    }
}
