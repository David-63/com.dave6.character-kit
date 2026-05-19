using Dave6.CharacterKit.GameFlow;
using Dave6.StatSystem2.Application;

namespace Dave6.CharacterKit.Handler.Stats
{
    public class PlayerStat : BaseStat
    {
        
        protected override void Initialize()
        {
            GameplayHub.Instance.Register(this);

            StatController.Initialize(_StatGroup);
        }
    }
}
