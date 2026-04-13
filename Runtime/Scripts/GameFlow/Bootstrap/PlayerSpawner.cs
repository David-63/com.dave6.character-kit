using System;
using System.Collections;
using UnityEngine;
using UnityUtils;

namespace Dave6.CharacterKit.GameFlow
{
    public class PlayerSpawner : SingletonTemplate<PlayerSpawner>
    {
        GameObject _Player;
        public bool HasPlayer => _Player != null;

        public void SetPlayer(GameObject player) => _Player = player;
        public void Spawn(string spawnId, Action onComplete = null)
        {
            var portal = SceneDirector.Instance.GetPortal(spawnId);

            if (portal != null)
            {
                _Player.transform.SetPositionAndRotation(portal.transform.position, portal.transform.rotation);
                var cc = _Player.GetComponent<CharacterController>();
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

            // 나중에는 여기서 하는게 아니라 플레이어가 스스로 하겟금 API 제공
            _Player.gameObject.SetActive(true);

            // 한 프레임 지연 후 게임 시작 (타이밍 안정화)
            StartCoroutine(DelayedResume(onComplete));
        }
        IEnumerator DelayedResume(Action onComplete)
        {
            yield return new WaitForEndOfFrame();
            onComplete?.Invoke();
        }
    }
}