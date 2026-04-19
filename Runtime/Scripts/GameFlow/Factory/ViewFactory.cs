using Dave6.CharacterKit.UnityUI.ItemSystem;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.GameFlow.Factory
{
    public class ViewFactory : MonoBehaviour
    {
        [SerializeField] VisualTreeAsset _ItemTemplate;
        [SerializeField] VisualTreeAsset _GridTemplate;
        [SerializeField] VisualTreeAsset _SocketTemplate;

        void Awake()
        {
            GameplayHub.Instance.Register(this);
        }

        public ItemView CreateItemView(ItemInteractionController interactionController)
        {
            var view = new ItemView();
            view.Initialize(_ItemTemplate);
            if (interactionController == null)
            {
                Debug.LogError("ItemInteractionController is null");
                return view;
            }
            view.AddManipulator(new ItemPointerManipulator(interactionController));

            return view;
        }
        public ContainerBaseView CreateContainerView(IItemContainer container, ItemInteractionController interactionController)
        {
            if (container is GridContainer)
            {
                var view = new GridContainerView();
                view.Initialize(_GridTemplate, interactionController);
                return view;
            }
            else if (container is SocketContainer)
            {
                var view = new SocketContainerView();
                view.Initialize(_SocketTemplate, interactionController);
                return view;
            }
            return null;
        }
    }

}
