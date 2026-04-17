using System;
using Dave6.CharacterKit.Handler.Interactor;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Binder
{
    public class InteractBinder : MonoBehaviour
    {
        PlayerInteractor _Interactor;
        InteractPanel _UI;

        void OnEnable()
        {
            if (GameplayHub.Instance == null)
            {
                Debug.LogError("GameplayHub not ready");
                return;
            }
            GameplayHub.Instance.OnRegistered += HandleRegister;
            TryResolveFromHub();
        }
        void OnDisable()
        {
            if (enabled == false) return;
            GameplayHub.Instance.OnRegistered -= HandleRegister;
        }

        void HandleRegister(Type type, object instance)
        {
            if (_Interactor == null && type == typeof(PlayerInteractor)) _Interactor = (PlayerInteractor)instance;
            else if (_UI == null && type == typeof(InteractPanel)) _UI = (InteractPanel)instance;

            TryBind();
        }
        void TryResolveFromHub()
        {
            var hub = GameplayHub.Instance;
            
            if (_Interactor == null) _Interactor = hub.Get<PlayerInteractor>();
            if (_UI == null) _UI = hub.Get<InteractPanel>();

            TryBind();
        }
        void TryBind()
        {
            if (_Interactor == null || _UI == null) return;

            //주입받을건 없고 그냥 이벤트 바인드로 함수 호출해주는게 최선같은데

            _Interactor.OnShowPrompt += _UI.Show;
            _Interactor.OnHidePrompt += _UI.Hide;

            enabled = false;

            GameplayHub.Instance.OnRegistered -= HandleRegister;
        }
    }
}