using UnityEngine;

namespace Dave6.CharacterKit.Sensor
{
    public class CastResolver
    {
        public Transform Root;
        public Vector3 LocalOrigin;

        public Vector3 ResolveOrigin() => Root.TransformPoint(LocalOrigin);
    }
}