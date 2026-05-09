using System;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.GameFlow.Factory
{
    public class ViewFactory : MonoBehaviour
    {
        [SerializeField] VisualTreeAsset _CollectionTemplate;
        [SerializeField] VisualTreeAsset _ContainerTemplate;
        [SerializeField] VisualTreeAsset _ItemTemplate;
        [SerializeField] VisualTreeAsset _InspectorTemplate;

        void Awake()
        {
            GameplayHub.Instance.Register(this);
        }

        public CollectionView CreateCollectionView()
        {
            var view = new CollectionView();
            view.Initialize(_CollectionTemplate);
            return view;
        }
        public ContainerBaseView CreateContainerView(IItemContainer container)
        {
            ContainerBaseView view;

            switch (container)
            {
                case GridContainer:
                    view = new GridContainerView();
                    break;
                case SocketContainer:
                    view = new SocketContainerView();
                    break;
                default:
                    Debug.LogError($"Unsupported container type: {container.GetType()}");
                    return null;
            }
            if (view == null) throw new InvalidOperationException("Failed to create container view");
            view.Initialize(_ContainerTemplate);
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

        #region inspector api
        #endregion
    }

}
