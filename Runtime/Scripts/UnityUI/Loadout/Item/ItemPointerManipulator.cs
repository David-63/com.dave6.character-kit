using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    /// <summary>
    /// 입력 해석과 이벤트 전달
    /// </summary>
    public class ItemPointerManipulator : PointerManipulator
    {
        ItemInteractionController _Controller;
        bool _IsPressed, _IsDragging;
        float _PressTime;

        // 시작 정보
        /// <summary>
        /// pointerStart: 마우스 클릭 위치 |
        /// elementStart: 클릭한 요소의 상대적인 위치
        /// </summary>
        Vector3 pointerStart, elementStart;
        const float _ClickThreshold = 0.2f; // 클릭과 롱프레스 구분 임계값
        const float _DragThreshold = 5f; // 드래그 시작 임계값

        public ItemPointerManipulator(ItemInteractionController controller) => _Controller = controller;

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

        #region Pointer Event
        void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0) return;

            _IsPressed = true;
            _IsDragging = false;

            pointerStart = evt.position; // pointerStart: 마우스 클릭 위치
            elementStart = new Vector2(target.resolvedStyle.left, target.resolvedStyle.top); // 상대적인 위치
            _PressTime = Time.time;

            target.CapturePointer(evt.pointerId);
        }
        /// <summary>
        /// 드래그 판정 및 수행
        /// </summary>
        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!_IsPressed || !target.HasPointerCapture(evt.pointerId)) return;

            float distance = (evt.position - pointerStart).magnitude;

            if (!_IsDragging && distance > _DragThreshold) DragStart();

            if (_IsDragging) Drag(evt);
        }
        void OnPointerUp(PointerUpEvent evt)
        {
            if (!_IsPressed || !target.HasPointerCapture(evt.pointerId)) return;

            _IsPressed = false;
            target.ReleasePointer(evt.pointerId);

            float duration = Time.time - _PressTime;

            if      (_IsDragging)                   Drop();
            else if (duration < _ClickThreshold)    Click();
            else                                    LongPress();
        }
        #endregion

        #region Action

        void DragStart() => _IsDragging = true;
        void Drag(PointerMoveEvent evt)
        {
            Vector2 delta = evt.position - pointerStart;

            target.style.left = elementStart.x + delta.x;
            target.style.top = elementStart.y + delta.y;
        }

        void Drop()
        {
            _Controller.HandleMove(target as ItemView);
        }
        void Click()
        {
            _Controller.SetFocusItem(target as ItemView);
        }
        void LongPress()
        {
            _Controller.SetFocusItem(target as ItemView);
        }
        #endregion
    }
}