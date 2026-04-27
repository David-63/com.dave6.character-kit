using Dave6.CharacterKit.UnityUI.ItemSystem;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.GameFlow.Factory
{
    public class ViewFactory : MonoBehaviour
    {
        [SerializeField] VisualTreeAsset _CollectionTemplate;
        [SerializeField] VisualTreeAsset _GridTemplate;
        [SerializeField] VisualTreeAsset _SocketTemplate;
        [SerializeField] VisualTreeAsset _ItemTemplate;

        void Awake()
        {
            GameplayHub.Instance.Register(this);
        }

        public ContainerCollectionView CreateCollectionView()
        {
            var view = new ContainerCollectionView();
            view.Initialize(_CollectionTemplate);
            return view;
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
        public ContainerBaseView CreateContainerView(IItemContainer container)
        {
            if (container is GridContainer)
            {
                var view = new GridContainerView();
                view.Initialize(_GridTemplate);
                return view;
            }
            else if (container is SocketContainer)
            {
                var view = new SocketContainerView();
                view.Initialize(_SocketTemplate);
                return view;
            }
            return null;
        }
    }

}
