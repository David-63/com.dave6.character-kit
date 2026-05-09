using Dave6.ItemSystem.Domain.Container;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public abstract partial class ContainerArea : VisualElement
    {
        public abstract void Build(IItemContainer container);
    }
}
