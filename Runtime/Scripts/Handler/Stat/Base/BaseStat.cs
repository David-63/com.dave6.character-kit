using System;
using System.Collections;
using System.Collections.Generic;
using Dave6.StatSystem2.Application;
using Dave6.StatSystem2.Domain;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Stats
{
    public abstract class BaseStat : MonoBehaviour
    {
        [SerializeField] protected List<StatTag> _StatTags;
        [SerializeField] protected List<StatGroup> _StatGroup;
        public StatController StatController { get; protected set; }
        public event Action<StatTag, float> OnStatChanged;

        public void Awake()
        {
            StatController = new StatController();
            Initialize();
            StatController.OnStatChanged += HandleStatChanged;
        }

        protected abstract void Initialize();

        public bool TryGetStatValue(StatTag tag, out StatValue stat)
        {
            return StatController.TryGetStatValue(tag, out stat);
        }
        public IEnumerable<StatGroup> GetStatGroups() => _StatGroup;

        void HandleStatChanged(StatTag tag, float value)
        {
            OnStatChanged?.Invoke(tag, value);
        }
    }
}
