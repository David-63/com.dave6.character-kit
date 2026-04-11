using System.Collections.Generic;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public abstract partial class SocketLayoutView : VisualElement
    {
        protected SocketContainer _container;
        protected Dictionary<SocketSlot, SocketSlotView> _SlotViews = new();
        public SocketSlotView GetSocketSlotView(int slotId)
        {
            var slot = _container.SocketSlots[slotId];
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

        public virtual void Build(SocketContainer container)
        {
            _container = container;
            Build();
        }
        protected abstract void Build();

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
    }
}
