using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Dave6.CharacterKit.Input
{
    using static DaveInput;

    [CreateAssetMenu(fileName = "Inputs", menuName = "DaveAssets/Input/InputReader")]
    public class InputReader : ScriptableObject, ICharacterActions
    {
        DaveInput actions;

        // 이벤트 바인딩이 필요하면 여기에
        public event UnityAction<Vector2> Move = delegate {};
        public event UnityAction<Vector2> Look = delegate {};
        public event UnityAction<bool> Jump = delegate {};
        public event UnityAction<bool> Aim = delegate {};
        public event UnityAction<bool> Shift = delegate {};
        public event UnityAction ShiftToggleChanged = delegate {};

        bool _shiftToggle;

        // 입력 값을 즉시 받으려면 여기에
        public Vector2 InputMove => actions.Character.Move.ReadValue<Vector2>();
        public Vector2 InputLook => actions.Character.Look.ReadValue<Vector2>();

        void OnDestroy()
        {
            actions.Dispose();                  // Destroy asset object.
        }

        void OnEnable()
        {
            if (actions == null)
            {
                actions = new DaveInput();
                actions.Character.SetCallbacks(this);
            }
        }

        public void EnablePlayerAction()
        {
            actions.Enable();                 // Enable all actions within map.
        }

        void OnDisable()
        {
            actions.Disable();                // Disable all actions within map.
        }

        ///     #region Interface implementation of MyActions.IPlayerActions
        ///
        ///     // Invoked when "Move" action is either started, performed or canceled.
        ///     public void OnMove(InputAction.CallbackContext context)
        ///     {
        ///         Debug.Log($"OnMove: {context.ReadValue&lt;Vector2&gt;()}");
        ///     }
        ///
        ///     // Invoked when "Attack" action is either started, performed or canceled.
        ///     public void OnAttack(InputAction.CallbackContext context)
        ///     {
        ///         Debug.Log($"OnAttack: {context.ReadValue&lt;float&gt;()}");
        ///     }

        public void OnMove(InputAction.CallbackContext context)
        {
            Move?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Jump?.Invoke(true);
                break;
                case InputActionPhase.Canceled:
                Jump?.Invoke(false);
                break;
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Aim?.Invoke(true);
                break;
                case InputActionPhase.Canceled:
                Aim?.Invoke(false);
                break;
            }
        }

        public void OnShift(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                Shift?.Invoke(true);

                _shiftToggle = !_shiftToggle;
                ShiftToggleChanged?.Invoke();
                break;
                case InputActionPhase.Canceled:
                Shift?.Invoke(false);
                break;
            }
        }
    }
}