using Dave6.ItemSystem.Domain.Container;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public abstract partial class ContainerBaseView : VisualElement
    {
        protected IItemContainer _container;

        public abstract void Initialize(VisualTreeAsset template);

        public abstract void Bind(IItemContainer container);
    }
}
