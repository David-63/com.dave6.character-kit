using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class SocketContainerView : ContainerBaseView
    {
        VisualElement _Contents;
        SocketContainer _SocketContainer;
        SocketLayoutView _SocketView;

        public override void Initialize(VisualTreeAsset template, ItemInteractionController interactionController)
        {
            _InteractionController = interactionController;
            Clear();
            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = 0;
            template.CloneTree(this);

            _Contents = this.Q<VisualElement>("socket-root");
        }

        public override void Bind(IItemContainer container)
        {
            _Container = container;
            _SocketContainer = container as SocketContainer;

            if (_SocketView != null) _Contents.Remove(_SocketView);

            var layout = _SocketContainer.SocketLayout;
            _SocketView = CreateSocketView(layout);

            _Contents.Add(_SocketView);
            _SocketView.Build(_SocketContainer);
        }
        SocketLayoutView CreateSocketView(SocketLayout type)
        {
            return type switch
            {
                SocketLayout.LabelRow => new SocketRowView(),
                SocketLayout.LabelAbove => new SocketColumnView(),
                _ => new SocketRowView()
            };
        }

        public override ItemPlacement ResolvePlacement(Vector2 panelPos)
        {
            var socket = _SocketView.GetSlotAtPosition(panelPos);
            if (socket == null) return null;
            return new SoketPlacement(socket.SlotId);
        }

        public override Vector2 PlacementToPanel(ItemPlacement placement)
        {
            if (placement is not SoketPlacement sp) return Vector2.zero;
            var slot = _SocketContainer.SocketSlots[sp.SlotId];
            var slotView = _SocketView.GetSlotView(slot);

            var localPos = new Vector2(slotView.worldBound.xMin, slotView.worldBound.yMin);
            Debug.Log($"Slot {sp.SlotId} localPos: {localPos}");
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
