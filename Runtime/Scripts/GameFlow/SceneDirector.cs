using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityUtils;

namespace Dave6.CharacterKit.GameFlow
{
    // SceneDirector.instance.RequestSceneLoad("GamePlayCore", "LobbyEnter");
    public class SceneDirector : SingletonTemplate<SceneDirector>
    {
        public event UnityAction<string> onSceneFullyEntered;
        string _NextSpawnId;
        string _PrevMap;
        Dictionary<string, Portal> _PortalCache = new Dictionary<string, Portal>();
        public List<Portal> ListPortal = new(); // 디버깅용

        protected override void Awake()
        {
            base.Awake();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void InitialGameplayCoreLoad()
        {
            StartCoroutine(InitialBootCoroutine());
        }

        /// <summary>
        /// Additive 방식으로 Map 불러오기
        /// </summary>
        public void RequestMapLoad(string name, string spawnId = null)
        {
            _NextSpawnId = spawnId;
            GameFlowController.Instance.ChangeState(GameState.Loading);
            // TODO
            StartCoroutine(ChangeMapCoroutine(name));
        }

        public void RequestSceneLoad(string name, string spawnId = null)
        {
            _NextSpawnId = spawnId;
            GameFlowController.Instance.ChangeState(GameState.Loading);
            StartCoroutine(LoadSceneCoroutine(name));
        }

        IEnumerator LoadSceneCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;
            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }

        IEnumerator InitialBootCoroutine()
        {
            yield return LoadSceneCoroutine("GamePlayCore");
            yield return new WaitUntil(() => PlayerConnector.Instance.HasPlayer);
            _NextSpawnId = "LobbyEnter";
            yield return LoadSceneCoroutine("Lobby");
            PlayerConnector.Instance.SpawnPlayer(_NextSpawnId, () =>
            {
                onSceneFullyEntered?.Invoke("Lobby");
            });
            _PrevMap = "Lobby";
        }

        IEnumerator ChangeMapCoroutine(string sceneName)
        {
            if (!string.IsNullOrEmpty(_PrevMap))
            {
                yield return SceneManager.UnloadSceneAsync(_PrevMap);
            }
            yield return LoadSceneCoroutine(sceneName);

            PlayerConnector.Instance.SpawnPlayer(_NextSpawnId, () =>
            {
                onSceneFullyEntered?.Invoke(sceneName);
            });
            _PrevMap = sceneName;
        }


        public void RegisterPortal(Portal portal)
        {
            if (!_PortalCache.ContainsKey(portal.PortalId))
                _PortalCache.Add(portal.PortalId, portal);
            ListPortal.Add(portal);
        }

        public Portal GetPortal(string id)
        {
            _PortalCache.TryGetValue(id, out var portal);
            return portal;
        }
        public void ClearPortals() => _PortalCache.Clear();
    }
}
