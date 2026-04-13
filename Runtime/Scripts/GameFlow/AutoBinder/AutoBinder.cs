using System;
using System.Reflection;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.AutoBinder
{
    public static class AutoBinder
    {
        public static void Bind(object target)
        {
            var hub = GameplayHub.Instance;

            var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var field in fields)
            {
                if (!Attribute.IsDefined(field, typeof(BindAttribute))) continue;

                var value = hub.Get(field.FieldType);

                if (value == null)
                {
                    Debug.LogWarning($"[AutoBinder] Missing binding: {field.FieldType.Name}");
                    continue;
                }
                field.SetValue(target, value);
            }

            // OnBind 자동 호출
            var method = target.GetType().GetMethod("OnBind", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            method?.Invoke(target, null);
        }
    }

    public abstract class AutoBindableMono : MonoBehaviour
    {
        protected virtual void Awake()
        {
            AutoBinder.Bind(this);
        }
    }
}