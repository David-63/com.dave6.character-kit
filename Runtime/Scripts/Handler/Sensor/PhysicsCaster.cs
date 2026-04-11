using UnityEngine;

namespace Dave6.CharacterKit.Sensor
{
    public static class PhysicsCaster
    {
        public static bool Cast(Vector3 origin, Vector3 direction, CastConfig config, out RaycastHit hitInfo)
        {
            if (config.Radius > 0f)
            {
                return Physics.SphereCast(origin, config.Radius, direction, out hitInfo, config.Length, config.LayerMask, config.TriggerInteraction);
            }
            return Physics.Raycast(origin, direction, out hitInfo, config.Length, config.LayerMask, config.TriggerInteraction);
        }
    }
}