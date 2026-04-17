namespace Dave6.CharacterKit.Handler.Combat
{
    public interface IActionModule
    {
        void TryAction(BaseActionContext ctx, IActionAnimation anim);

        bool IsFinished(BaseActionContext ctx);
        //bool EvaluateExit(BaseActionContext ctx, out EActionExitReason reason);
        void CleanupAction(BaseActionContext ctx);
    }

}