using Dave6.CharacterKit.AnimHandler;

namespace Dave6.CharacterKit.Handler.Combat
{
    public class ReloadModule : IActionModule
    {
        public bool IsAvailable {get;}

        public void TryAction(BaseActionContext ctx, IActionAnimation anim)
        {
            PlayerCombatContext playerCtx = ctx as PlayerCombatContext;

            playerCtx.Reloading = true;

            if (playerCtx.IsFocus)
            {
                anim.PlayAction("Firearm_Reload_Strafe");
            }
            else
            {
                anim.PlayAction("Firearm_Reload_Freelook");
            }
        }
        public bool EvaluateExit(BaseActionContext ctx, out EActionExitReason reason)
        {
            PlayerCombatContext playerCtx = ctx as PlayerCombatContext;
            if (!playerCtx.Reloading)
            {
                reason = EActionExitReason.LeaseExpired;
                return true;
            }
            reason = EActionExitReason.None;
            return false;
        }

        public void CleanupAction(BaseActionContext ctx)
        {
            PlayerCombatContext playerCtx = ctx as PlayerCombatContext;
            playerCtx.Reloading = false;
        }
    }

}