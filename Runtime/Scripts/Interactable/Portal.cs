using Dave6.CharacterKit.GameFlow;
using UnityEngine;
namespace Dave6.CharacterKit.Interactable
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
    public class Portal : WorldActor
    {
        public string PortalId => _PortalId;
        public string ConnectId => _ConnectId;

        [SerializeField] string _TargetScene;
        [SerializeField] string _PortalId;
        [SerializeField] string _ConnectId;

        bool IsCunsumed = false;

        void Awake()
        {
            bool flowControl = RegisterPortal();
            if (!flowControl)
            {
                return;
            }
        }

        private bool RegisterPortal()
        {
            if (SceneDirector.Instance == null)
            {
                Debug.LogError("포탈 초기화 누락됨!");
                return false;
            }
            SceneDirector.Instance.RegisterPortal(this);
            return true;
        }

        public override bool CanInteract(IInteractor interactor)
        {
            return base.CanInteract(interactor) && !IsCunsumed;
        }

        public override string GetPromptText(IInteractor interactor)
        {
            return "[F] Enter";
        }

        protected override void OnInteract(IInteractor interactor)
        {
            IsCunsumed = true;

            SceneDirector.Instance.RequestSceneLoad(_TargetScene, _ConnectId);
        }
    }

}
