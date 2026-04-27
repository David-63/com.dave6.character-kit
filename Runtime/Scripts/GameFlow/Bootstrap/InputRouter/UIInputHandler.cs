using Dave6.CharacterKit.Inputs;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Input
{
    public class UIInputHandler : MonoBehaviour
    {
        [SerializeField] InputReader _Input;
        LoadoutMainPanel _UI;

        bool _IsOpen;

        public void SetUI(LoadoutMainPanel ui) => _UI = ui;

        void OnEnable()
        {
            _Input.OpenLoadout += HandleOpen;
            _Input.Close += HandleClose;
            _Input.DropTap += HandleDrop;
        }

        void OnDisable()
        {
            _Input.OpenLoadout -= HandleOpen;
            _Input.Close -= HandleClose;
            _Input.DropTap -= HandleDrop;
        }

        void HandleOpen(bool pressed)
        {
            if (_UI == null) Debug.LogWarning("UI 입력 배치 안됨");
            if (!pressed || _IsOpen) return;

            _IsOpen = true;

            _Input.EnableStatusInput(); // 핵심
            _UI.ShowUI();
        }

        void HandleClose(bool pressed)
        {
            if (_UI == null) Debug.LogWarning("UI 입력 배치 안됨");
            if (!pressed || !_IsOpen) return;

            _IsOpen = false;

            _Input.EnableCharacterInput(); // 핵심
            _UI.HideUI();
        }
        void HandleDrop()
        {
            _UI.RequestDrop();
        }
    }
}