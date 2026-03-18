using System.Collections.Generic;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Stat
{
    [CreateAssetMenu(fileName = "StatTagCollection", menuName = "DaveAssets/Character/Stat/Stat Tag Collection")]
    public class StatTagCollection : ScriptableObject
    {
        public List<StatTag> statTags;
    }
}