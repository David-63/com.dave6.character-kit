using UnityEngine;

namespace Dave6.CharacterKit.Sensor
{
    public class RaycastSensor
    {
        public float castLength = 1f;
        public LayerMask layermask = 255;

        Vector3 origin = Vector3.zero;
        Transform characterTransform;
        public enum CastDirection { Forward, Right, Up, Backward, Left, Down }
        CastDirection castDirection;

        RaycastHit hitInfo;
        public RaycastSensor(Transform transform) => characterTransform = transform;

        public void Cast()
        {
            Vector3 worldOrigin = characterTransform.TransformPoint(origin);
            Vector3 worldDirection = GetCastDirection();

            Physics.Raycast(worldOrigin, worldDirection, out hitInfo, castLength, layermask, QueryTriggerInteraction.Ignore);
        }

        public bool HasDetecteHit() => hitInfo.collider != null;
        public float GetDistance() => hitInfo.distance;
        public Vector3 GetNormal() => hitInfo.normal;
        public Vector3 GetPosition() => hitInfo.point;
        public Collider GetCollider() => hitInfo.collider;
        public Transform GetTransform() => hitInfo.transform;

        public void SetCastDirection(CastDirection direction) => castDirection = direction;
        public void SetCastOrigin(Vector3 pos) => origin = characterTransform.InverseTransformPoint(pos);

        Vector3 GetCastDirection()
        {
            return castDirection switch
            {
                CastDirection.Forward => characterTransform.forward,
                CastDirection.Right => characterTransform.right,
                CastDirection.Up => characterTransform.up,
                CastDirection.Backward => characterTransform.forward,
                CastDirection.Left => -characterTransform.right,
                CastDirection.Down => -characterTransform.up,
                _ => Vector3.one
            };
        }
    }
}