using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public abstract partial class ContainerBaseView : VisualElement
    {
        protected IItemContainer _Container;
        protected ItemInteractionController _InteractionController; // 굳이 필요없어보이는데

        public abstract void Initialize(VisualTreeAsset template);

        public abstract void Bind(IItemContainer container);

        public abstract ItemPlacement ResolvePlacement(Vector2 panelPos);
        public abstract Vector2 PlacementToPanel(ItemPlacement placement);

        public abstract bool OverlapView(Rect area);
        public IItemContainer GetContainer() => _Container;
    }
}
