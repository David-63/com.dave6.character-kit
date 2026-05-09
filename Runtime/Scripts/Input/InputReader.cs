using Dave6.CharacterKit.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Dave6.CharacterKit.Inputs
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Dave6/Input/InputReader")]
    public class InputReader : ScriptableObject, DaveInput.ICharacterActions, DaveInput.ILoadoutActions
    {
        DaveInput _Actions;

        // ====================================================

        // 이벤트 바인딩이 필요하면 여기에

        #region Movement
        public event UnityAction<Vector2> Move = delegate {};
        public event UnityAction<Vector2> Look = delegate {};
        public event UnityAction<bool> Jump = delegate {};
        public event UnityAction<bool> Shift = delegate {};
        public event UnityAction ShiftTap = delegate {};
        #endregion
        #region Combat
        public event UnityAction<bool> Attack = delegate {};
        public event UnityAction AttackTap = delegate {};
        public event UnityAction AttackHold = delegate {};
        public event UnityAction<bool> Focus = delegate {};

        public event UnityAction<bool> Reload = delegate {};
        public event UnityAction ReloadTap = delegate {};
        public event UnityAction ReloadHold = delegate {};
        #endregion

        #region Interaction
        public event UnityAction<bool> Interact = delegate {};
        public event UnityAction InteractTap = delegate {};
        #endregion

        #region UI
        public event UnityAction<bool> OpenLoadout = delegate {};
        public event UnityAction<bool> CloseLoadout = delegate {};

        public event UnityAction<bool> DropSelected = delegate {};
        public event UnityAction DropSelectedTap = delegate {};
        public event UnityAction<bool> InspectSelected = delegate {};
        public event UnityAction InspectSelectedTap = delegate {};

        public event UnityAction<bool> CancelAction = delegate {};
        public event UnityAction CancelActionTap = delegate {};
        #endregion

        #region System
        public event UnityAction Save = delegate {};
        public event UnityAction Load = delegate {};
        #endregion

        // ==========================
        // 미지정
        public event UnityAction<float> ScrollSelect = delegate {};
        public event UnityAction<bool> Equip = delegate {};
        public event UnityAction EquipTap = delegate {};
        public event UnityAction WeaponSwitchToggleChanged = delegate {};
        // ==========================

        #region State
        bool _ShiftToggle;
        bool _WeaponSwitchToggle;
        #endregion

        #region Raw Input Values
        public Vector2 InputMove => _Actions.Character.Move.ReadValue<Vector2>();
        public Vector2 InputLook => _Actions.Character.Look.ReadValue<Vector2>();
        public float InputScroll => _Actions.Character.ScrollSelect.ReadValue<float>();
        #endregion

        #region Lifecycle
        void OnEnable()
        {
            if (_Actions == null)
            {
                _Actions = new DaveInput();
                _Actions.Character.SetCallbacks(this);
                _Actions.Loadout.SetCallbacks(this);
            }
            EnableCharacterInput();
        }
        void OnDisable()
        {
            _Actions.Disable();                // Disable all actions within map.
        }
        #endregion

        #region Input Map Control
        public void EnableCharacterInput()
        {
            _Actions.Loadout.Disable();
            _Actions.Character.Enable();
        }

        public void EnableStatusInput()
        {
            _Actions.Character.Disable();
            _Actions.Loadout.Enable();
        }
        #endregion

        #region Character Input

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
        #endregion
        #region Loadout Input
        public void OnOpenLoadout(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                OpenLoadout?.Invoke(true);
                break;
                case InputActionPhase.Canceled:
                OpenLoadout?.Invoke(false);
                break;
            }
        }
        public void OnCloseLoadout(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                CloseLoadout?.Invoke(true);
                break;
                case InputActionPhase.Canceled:
                CloseLoadout?.Invoke(false);
                break;
            }
        }

        public void OnDropSelected(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                DropSelected?.Invoke(true);
                DropSelectedTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                DropSelected?.Invoke(false);
                break;
            }
        }

        public void OnInspectSelected(InputAction.CallbackContext context)
        {
                        switch (context.phase)
            {
                case InputActionPhase.Started:
                InspectSelected?.Invoke(true);
                InspectSelectedTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                InspectSelected?.Invoke(false);
                break;
            }
        }

        public void OnCancelAction(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                CancelAction?.Invoke(true);
                CancelActionTap?.Invoke();
                break;
                case InputActionPhase.Canceled:
                CancelAction?.Invoke(false);
                break;
            }
        }
        #endregion



        #region System Input
        public void OnSave(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Save?.Invoke();
                break;
            }
        }

        public void OnLoad(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Load?.Invoke();
                break;
            }
        }
        #endregion

        // public void OnDrop(InputAction.CallbackContext context)
        // {
        //     switch (context.phase)
        //     {
        //         case InputActionPhase.Started:
        //         Drop?.Invoke(true);
        //         DropTap?.Invoke();
        //         break;
        //         case InputActionPhase.Canceled:
        //         Drop?.Invoke(false);
        //         break;
        //     }
        // }
        // public void OnClose(InputAction.CallbackContext context)
        // {
        //     switch (context.phase)
        //     {
        //         case InputActionPhase.Started:
        //         Close?.Invoke(true);
        //         break;
        //         case InputActionPhase.Canceled:
        //         Close?.Invoke(false);
        //         break;
        //     }
        // }

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

        // 안쓰는 기능
        public void OnDrop(InputAction.CallbackContext context)
        {
            
        }
    }
}