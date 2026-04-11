using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    /// <summary>
    /// 이걸 싱글톤으로?
    /// </summary>
    public class ViewFactory : SingletonTemplate<ViewFactory>
    {
        [SerializeField] VisualTreeAsset _ItemTemplate;
        [SerializeField] VisualTreeAsset _GridTemplate;
        [SerializeField] VisualTreeAsset _SocketTemplate;

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
