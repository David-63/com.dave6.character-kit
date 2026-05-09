using Dave6.CharacterKit.Inputs;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Input
{
    public class UIInputHandler : MonoBehaviour
    {
        [SerializeField] InputReader _Input;
        LoadoutMain _LoadoutUI;
        ItemInspector _ItemInspectorUI;

        bool _IsOpen;

        #region Inject
        public void Inject(LoadoutMain loadoutUI, ItemInspector itemInspector)
        {
            _LoadoutUI = loadoutUI;
            _ItemInspectorUI = itemInspector;
        }
        #endregion

        #region Binding API
        public void InputBind()
        {
            _Input.OpenLoadout += HandleOpen;
            _Input.CloseLoadout += HandleClose;
            _Input.DropSelectedTap += HandleDrop;

            // Inspector 인풋
            _Input.InspectSelectedTap += HandleRequestInspect;
            _Input.CancelActionTap += HandleCancleInspect;
        }
        public void InputUnbind()
        {
            _Input.OpenLoadout -= HandleOpen;
            _Input.CloseLoadout -= HandleClose;
            _Input.DropSelectedTap -= HandleDrop;

            _Input.InspectSelectedTap -= HandleRequestInspect;
            _Input.CancelActionTap -= HandleCancleInspect;
        }
        #endregion

        #region Handlers
        void HandleOpen(bool pressed)
        {
            if (_LoadoutUI == null) Debug.LogWarning("UI 입력 배치 안됨");
            if (!pressed || _IsOpen) return;

            _IsOpen = true;

            _Input.EnableStatusInput(); // 핵심
            _LoadoutUI.ShowUI();
        }

        void HandleClose(bool pressed)
        {
            if (_LoadoutUI == null) Debug.LogWarning("UI 입력 배치 안됨");
            if (!pressed || !_IsOpen) return;

            _IsOpen = false;

            _Input.EnableCharacterInput(); // 핵심
            _LoadoutUI.HideUI();
            _ItemInspectorUI.Hide();
            _ItemInspectorUI.Unbind();
        }
        void HandleDrop()
        {
            _LoadoutUI.RequestDrop();
        }

        void HandleRequestInspect()
        {
            _LoadoutUI.RequestInspect();
        }
        void HandleCancleInspect()
        {
            _ItemInspectorUI.Hide();
            _ItemInspectorUI.Unbind();
        }
        #endregion
    }
}