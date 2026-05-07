using Dave6.CharacterKit.GameFlow;
using Dave6.StatSystem2.Application;

namespace Dave6.CharacterKit.Handler.Stat
{
    public class PlayerStat : BaseStat
    {
        public void Awake()
        {
            StatController = new StatController();
            GameplayHub.Instance.Register(this);
        }
    }
}
