using UnityEngine;

namespace Dave6.CharacterKit.Sensor
{
    public class CastConfig
    {
        public float Length = 1f;
        public float Radius = 0f;
        public LayerMask LayerMask = ~0;
        public QueryTriggerInteraction TriggerInteraction = QueryTriggerInteraction.Ignore;
    }
}