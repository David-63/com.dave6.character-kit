using UnityEngine;

namespace Dave6.CharacterKit.Inputs
{
    public interface IInputReceiver
    {
        void OnMove(Vector2 value);
        void OnLook(Vector2 value);
        void OnAction(ActionType type, bool isPressed); // Jump, Attack, Focus 등
        void OnTap(ActionType type); // ReloadTap, InteractTap
    }

    public enum ActionType
    {
        Jump,
        Shift,
        Focus,
        Attack,
        Reload,
        Interact,
        Status,
        Close,
    }
}