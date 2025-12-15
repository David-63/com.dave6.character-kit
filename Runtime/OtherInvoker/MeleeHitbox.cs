using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class MeleeHitbox : MonoBehaviour, IStatInvoker
    {
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;

        // 원래는 플레이어가 생성하면서 초기화 함수로 본인을 넣어줘야함
        [SerializeField] IEntity m_Actor;
        public IEntity actor => m_Actor;

        // 나중에 공격 배율 추가하기 (Effect에서 factor 요소 추가하면 될듯)
        float m_Factor;

        /// <summary>
        /// 생성될 때, 스텟을 받도록 할수도 있음
        /// </summary>
        public void Initialize(IEntity actorEntity)
        {
            m_Actor = actorEntity;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IStatReceiver>(out var entity))
            {
                entity.Accept(this);
            }
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
        }
    }
}