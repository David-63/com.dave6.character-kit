using Dave6.CharacterKit.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Dave6.CharacterKit.Inputs
{
    [CreateAssetMenu(fileName = "Inputs", menuName = "DaveAssets/Input/InputReader")]
    public class InputReader : ScriptableObject, DaveInput.ICharacterActions, DaveInput.IStatusActions
    {
        DaveInput actions;

        // 이벤트 바인딩이 필요하면 여기에
        public event UnityAction<Vector2> Move = delegate {};
        public event UnityAction<Vector2> Look = delegate {};
        public event UnityAction<bool> Jump = delegate {};

        public event UnityAction<bool> Shift = delegate {};
        public event UnityAction ShiftTap = delegate {};

        public event UnityAction<bool> Focus = delegate {};
        public event UnityAction<bool> Attack = delegate {};
        public event UnityAction AttackTap = delegate {};
        public event UnityAction AttackHold = delegate {};

        public event UnityAction<bool> Reload = delegate {};
        public event UnityAction ReloadTap = delegate {};
        public event UnityAction ReloadHold = delegate {};

        public event UnityAction<bool> Interact = delegate {};
        public event UnityAction InteractTap = delegate {};

        public event UnityAction<float> ScrollSelect = delegate {};

        public event UnityAction<bool> Equip = delegate {};
        public event UnityAction EquipTap = delegate {};
        public event UnityAction<bool> Drop = delegate {};
        public event UnityAction DropTap = delegate {};


        public event UnityAction WeaponSwitchToggleChanged = delegate {};

        public event UnityAction<bool> OpenStatus = delegate {};
        public event UnityAction<bool> Close = delegate {};


        bool _ShiftToggle;
        bool _WeaponSwitchToggle;

        // 입력 값을 즉시 받으려면 여기에
        public Vector2 InputMove => actions.Character.Move.ReadValue<Vector2>();
        public Vector2 InputLook => actions.Character.Look.ReadValue<Vector2>();
        public float InputScroll => actions.Character.ScrollSelect.ReadValue<float>();

        void OnEnable()
        {
            if (actions == null)
            {
                actions = new DaveInput();
                actions.Character.SetCallbacks(this);
                actions.Status.SetCallbacks(this);
            }
            EnableCharacterInput();
        }
        void OnDisable()
        {
            actions.Disable();                // Disable all actions within map.
        }
        public void EnableCharacterInput()
        {
            actions.Status.Disable();
            actions.Character.Enable();
        }

        public void EnableStatusInput()
        {
            actions.Character.Disable();
            actions.Status.Enable();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) Move?.Invoke(context.ReadValue<Vector2>());
            else if (context.phase == InputActionPhase.Canceled) Move?.Invoke(Vector2.zero);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started) Jump?.Invoke(true);
            else if (context.phase == InputActionPhase.Canceled) Jump?.Invoke(false);
        }

        public void OnShift(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Shift?.Invoke(true);

                _ShiftToggle = !_ShiftToggle;
                ShiftTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Shift?.Invoke(false);
                break;
            }
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Attack?.Invoke(true);
                AttackTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Attack?.Invoke(false);
                break;
            }
        }
        public void OnInteract(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Interact?.Invoke(true);
                InteractTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Interact?.Invoke(false);
                break;
            }
        }

        public void OnScrollSelect(InputAction.CallbackContext context)
        {
            ScrollSelect?.Invoke(context.ReadValue<float>());
        }

        public void OnEquip(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Equip?.Invoke(true);
                EquipTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Equip?.Invoke(false);
                break;
            }
        }

        public void OnDrop(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Drop?.Invoke(true);
                DropTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Drop?.Invoke(false);
                break;
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Reload?.Invoke(true);
                ReloadTap?.Invoke();
                break;
                case InputActionPhase.Performed:
                ReloadHold?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Reload?.Invoke(false);
                break;
            }
        }

        public void OnWeaponSwitch(InputAction.CallbackContext context)
        {

            switch (context.phase)
            {
                case InputActionPhase.Started:
                Shift?.Invoke(true);

                _WeaponSwitchToggle = !_WeaponSwitchToggle;
                WeaponSwitchToggleChanged?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Shift?.Invoke(false);
                break;
            }

            
        }

        public void OnFocus(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Focus?.Invoke(true);
                break;
                case InputActionPhase.Canceled:
                Focus?.Invoke(false);
                break;
            }
        }

        public void OnOpenStatus(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                OpenStatus?.Invoke(true);
                break;
                case InputActionPhase.Canceled:
                OpenStatus?.Invoke(false);
                break;
            }
        }

        public void OnClose(InputAction.CallbackContext context)
        {
            
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Close?.Invoke(true);
                break;
                case InputActionPhase.Canceled:
                Close?.Invoke(false);
                break;
            }
        }
    }
}