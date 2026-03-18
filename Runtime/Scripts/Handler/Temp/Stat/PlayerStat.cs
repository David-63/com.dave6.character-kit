using Dave6.StatSystem;
using Dave6.StatSystem.Interaction;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Stat
{
    public interface IEntityStat
    {
        
    }
    /// <summary>
    /// 플레이어 캐릭터의 스텟 핸들링 제공
    /// </summary>
    public class PlayerStat : MonoBehaviour, IStatReceiver
    {
        StatHandler m_StatHandler;
        public StatHandler StatHandler => m_StatHandler;
        [SerializeField] StatDatabase m_StatDatabase;
        [SerializeField] StatTagCollection m_StatTags;

        void Awake()
        {
            m_StatHandler = new StatHandler(m_StatDatabase);
        }

        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }

    }
}