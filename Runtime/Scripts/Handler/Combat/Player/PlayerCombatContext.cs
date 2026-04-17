namespace Dave6.CharacterKit.Handler.Combat
{
    public class PlayerCombatContext : BaseActionContext
    {
        // state
        public bool IsFocus;
        public bool HasFirearm = true;

        // melee
        public bool HasBufferedInput; // 나중에 공격 딜레이간 선입력 유지용

        // reload
        //public bool wantReload;
        public bool Reloading;

        //public bool AnimReloadFinished;

    }

}