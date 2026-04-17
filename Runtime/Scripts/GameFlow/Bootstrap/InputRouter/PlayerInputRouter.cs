using Dave6.CharacterKit.Inputs;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Input
{
    public class PlayerInputRouter : MonoBehaviour
    {
        [SerializeField] InputReader _Input;

        IInputReceiver _Target;

        public void SetTarget(IInputReceiver target)
        {
            _Target = target;
        }

        void OnEnable()
        {
            _Input.Move += InputMove;
            _Input.Look += InputLook;
            _Input.Jump += InputJump;
            _Input.Shift += InputShift;
            _Input.Focus += InputFocus;
            _Input.Attack += InputAttack;
            _Input.AttackTap += InputAttackTap;
            _Input.Reload += InputReload;
            _Input.ReloadTap += InputReloadTap;
            _Input.Interact += InputInteract;
            _Input.InteractTap += InputInteractTap;
        }

        void OnDisable()
        {
            _Input.Move -= InputMove;
            _Input.Look -= InputLook;
            _Input.Jump -= InputJump;
            _Input.Shift -= InputShift;
            _Input.Focus -= InputFocus;
            _Input.Attack -= InputAttack;
            _Input.AttackTap -= InputAttackTap;
            _Input.Reload -= InputReload;
            _Input.Interact -= InputInteract;
            _Input.ReloadTap -= InputReloadTap;
            _Input.InteractTap -= InputInteractTap;
        }

        void InputMove(Vector2 value) => _Target?.OnMove(value);
        void InputLook(Vector2 value) => _Target?.OnLook(value);
        void InputJump(bool value) => _Target?.OnAction(ActionType.Jump, value);
        void InputShift(bool value) => _Target?.OnAction(ActionType.Shift, value);
        void InputFocus(bool value) => _Target?.OnAction(ActionType.Focus, value);
        void InputAttack(bool value) => _Target?.OnAction(ActionType.Attack, value);
        void InputAttackTap() => _Target?.OnTap(ActionType.Attack);
        void InputReload(bool value) => _Target?.OnAction(ActionType.Reload, value);
        void InputReloadTap() => _Target?.OnTap(ActionType.Reload);
        void InputInteract(bool value) => _Target?.OnAction(ActionType.Interact, value);
        void InputInteractTap() => _Target?.OnTap(ActionType.Interact);
    }
}