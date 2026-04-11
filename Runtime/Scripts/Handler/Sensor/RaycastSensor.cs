using UnityEngine;

namespace Dave6.CharacterKit.Sensor
{
    /// <summary>
    /// 리펙토링 예정
    /// Config | 어떻게 쏠 것인가
    /// Resolver | 어디서 어느 방향으로 쏠 것인가
    /// Executor | 실제 Physics 호출
    /// </summary>
    public class RaycastSensor
    {
        public float CastLength = 1f;
        public LayerMask Layermask = 255;

        Vector3 Origin = Vector3.zero;
        Transform CharacterTransform;
        public enum CastDirections { Forward, Right, Up, Backward, Left, Down }
        CastDirections CastDirection;
        float Radius;

        RaycastHit HitInfo;
        public RaycastSensor(Transform transform) => CharacterTransform = transform;

        public void Cast()
        {
            Vector3 worldOrigin = CharacterTransform.TransformPoint(Origin);
            Vector3 worldDirection = GetCastDirection();

            Physics.Raycast(worldOrigin, worldDirection, out HitInfo, CastLength, Layermask, QueryTriggerInteraction.Ignore);
        }
        public void SphereCast()
        {
            Vector3 worldOrigin = CharacterTransform.TransformPoint(Origin);
            Vector3 worldDirection = GetCastDirection();
            Physics.SphereCast(worldOrigin, Radius, worldDirection, out HitInfo, CastLength, Layermask);
        }

        public bool HasDetecteHit() => HitInfo.collider != null;
        public float GetDistance() => HitInfo.distance;
        public Vector3 GetNormal() => HitInfo.normal;
        public Vector3 GetPosition() => HitInfo.point;
        public Collider GetCollider() => HitInfo.collider;
        public Transform GetTransform() => HitInfo.transform;

        public void SetCastDirection(CastDirections direction) => CastDirection = direction;
        public void SetCastOrigin(Vector3 pos) => Origin = CharacterTransform.InverseTransformPoint(pos);
        public void SetRadius(float value) => Radius = value;

        Vector3 GetCastDirection()
        {
            return CastDirection switch
            {
                CastDirections.Forward => CharacterTransform.forward,
                CastDirections.Right => CharacterTransform.right,
                CastDirections.Up => CharacterTransform.up,
                CastDirections.Backward => -CharacterTransform.forward,
                CastDirections.Left => -CharacterTransform.right,
                CastDirections.Down => -CharacterTransform.up,
                _ => Vector3.one
            };
        }
    }

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
        public LayerMask Layermask
        {
            get => _Config.LayerMask;
            set => _Config.LayerMask = value;
        }

        readonly CastConfig _Config = new();
        readonly CastResolver _Resolver = new();

        Vector3 _Direction;
        RaycastHit _HitInfo;

        public RaycastSensor2(Transform root) => _Resolver.Root = root;

        public bool Cast() => PhysicsCaster.Cast(_Resolver.ResolveOrigin(), _Direction, _Config, out _HitInfo);
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