using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public abstract partial class ContainerBaseView : VisualElement
    {
        protected IItemContainer _Container;
        protected ItemInteractionController _InteractionController;

        public abstract void Initialize(VisualTreeAsset template, ItemInteractionController interactionController);

        public abstract void Bind(IItemContainer container);

        public abstract ItemPlacement ResolvePlacement(Vector2 panelPos);
        public abstract Vector2 PlacementToPanel(ItemPlacement placement);

        public abstract bool OverlapView(Rect area);
        public IItemContainer GetContainer() => _Container;
    }
}
