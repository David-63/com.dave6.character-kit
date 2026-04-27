using System;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.CharacterKit.GameFlow.Input;
using Dave6.CharacterKit.Handler.Loadout;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Binder
{
    public class LoadoutBinder : MonoBehaviour
    {
        ViewFactory _ViewFactory;
        LoadoutSystem _Manager;
        PlayerLoadout _Loadout;
        LoadoutMainPanel _UI;

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
            if (_ViewFactory == null && type == typeof(ViewFactory)) _ViewFactory = (ViewFactory)instance;
            else if (_Manager == null && type == typeof(LoadoutSystem)) _Manager = (LoadoutSystem)instance;
            else if (_Loadout == null && type == typeof(PlayerLoadout)) _Loadout = (PlayerLoadout)instance;
            else if (_UI == null && type == typeof(LoadoutMainPanel)) _UI = (LoadoutMainPanel)instance;

            TryBind();
        }
        void TryResolveFromHub()
        {
            var hub = GameplayHub.Instance;
            
            if (_ViewFactory == null) _ViewFactory = hub.Get<ViewFactory>();
            if (_Manager == null) _Manager = hub.Get<LoadoutSystem>();
            if (_Loadout == null) _Loadout = hub.Get<PlayerLoadout>();
            if (_UI == null) _UI = hub.Get<LoadoutMainPanel>();

            TryBind();
        }
        void TryBind()
        {
            if (_ViewFactory == null || _Manager == null || _Loadout == null || _UI == null) return;

            _Manager.BindContext(_Loadout);
            //_UI.Bind(_Loadout);

            _Manager.OnLoadComplete -= _UI.Rebuild;
            _Manager.OnLoadComplete += _UI.Rebuild;
            enabled = false;

            var uiInput = FindFirstObjectByType<UIInputHandler>();
            uiInput.SetUI(_UI);
            var systemInput = FindFirstObjectByType<SystemInputHandler>();
            systemInput.Inject(_Manager.Save, _Manager.Load);

            GameplayHub.Instance.OnRegistered -= HandleRegister;
        }
    }
}