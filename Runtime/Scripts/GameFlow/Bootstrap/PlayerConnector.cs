using System;
using System.Collections;
using System.Collections.Generic;
using Dave6.CharacterKit.Handler.Loadout;
using Dave6.CharacterKit.Inputs;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using UnityEngine;
using UnityUtils;

namespace Dave6.CharacterKit.GameFlow
{
    public class PlayerConnector : SingletonTemplate<PlayerConnector>
    {
        GameObject _PlayerInstance;
        [SerializeField] InputReader _Input;

        IInputReceiver _ActiveReceiver;
        LoadoutMainPanel _UiPanel;
        LoadoutManager _LoadoutManager;

        Dictionary<Type, IProvider> _ProviderRegistry = new();

        public bool HasPlayer => _PlayerInstance != null;

        public void RegisterTarget(IInputReceiver receiver)
        {
            _ActiveReceiver = receiver;
            if (receiver is Component instance)
            {
                _PlayerInstance = instance.gameObject;
            }
        }

        public void RegisterProvider<T>(IProvider instance) where T : IProvider
        {
            _ProviderRegistry[typeof(T)] = instance;
            Debug.Log($"[PlayerConnector] Registered provider for {typeof(T).Name}: {instance.GetType().Name}");
            TryBind();
        }

        public void SpawnPlayer(string spawnId, Action onComplete = null)
        {
            if (_PlayerInstance == null) return;

            var portal = SceneDirector.Instance.GetPortal(spawnId);

            if (portal != null)
            {
                _PlayerInstance.transform.position = portal.transform.position;
                _PlayerInstance.transform.rotation = portal.transform.rotation;

                var cc = _PlayerInstance.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.enabled = false;
                    cc.enabled = true;
                }

                Debug.Log($"[PlayerSpawner] Player spawned at: {portal.name}");
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawner] Portal ID '{spawnId}' not found. Using current position.");
            }

            _PlayerInstance.SetActive(true);

            // 한 프레임 지연 후 게임 시작 (타이밍 안정화)
            StartCoroutine(DelayedResume(onComplete));
        }

        IEnumerator DelayedResume(Action onComplete)
        {
            yield return new WaitForEndOfFrame();
            GameFlowController.Instance.ChangeState(GameState.Running);
            onComplete?.Invoke();
        }


        #region 인풋 제어
        void OnEnable()
        {
            if (_Input == null) return;
            _Input.Move += InputMove;
            _Input.Look += InputLook;
            _Input.Jump += InputJump;
            _Input.Shift += InputShift;
            _Input.Focus += InputFocus;
            _Input.Attack += InputAttack;
            _Input.Reload += InputReload;
            _Input.Interact += InputInteract;
            _Input.OpenStatus += HandleOpenStatus;
            _Input.Close += HandleClose;
        }
        void OnDisable()
        {
            UnsubscribeAll();
        }
        void UnsubscribeAll()
        {
            // 모든 이벤트를 해제하는 로직 (메모리 누수 방지)
            _Input.Move -= InputMove;
            _Input.Look -= InputLook;
            _Input.Jump -= InputJump;
            _Input.Shift -= InputShift;
            _Input.Focus -= InputFocus;
            _Input.Attack -= InputAttack;
            _Input.Reload -= InputReload;
            _Input.Interact -= InputInteract;
            _Input.OpenStatus -= HandleOpenStatus;
            _Input.Close -= HandleClose;


            _Input.Save -= _LoadoutManager.Save;
            _Input.Load -= _LoadoutManager.Load;
        }

        void InputMove(Vector2 value) => _ActiveReceiver?.OnMove(value);
        void InputLook(Vector2 value) => _ActiveReceiver?.OnLook(value);
        void InputJump(bool value) => _ActiveReceiver?.OnAction(ActionType.Jump, value);
        void InputShift(bool value) => _ActiveReceiver?.OnAction(ActionType.Shift, value);
        void InputFocus(bool value) => _ActiveReceiver?.OnAction(ActionType.Focus, value);
        void InputAttack(bool value) => _ActiveReceiver?.OnAction(ActionType.Attack, value);
        void InputReload(bool value) => _ActiveReceiver?.OnAction(ActionType.Reload, value);
        void InputInteract(bool value) => _ActiveReceiver?.OnAction(ActionType.Interact, value);

        void TryBind()
        {
            if (!_ProviderRegistry.TryGetValue(typeof(LoadoutManager), out var mObj)) return;
            if (!_ProviderRegistry.TryGetValue(typeof(PlayerLoadout), out var lObj)) return;
            if (!_ProviderRegistry.TryGetValue(typeof(LoadoutMainPanel), out var uiObj)) return;

            var loadout = (PlayerLoadout)lObj;
            _LoadoutManager = (LoadoutManager)mObj;
            _UiPanel = (LoadoutMainPanel)uiObj;

            // ---- 여기서 전부 연결 ----

            _LoadoutManager.BindContext(loadout);
            _UiPanel.Bind(loadout);

            _LoadoutManager.OnLoadComplete -= _UiPanel.Rebuild;
            _LoadoutManager.OnLoadComplete += _UiPanel.Rebuild;

            _Input.Save -= _LoadoutManager.Save;
            _Input.Save += _LoadoutManager.Save;

            _Input.Load -= _LoadoutManager.Load;
            _Input.Load += _LoadoutManager.Load;
        }
        void HandleOpenStatus(bool pressed)
        {
            if (!pressed) return;
            Debug.Log("<color=cyan>[Input] Open Status Triggered. Switching to Status Map.</color>");
            _Input.EnableStatusInput();
            
            _UiPanel.ShowUI();
        }
        void HandleClose(bool pressed)
        {
            if (!pressed) return;

            Debug.Log("<color=yellow>[Input] Close Triggered. Switching back to Character Map.</color>");

            _Input.EnableCharacterInput();
            _UiPanel.HideUI();
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Label($"<color=white>Active Receiver: {_ActiveReceiver?.GetType().Name}</color>");
            // InputReader에 현재 어떤 맵이 켜져있는지 확인하는 변수(bool)를 추가하면 더 좋음
            GUILayout.EndArea();
        }

        #endregion
    }

}
