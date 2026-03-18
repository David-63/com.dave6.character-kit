using UnityUtils.Timer;

namespace Dave6.CharacterKit.Handler.Combat
{
    public class PlayerCombatContext : BaseActionContext
    {
        // state

        public bool IsFocus;
        public bool HasFirearm = true;

        // melee
        public bool HasBufferedInput;

        // reload
        //public bool wantReload;
        public bool Reloading;

        public bool AnimReloadFinished;

    }

}