using System;
using System.Collections;
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
        IContainerProvider _CachedLoadout;
        LoadoutManager _LoadoutManager;


        public bool HasPlayer => _PlayerInstance != null;

        public void RegisterTarget(IInputReceiver receiver)
        {
            _ActiveReceiver = receiver;
            if (receiver is Component instance)
            {
                _PlayerInstance = instance.gameObject;
            }
        }

        public void RegisterLoadoutManager(LoadoutManager manager)
        {
            _LoadoutManager = manager;            
            _Input.Save += _LoadoutManager.Save;
            _Input.Load += _LoadoutManager.Load;
        }
        public void RegisterLoadout(PlayerLoadout loadout)
        {
            _CachedLoadout = loadout;
            _LoadoutManager.BindContext(_CachedLoadout);
        }
        public void RegisterLoadoutUI(LoadoutMainPanel uiPanel) => _UiPanel = uiPanel;

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

        public void InputBindLoadout()
        {

        }
        public void InputBindLoad()
        {
            
        }

        void HandleOpenStatus(bool pressed)
        {
            if (!pressed) return;
            Debug.Log("<color=cyan>[Input] Open Status Triggered. Switching to Status Map.</color>");
            _Input.EnableStatusInput();
            _UiPanel.Bind(_PlayerInstance.GetComponent<PlayerLoadout>());
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
