using UnityEngine;

namespace Dave6.CharacterKit.Sensor
{
    /// <summary>
    /// 위랑 다른점
    /// direction 방향이 갱신 안됨
    /// </summary>
    public class RaycastSensor2
    {
        public float CastLength
        {
            get => _Config.Length;
            set => _Config.Length = value;
        }
        public float CastRadius
        {
            get => _Config.Radius;
            set => _Config.Radius = value;
        }
        public LayerMask Layermask
        {
            get => _Config.LayerMask;
            set => _Config.LayerMask = value;
        }
        public QueryTriggerInteraction TriggerInteraction
        {
            get => _Config.TriggerInteraction;
            set => _Config.TriggerInteraction = value;
        }

        readonly CastConfig _Config = new();
        readonly CastResolver _Resolver = new();

        Vector3 _Direction;
        RaycastHit _HitInfo;

        public RaycastSensor2(Transform root) => _Resolver.Root = root;

        public bool Cast()
        {
            var hit = PhysicsCaster.Cast(_Resolver.ResolveOrigin(), _Direction, _Config, out _HitInfo);
            if (hit == false) _HitInfo = default;
            return hit;
        }
        public bool HasDetecteHit() => _HitInfo.collider != null;
        public float GetDistance() => _HitInfo.distance;
        public Vector3 GetNormal() => _HitInfo.normal;
        public Vector3 GetPosition() => _HitInfo.point;
        public Collider GetCollider() => _HitInfo.collider;
        public void SetRadius(float value) => _Config.Radius = value;
        public void SetCastOrigin(Vector3 worldPos) => _Resolver.LocalOrigin = _Resolver.Root.InverseTransformPoint(worldPos);
        public void SetCastDirection(Vector3 direction) => _Direction = direction.normalized;
    
    }
}