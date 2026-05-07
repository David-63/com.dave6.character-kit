using Dave6.StatSystem2.Application;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Stat
{
    public abstract class BaseStat : MonoBehaviour
    {
        public StatController StatController { get; protected set; }
    }
}
