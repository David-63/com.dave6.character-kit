using System.Collections.Generic;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public partial class SocketArea : ContainerArea
    {
        protected SocketContainer _SocketContainer;
        protected Dictionary<SocketSlot, SocketSlotView> _SlotViews = new();

        public override void Build(IItemContainer container)
        {
            Clear();
            _SlotViews.Clear();
            _SocketContainer = container as SocketContainer;

            // var layout = _SocketContainer.SocketLabelLayout;
            // SetupArea(layout);
            SetupFlow(_SocketContainer.SocketFlowLayout);

            foreach (var socket in _SocketContainer.SocketSlots)
            {
                Add(CreateSocketElement(socket));
            }
        }
        void SetupFlow(SocketFlowLayout flow)
        {
            switch (flow)
            {
                case SocketFlowLayout.Horizontal:
                    style.flexDirection = FlexDirection.Row;
                    break;
                case SocketFlowLayout.Vertical:
                    style.flexDirection = FlexDirection.Column;
                    break;
                case SocketFlowLayout.Wrapped:
                    style.flexDirection = FlexDirection.Row;
                    style.flexWrap = Wrap.Wrap;
                    style.maxWidth = 260; // 2 columns of 128px slots + margins
                    break;
                default:
                    Debug.LogError($"Unsupported SocketFlowLayout: {flow}");
                    break;
            }
        }

        VisualElement CreateSocketElement(SocketSlot socket)
        {
            var root = new VisualElement();
            if (_SocketContainer.SocketLabelLayout == SocketLabelLayout.LabelAbove)
            {
                root.style.flexDirection = FlexDirection.Column;
            }
            else
            {
                root.style.flexDirection = FlexDirection.Row;
            }
            var label = new Label(socket.SlotCategory.ToString());
            label.AddToClassList("s-source-label");
            var slot = new SocketSlotView();
            slot.style.width = 128;
            slot.style.height = 128;
            slot.Bind(socket);

            _SlotViews[socket] = slot;
            root.Add(label);
            root.Add(slot);
            return root;
        }

        void SetupArea(SocketLabelLayout layout)
        {
            switch (layout)
            {
                case SocketLabelLayout.LabelRow:
                    SetupRow();
                    break;
                case SocketLabelLayout.LabelAbove:
                    SetupColumn();
                    break;
                default:
                    Debug.LogError($"Unsupported SocketLayout: {layout}");
                    break;
            }
        }

        void SetupRow()
        {
            style.flexDirection = FlexDirection.Row;

            foreach (var socket in _SocketContainer.SocketSlots)
            {
                Debug.Log($"Setting up socket slot: {socket.SlotId} ({socket.SlotCategory})");
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
        void SetupColumn()
        {
            style.flexDirection = FlexDirection.Column;

            foreach (var socket in _SocketContainer.SocketSlots)
            {
                var row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                row.style.marginBottom = 4;
                row.style.alignItems = Align.Center;

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
                row.Add(label);
                row.Add(slot);
                Add(row);
            }
        }

        
        #region API
        public SocketSlotView GetSocketSlotView(int slotId)
        {
            var slot = _SocketContainer.SocketSlots[slotId];
            return GetSlotView(slot);
        }

        public SocketSlotView GetSlotView(SocketSlot slot)
        {
            if (_SlotViews.TryGetValue(slot, out var view))
            {
                return view;
            }
            return null;
        }
        public bool OverlapView(Rect area)
        {
            foreach (var pair in _SlotViews)
            {
                if (pair.Value.worldBound.Overlaps(area)) return true;
            }
            return false;
        }
        public SocketSlot GetSlotAtPosition(Vector2 panelPos)
        {
            foreach (var pair in _SlotViews)
            {
                if (pair.Value.worldBound.Contains(panelPos))
                {
                    return pair.Key;
                }
            }

            return null;
        }
        #endregion
    }
}
