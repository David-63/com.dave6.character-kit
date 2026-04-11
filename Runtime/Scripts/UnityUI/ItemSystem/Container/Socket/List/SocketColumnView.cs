using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class SocketColumnView : SocketLayoutView
    {
        protected override void Build()
        {
            Clear();

            style.flexDirection = FlexDirection.Row;

            foreach (var socket in _container.SocketSlots)
            {
                var column = new VisualElement();
                column.style.flexDirection = FlexDirection.Column;
                column.style.marginBottom = 4;
                column.style.alignItems = Align.Center;

                var label = new Label(socket.SlotCategory.ToString());
                label.style.width = 100;
                label.style.unityTextAlign = TextAnchor.MiddleLeft;
                label.style.fontSize = 14;
                label.style.color = Color.white;
                label.style.unityFontStyleAndWeight = FontStyle.Bold;

                var slot = new SocketSlotView();
                slot.style.width = 128;
                slot.style.height = 128;
                slot.style.borderTopWidth = 1;
                slot.style.borderBottomWidth = 1;
                slot.style.borderLeftWidth = 1;
                slot.style.borderRightWidth = 1;
                slot.style.borderTopColor = Color.gray;
                slot.style.borderBottomColor = Color.gray;
                slot.style.borderLeftColor = Color.gray;
                slot.style.borderRightColor = Color.gray;
                slot.name = "socket-slot";
                slot.Bind(socket);
                _SlotViews.Add(socket, slot);

                column.Add(label);
                column.Add(slot);
                Add(column);
            }
        }
    }
    
}
