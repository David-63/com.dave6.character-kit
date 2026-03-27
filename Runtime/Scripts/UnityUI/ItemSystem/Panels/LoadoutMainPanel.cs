using Dave6.CharacterKit.GameFlow;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public class LoadoutMainPanel : MonoBehaviour
    {
        VisualElement _Root;
        IContainerProvider _Provider;

        [Header("Visual Elements")]
        [SerializeField] VisualTreeAsset _GridContainer;
        [SerializeField] VisualTreeAsset _SocketContainer;

        VisualElement _ContentsContainer;
        VisualElement _DragLayer;

        void Awake()
        {
            var doc = GetComponent<UIDocument>();
            _Root = doc.rootVisualElement.Q<VisualElement>("main-root");
            Initialize();
            PlayerConnector.Instance.RegisterLoadoutUI(this);
        }

        void Initialize()
        {
            _ContentsContainer = _Root.Q<VisualElement>("contents-container");

            _DragLayer = _Root.Q<VisualElement>("drag-layer");
            _DragLayer.pickingMode = PickingMode.Ignore;
            _DragLayer.style.position = Position.Absolute;
            _DragLayer.style.top = 0;
            _DragLayer.style.bottom = 0;
            _DragLayer.style.left = 0;
            _DragLayer.style.right = 0;

            HideUI();
        }

        public void 
        Bind(IContainerProvider provider)
        {
            if (_Provider == provider) return;
            _Provider = provider;
        }

        public void ShowUI()
        {
            _Root.style.display = DisplayStyle.Flex;
            //_Root.style.visibility = Visibility.Visible;
            RefreshView();
            //SetRootContainerView();
        }

        public void HideUI()
        {
            _Root.style.display = DisplayStyle.None;
            //_Root.style.visibility = Visibility.Hidden;
        }


        // dirty 플래그를 통해서 호출하도록 하기¿
        void RefreshView()
        {
            // Awake에서 찾은 참조가 null이거나 파괴되었을 수 있음
            if (_ContentsContainer == null) _ContentsContainer = _Root.Q<VisualElement>("contents-container");

            _ContentsContainer.Clear();

            if (_Provider == null) return;

            // 컨테이너 생성
            foreach (var container in _Provider.GetRootContainers())
            {
                ContainerBaseView view = container switch
                {
                    GridContainer => new GridContainerView(),
                    SocketContainer => new SocketContainerView(),
                    _ => null
                };
                if (view != null)
                {
                    _ContentsContainer.Add(view);
                    view.Initialize(container is GridContainer ? _GridContainer : _SocketContainer);
                    view.Bind(container);
                }
            }
        }
    }
}
