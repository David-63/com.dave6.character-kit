using Dave6.ItemSystem.Domain.Container;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class SocketSlotView : VisualElement
    {
        public SocketSlot Slot { get; private set; }

        public void Bind(SocketSlot slot) => Slot = slot;
    }
}
