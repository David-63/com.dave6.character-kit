using UnityEngine;

namespace Dave6.CharacterKit.Sensor
{
    /// <summary>
    /// Cast 방향을 보관하고
    /// 상태를 저장하는 구조
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
}