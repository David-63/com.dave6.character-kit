using System;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.CharacterKit.GameFlow.Input;
using Dave6.CharacterKit.Handler.Interactor;
using Dave6.CharacterKit.Handler.Loadout;
using Dave6.CharacterKit.Handler.Stat;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow.Binder
{
    /// <summary>
    /// Runtime Composition Layer
    /// </summary>
    public class PlayerBinder : MonoBehaviour
    {
        // ===== Loadout =====
        #region Loadout
        ViewFactory _ViewFactory;
        LoadoutSystem _LoadoutSystem;
        PlayerLoadout _Loadout;
        LoadoutMain _LoadoutUI;
        #endregion
        #region Item Inspector
        ItemInspector _ItemInspector;
        #endregion

        // ===== Interactor =====
        #region Interactor
        PlayerInteractor _Interactor;
        InteractPanel _InteractUI;
        #endregion

        // ===== Stat =====
        #region Stat
        PlayerStat _PlayerStat;
        ItemStatApplier _ItemStatApplier;
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
            else if (_LoadoutUI == null && type == typeof(LoadoutMain)) _LoadoutUI = (LoadoutMain)instance;
            #endregion

            // ===== Item Inspector =====
            #region Item Inspector
            else if (_ItemInspector == null && type == typeof(ItemInspector)) _ItemInspector = (ItemInspector)instance;
            #endregion

            // ===== Interactor =====
            #region Interactor
            else if (_Interactor == null && type == typeof(PlayerInteractor)) _Interactor = (PlayerInteractor)instance;
            else if (_InteractUI == null && type == typeof(InteractPanel)) _InteractUI = (InteractPanel)instance;
            #endregion

            // ===== Stat =====
            #region Stat
            else if (_PlayerStat == null && type == typeof(PlayerStat)) _PlayerStat = (PlayerStat)instance;
            else if (_ItemStatApplier == null && type == typeof(ItemStatApplier)) _ItemStatApplier = (ItemStatApplier)instance;
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
            if (_LoadoutUI == null) _LoadoutUI = hub.Get<LoadoutMain>();
            #endregion

            // ===== Interactor =====
            #region Interactor
            if (_Interactor == null) _Interactor = hub.Get<PlayerInteractor>();
            if (_InteractUI == null) _InteractUI = hub.Get<InteractPanel>();
            #endregion

            // ===== Stat =====
            #region Stat
            if (_PlayerStat == null) _PlayerStat = hub.Get<PlayerStat>();
            if (_ItemStatApplier == null) _ItemStatApplier = hub.Get<ItemStatApplier>();
            #endregion

            // ===== Inspector =====
            #region Inspector
            if (_ItemInspector == null) _ItemInspector = hub.Get<ItemInspector>();
            #endregion

            TryBind();
        }

        void TryBind()
        {
            if (!CanBind()) return;

            BindLoadout();
            BindInteractor();
            BindInput();
            BindStat();

            FinishBinding();
        }

        void HandleEquipChanged(ItemInstance item, IItemContainer container)
        {
            if (_Loadout.IsItemInEquipment(item)) _ItemStatApplier.ApplyItem(_PlayerStat.StatController, item);
            else _ItemStatApplier.RemoveItem(_PlayerStat.StatController, item);
        }

        void HandleEquipRemoved(ItemInstance item, IItemContainer container)
        {
            _ItemStatApplier.RemoveItem(_PlayerStat.StatController, item);
        }
        void HandleInspect(ItemInstance item)
        {
            _ItemInspector.Bind(item);
            _ItemInspector.Show();
        }

        bool CanBind()
        {
            return _ViewFactory != null && _LoadoutSystem != null && _Loadout != null && _LoadoutUI != null && _Interactor != null &&
             _ItemStatApplier != null && _PlayerStat != null && _ItemInspector != null;
        }
        void BindLoadout()
        {
            _LoadoutSystem.BindContext(_Loadout);
            _LoadoutUI.Bind(_Loadout, _Interactor);
            _LoadoutUI.OnInspectRequested += HandleInspect;

            _LoadoutSystem.OnLoadComplete -= _LoadoutUI.Rebuild;
            _LoadoutSystem.OnLoadComplete += _LoadoutUI.Rebuild;

            var loadoutCtx = _Loadout.GetContext();
            loadoutCtx.OnItemAdded += HandleEquipChanged;
            loadoutCtx.OnItemMoved += HandleEquipChanged;
            loadoutCtx.OnItemRemoved += HandleEquipRemoved;
        }
        void BindInteractor()
        {
            _Interactor.OnShowPrompt += _InteractUI.Show;
            _Interactor.OnHidePrompt += _InteractUI.Hide;
        }
        void BindInput()
        {
            var uiInput = FindFirstObjectByType<UIInputHandler>();
            if (uiInput == null) Debug.LogWarning("UIInputHandler not found");
            
            uiInput.Inject(_LoadoutUI, _ItemInspector);
            uiInput.InputBind();


            var systemInput = FindFirstObjectByType<SystemInputHandler>();
            if (systemInput == null) Debug.LogWarning("SystemInputHandler not found");
            systemInput.Inject(_LoadoutSystem.Save, _LoadoutSystem.Load);
        }
        void BindStat()
        {
            // todo
        }
        void FinishBinding()
        {
            Debug.Log("플레이어 바인딩 완료");
            enabled = false;
            GameplayHub.Instance.OnRegistered -= HandleRegister;
        }
    }
}