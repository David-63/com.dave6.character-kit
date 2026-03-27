using Dave6.ItemSystem.Domain.Container;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class SocketContainerView : ContainerBaseView
    {
        VisualElement _Contents;
        SocketView _SocketView;
        SocketContainer _SocketContainer;

        public override void Initialize(VisualTreeAsset template)
        {
            Clear();
            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = 0;
            template.CloneTree(this);

            _Contents = this.Q<VisualElement>("socket-root");
        }

        public override void Bind(IItemContainer container)
        {
            _Container = container;
            _SocketContainer = container as SocketContainer;

            if (_SocketView != null) _Contents.Remove(_SocketView);

            var layout = _SocketContainer.SocketLayout;
            _SocketView = CreateSocketView(layout);

            _Contents.Add(_SocketView);
            _SocketView.Build(_SocketContainer);
        }
        SocketView CreateSocketView(SocketLayout type)
        {
            return type switch
            {
                SocketLayout.LabelRow => new SocketLabelRowView(),
                SocketLayout.LabelAbove => new SocketLabelAboveView(),
                _ => new SocketLabelRowView()
            };
        }
    }

    [UxmlElement]
    public abstract partial class SocketView : VisualElement
    {
        protected SocketContainer _container;

        public virtual void Build(SocketContainer container)
        {
            _container = container;
            Build();
        }
        protected abstract void Build();
    }
}
