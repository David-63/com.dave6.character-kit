using System;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class SocketContainerView : ContainerBaseView
    {
        SocketContainer _SocketContainer;
        SocketArea _SocketView;

        protected override void BuildArea()
        {
            _SocketContainer = _Container as SocketContainer;
            _ContainerArea = new SocketArea();
            _SocketView = _ContainerArea as SocketArea;
            _VisualArea.Add(_ContainerArea);
            SetupSocket();
        }
        void SetupSocket()
        {
            _ContainerArea.Build(_SocketContainer);
        }

        public override ItemPlacement ResolvePlacement(Vector2 panelPos)
        {
            var socket = _SocketView.GetSlotAtPosition(panelPos);
            if (socket == null) return null;
            return new SocketPlacement(socket.SlotId);
        }

        public override Vector2 PlacementToPanel(ItemPlacement placement)
        {
            if (placement is not SocketPlacement sp) return Vector2.zero;
            var slot = _SocketContainer.SocketSlots[sp.SlotId];
            var slotView = _SocketView.GetSlotView(slot);

            var localPos = new Vector2(slotView.worldBound.xMin, slotView.worldBound.yMin);
            // Debug.Log($"Slot {sp.SlotId} localPos: {localPos}");
            return localPos;
        }

        public override bool OverlapView(Rect area)
        {
            return _SocketView.OverlapView(area);
        }

        Vector2 SlotToLocal(int slotId)
        {
            var view = _SocketView.GetSocketSlotView(slotId);
            return view.worldBound.position;
        }
        public Vector2 LocalToPanel(Vector2 localPos)
        {
            return _SocketView.LocalToWorld(localPos);
        }
    }
}
