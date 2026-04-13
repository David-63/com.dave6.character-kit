using System;
using System.Collections.Generic;
using UnityUtils;

namespace Dave6.CharacterKit.GameFlow
{
    public class GameplayHub : SingletonTemplate<GameplayHub>
    {
        /// <summary>
        /// 저장소
        /// </summary>
        Dictionary<Type, object> _Registry = new();
        /// <summary>
        /// 등록 이벤트
        /// </summary>
        public event Action<Type, object> OnRegistered;

        public void Register<T>(T instance)
        {
            var type = typeof(T);
            _Registry[type] = instance;
            OnRegistered?.Invoke(type, instance);
        }
        public T Get<T>()
        {
            var type = typeof(T);
            if (_Registry.ContainsKey(type))
            {
                return (T)_Registry[type];
            }
            return default;
        }
        public object Get(Type type)
        {
            if (_Registry.ContainsKey(type))
            {
                return _Registry[type];
            }
            return default;
        }
    }
}