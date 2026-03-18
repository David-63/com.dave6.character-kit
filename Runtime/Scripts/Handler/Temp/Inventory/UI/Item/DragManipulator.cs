using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit
{
    /// <summary>
    /// 입력 처리만 하고싶어요
    /// </summary>
    public class DragManipulator : PointerManipulator
    {
        bool isDragging;
        int m_PointerId;    // 필요없을듯?

        // 시작 정보
        Vector3 pointerStart;
        Vector2 elementStart;

        Func<Vector2, GridSpace> resolveSpace;
        public Action onDrop;

        public DragManipulator(Func<Vector2, GridSpace> resolveSpace)
        {
            this.resolveSpace = resolveSpace;
        }
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
        }
        void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0)
                return;

            isDragging = true;

            // pointerStart: 마우스 클릭 위치
            pointerStart = evt.position;
            // 상대적인 위치
            elementStart = new Vector2(target.resolvedStyle.left, target.resolvedStyle.top);

            target.CapturePointer(evt.pointerId);
        }
        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging || !target.HasPointerCapture(evt.pointerId))
                return;

            Vector2 delta = evt.position - pointerStart;

            target.style.left = elementStart.x + delta.x;
            target.style.top = elementStart.y + delta.y;
        }
        void OnPointerUp(PointerUpEvent evt)
        {
            if (!isDragging || !target.HasPointerCapture(evt.pointerId))
                return;

            isDragging = false;
            target.ReleasePointer(evt.pointerId);

            // 1. panel 좌표 계산
            Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(target.panel, evt.position);

            onDrop?.Invoke();
        }
    }
}
