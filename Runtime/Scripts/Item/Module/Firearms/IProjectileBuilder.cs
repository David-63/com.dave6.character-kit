using UnityEngine;

namespace Dave6.CharacterKit.Item
{
    public interface IProjectileBuilder
    {
        Transform GetMuzzle();
        void BuildProjectile(ProjectileMover projectile, Vector3 targetPoint);
    }
}
