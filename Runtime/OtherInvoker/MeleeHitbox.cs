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
        IStatController m_Actor;
        public IStatController actor => m_Actor;

        /// <summary>
        /// 생성될 때, 스텟을 받도록 할수도 있음
        /// </summary>
        public void Initialize(IStatController actorEntity)
        {
            m_Actor = actorEntity;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IStatReceiver>(out var entity))
            {
                Debug.Log("Hit Someone");
                entity.Accept(this);
            }
        }

        public void Invoke<T>(T target) where T : Component, IStatReceiver
        {
            // 상대 스탯 가져오기
            IStatController entity = target as IStatController;
            var stat = entity.statHandler.GetHealthStat();

            // 본인 스탯을 상대에게 때려박기
            m_Actor.statHandler.ApplyEffect(effectDefinition, stat);
            Debug.Log($"target Helth: {stat.currentValue}/{stat.finalValue}");

            entity.CheckHealth();
        }
    }
}