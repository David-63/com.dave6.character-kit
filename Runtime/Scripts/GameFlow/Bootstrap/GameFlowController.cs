using UnityEngine;
using UnityEngine.Events;
using UnityUtils;

namespace Dave6.CharacterKit.GameFlow
{
    public enum GameState
    {
        Boot,       // 게임 초기화

        Stopped,    // 시작메뉴 조작가능
        Loading,    // 씬 준비
        Running,    // 플레이 중
        Paused,     // 일시정지
    }


    public class GameFlowController : SingletonTemplate<GameFlowController>
    {
        public bool ShowDebugLogs = false;
        public event UnityAction<GameState, GameState> OnStateChanged;
        public GameState CurrentState { get; private set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        void Start()
        {
            SceneDirector.Instance.InitialGameplayCoreLoad();
            //Newtonsoft
        }
        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            var previous = CurrentState;
            CurrentState = newState;

            if (ShowDebugLogs) Debug.Log($"[GameFlow] State: {previous} → {newState}");

            // TODO: 상태 진입/퇴장 핸들링 추가
            OnStateChanged?.Invoke(previous, newState);
        }

        #region 커서 및 상태 관리
        public void CursorLock() => Cursor.lockState = CursorLockMode.Locked;
        public void CursorUnlock() => Cursor.lockState = CursorLockMode.None;

        public void PauseGame()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public void ResumeGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        #endregion

    }

}
