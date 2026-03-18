using UnityEngine;
namespace Dave6.CharacterKit.GameFlow
{
    /*
    1 포탈 연출
    화면 페이드
    입력 잠금
    이동 불가 처리

    2 포탈 조건
    퀘스트 완료 여부
    키 아이템
    난이도 제한

    3 씬 내부 포탈
    같은 씬, 위치만 이동
    씬 로드 없는 워프
    */
    public class Portal : MonoBehaviour, IInteractable
    {
        [SerializeField] string _TargetScene;
        [SerializeField] string _PortalId;
        public string PortalId => _PortalId;
        [SerializeField] string _ConnectId;
        public string ConnectId => _ConnectId;

        bool IsCunsumed = false;

        void Awake()
        {
            if (SceneDirector.Instance == null)
            {
                Debug.LogError("Bootstrap 씬에서 초기화 누락됨!");
                return;
            }
            SceneDirector.Instance.RegisterPortal(this);
        }

        public void Interact(IInteractor interactor)
        {
            if (IsCunsumed) return;
            IsCunsumed = true;

            SceneDirector.Instance.RequestSceneLoad(_TargetScene, _ConnectId);
        }
    }

}
