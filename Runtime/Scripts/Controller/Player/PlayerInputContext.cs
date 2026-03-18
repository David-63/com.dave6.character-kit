using UnityEngine;

namespace Dave6.CharacterKit
{
    public class PlayerInputContext
    {
        public Vector2 move;
        public Vector2 look;

        // type: hold
        public bool jump;
        public bool focus;
        public bool shift;

        public bool attack;
        public bool reload;
        public bool interact;

        // type: tap
        public bool shiftTap;
        public bool attackTap;
        public bool reloadTap;
        public bool interactTap;

        public void SetMoveInput(Vector2 input) => move = input;

        public bool HasMoveInput() => move.sqrMagnitude > 0.001f;
    }
}
