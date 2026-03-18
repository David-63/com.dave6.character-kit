using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace Dave6.CharacterKit.Item
{
    [CreateAssetMenu(fileName = "Trail Config", menuName = "DaveAssets/Item/FireArm/Trail Config")]
    public class TrailConfig : ScriptableObject
    {
        public Material material;
        public AnimationCurve widthCurve;
        public float duration = 0.5f;
        public float minVertexDistance = 0.1f;
        public Gradient color;

        public float missDistance = 100f;
        public float simulationSpeed = 200f;
    }

    [CreateAssetMenu(fileName = "FireArm", menuName = "DaveAssets/Item/FireArm/FireArm")]
    public class FireArm : ScriptableObject
    {
        public WeaponType weaponType;
        public string fireArmName;
        public GameObject modelPrefab;
        public Vector3 SpawnPoint;
        public Vector3 SpawnRotation;

        public ShootConfig shootConfig;
        public TrailConfig trailConfig;

        MonoBehaviour m_ActiveMono;
        GameObject m_Model;
        float m_LastFireTime;
        ParticleSystem m_ShootParticle;
        ObjectPool<TrailRenderer> m_TrailPool;

        public void Spawn(Transform parent, MonoBehaviour activeMono)
        {
            m_ActiveMono = activeMono;
            m_LastFireTime = 0; // in editor this will not be properly reset, in build it's fine.

            m_TrailPool = new ObjectPool<TrailRenderer>(CreateTrail);
            m_Model = Instantiate(modelPrefab);
            m_Model.transform.SetParent(parent, false);
            m_Model.transform.localPosition = SpawnPoint;
            m_Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

            m_ShootParticle = m_Model.GetComponentInChildren<ParticleSystem>();
        }

        public void Shoot()
        {
            float fireDelay = 60f / shootConfig.FireRate;
            if (Time.time > fireDelay + m_LastFireTime)
            {
                m_LastFireTime = Time.time;
                m_ShootParticle.Play();
                Vector3 shootDirection = m_ShootParticle.transform.forward + new Vector3(
                    Random.Range(-shootConfig.Spread.x, shootConfig.Spread.x),
                    Random.Range(-shootConfig.Spread.y, shootConfig.Spread.y),
                    Random.Range(-shootConfig.Spread.z, shootConfig.Spread.z)
                );
                shootDirection.Normalize();
                if (Physics.Raycast(m_ShootParticle.transform.position, shootDirection
                    , out RaycastHit hit, float.MaxValue, shootConfig.hitMask))
                {
                    m_ActiveMono.StartCoroutine(PlayTrail(m_ShootParticle.transform.position, hit.point, hit));
                }
                else
                {
                    m_ActiveMono.StartCoroutine(PlayTrail(m_ShootParticle.transform.position, m_ShootParticle.transform.position + (shootDirection * trailConfig.missDistance), new RaycastHit()));
                }
            }
        }

        TrailRenderer CreateTrail()
        {
            GameObject instance = new GameObject("Bullet trail");
            TrailRenderer trail = instance.AddComponent<TrailRenderer>();
            trail.colorGradient = trailConfig.color;
            trail.material = trailConfig.material;
            trail.widthCurve = trailConfig.widthCurve;
            trail.time = trailConfig.duration;
            trail.minVertexDistance = trailConfig.minVertexDistance;
            trail.emitting = false;
            trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return trail;
        }

        IEnumerator PlayTrail(Vector3 startPoint, Vector3 endPoint, RaycastHit hit)
        {
            TrailRenderer instance = m_TrailPool.Get();
            instance.gameObject.SetActive(true);
            instance.transform.position = startPoint;
            yield return null; // avoid position carry-over from last f rame if reused.

            instance.emitting = true;
            float distance = Vector3.Distance(startPoint, endPoint);
            float remainingDistance = distance;
            while (remainingDistance > 0)
            {
                instance.transform.position = Vector3.Lerp(startPoint, endPoint, Mathf.Clamp01(1 - (remainingDistance / distance)));
                remainingDistance -= trailConfig.simulationSpeed + Time.deltaTime;

                yield return null;
            }

            instance.transform.position = endPoint;

            if (hit.collider != null)
            {
                //SurfaceManager
            }
            yield return new WaitForSeconds(trailConfig.duration);
            yield return null;
            instance.emitting = false;
            instance.gameObject.SetActive(false);
            m_TrailPool.Release(instance);
        }
    }
}
