using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class ProjectileMover : MonoBehaviour, IStatInvoker
    {
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;

        [SerializeField] float m_MoveSpeed = 10f;
        IEntity m_Actor;
        public IEntity actor => m_Actor;
        Vector3 m_PreviousPosition;

        void Start()
        {
            // 위치 초기화
            m_PreviousPosition = transform.position;
        }

        public void Initialize(IEntity actorEntity)
        {
            m_Actor = actorEntity;
        }
        // Update is called once per frame
        void Update()
        {
            transform.Translate(Vector3.forward * m_MoveSpeed * Time.deltaTime);
        }
        void LateUpdate()
        {
            Vector3 rayDirection = transform.position - m_PreviousPosition;
            float rayDistance = rayDirection.magnitude;
            RaycastHit hit;
            if (Physics.Raycast(m_PreviousPosition, rayDirection.normalized, out hit, rayDistance))
            {
                // 여기서 충돌 로직 처리

                if (hit.collider.TryGetComponent<IStatReceiver>(out var entity))
                {
                    entity.Accept(this);
                }
            }

            m_PreviousPosition = transform.position;
        }


        public void Invoke<T>(T target) where T : Component, IStatReceiver
        {
            IEntity entity = target as IEntity;
            var stat = entity.statHandler.GetHealthStat();

            m_Actor.statHandler.ApplyEffect(effectDefinition, stat);
            Debug.Log($"target Helth: {stat.currentValue}/{stat.finalValue}");

            if (stat.currentValue <= 0)
            {
                Destroy(target.gameObject);
            }

            Destroy(gameObject);
        }


    }
}
