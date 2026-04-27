using System;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.CharacterKit.GameFlow.Input;
using Dave6.CharacterKit.Handler.Interactor;
using Dave6.CharacterKit.Handler.Loadout;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Binder
{
    public class PlayerBinder : MonoBehaviour
    {
        // ===== Loadout =====
        #region Loadout
        ViewFactory _ViewFactory;
        LoadoutSystem _LoadoutSystem;
        PlayerLoadout _Loadout;
        LoadoutMainPanel _LoadoutUI;
        #endregion

        // ===== Interactor =====
        #region Interactor
        PlayerInteractor _Interactor;
        InteractPanel _InteractUI;
        #endregion


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
            // ===== Loadout =====
            #region Loadout
            if (_ViewFactory == null && type == typeof(ViewFactory)) _ViewFactory = (ViewFactory)instance;
            else if (_LoadoutSystem == null && type == typeof(LoadoutSystem)) _LoadoutSystem = (LoadoutSystem)instance;
            else if (_Loadout == null && type == typeof(PlayerLoadout)) _Loadout = (PlayerLoadout)instance;
            else if (_LoadoutUI == null && type == typeof(LoadoutMainPanel)) _LoadoutUI = (LoadoutMainPanel)instance;
            #endregion

            // ===== Interactor =====
            #region Interactor
            else if (_Interactor == null && type == typeof(PlayerInteractor)) _Interactor = (PlayerInteractor)instance;
            else if (_InteractUI == null && type == typeof(InteractPanel)) _InteractUI = (InteractPanel)instance;
            #endregion

            TryBind();
        }

        void TryResolveFromHub()
        {
            var hub = GameplayHub.Instance;

            // ===== Loadout =====
            #region Loadout
            if (_ViewFactory == null) _ViewFactory = hub.Get<ViewFactory>();
            if (_LoadoutSystem == null) _LoadoutSystem = hub.Get<LoadoutSystem>();
            if (_Loadout == null) _Loadout = hub.Get<PlayerLoadout>();
            if (_LoadoutUI == null) _LoadoutUI = hub.Get<LoadoutMainPanel>();
            #endregion

            // ===== Interactor =====
            #region Interactor
            if (_Interactor == null) _Interactor = hub.Get<PlayerInteractor>();
            if (_InteractUI == null) _InteractUI = hub.Get<InteractPanel>();
            #endregion

            TryBind();
        }

        void TryBind()
        {
            if (_ViewFactory == null || _LoadoutSystem == null || _Loadout == null || _LoadoutUI == null || _Interactor == null)
                return;

            // ===== Loadout Binding =====
            #region Loadout
            _LoadoutSystem.BindContext(_Loadout);
            _LoadoutUI.Bind(_Loadout, _Interactor);

            _LoadoutSystem.OnLoadComplete -= _LoadoutUI.Rebuild;
            _LoadoutSystem.OnLoadComplete += _LoadoutUI.Rebuild;
            #endregion

            // ===== Interactor Binding =====
            #region Interactor
            if (_InteractUI != null)
            {
                _Interactor.OnShowPrompt += _InteractUI.Show;
                _Interactor.OnHidePrompt += _InteractUI.Hide;
            }
            #endregion

            // ===== Input Binding =====
            #region Input
            var uiInput = FindFirstObjectByType<UIInputHandler>();
            if (uiInput != null) uiInput.SetUI(_LoadoutUI);

            var systemInput = FindFirstObjectByType<SystemInputHandler>();
            if (systemInput != null) systemInput.Inject(_LoadoutSystem.Save, _LoadoutSystem.Load);
            #endregion

            enabled = false;
            GameplayHub.Instance.OnRegistered -= HandleRegister;
        }
    }
}