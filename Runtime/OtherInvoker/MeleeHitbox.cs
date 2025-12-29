using Dave6.StatSystem;
using Dave6.StatSystem.Effect;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class MeleeHitbox : MonoBehaviour, IStatInvoker
    {
        [Header("충돌체 세팅")]
        [SerializeField] EffectDefinition m_EffectDefinition;
        public EffectDefinition effectDefinition => m_EffectDefinition;
        public IStatController actor { get; private set; }

        [SerializeField] StatTag healthStatTag;

        /// <summary>
        /// 생성될 때, 스텟을 받도록 할수도 있음
        /// </summary>
        public void Initialize(IStatController actorEntity)
        {
            actor = actorEntity;
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

            entity.statHandler.TryGetStat(healthStatTag, out var targetHealth);

            // 본인 스탯을 상대에게 때려박기
            actor.statHandler.CreateEffectInstance(effectDefinition, targetHealth);
        }
    }
}