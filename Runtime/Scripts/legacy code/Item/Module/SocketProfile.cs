using System;
using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    [Serializable]
    public struct OffsetLocation
    {
        public Vector3 offsetPos;
        public Vector3 offsetRot;
    }
    [CreateAssetMenu(fileName = "SocketProfile", menuName = "DaveAssets/Item/Module/Socket Profile")]
    public class SocketProfile : ScriptableObject
    {
        // SO는 참조를 못하니까 상대적 위치를 계산해야함
        public OffsetLocation offset;           //CombatOffset으로 바꿔야하는데 프리팹 전부 수정하기 귀찮아서 냅둠
        public OffsetLocation handOffset;

    }


}
