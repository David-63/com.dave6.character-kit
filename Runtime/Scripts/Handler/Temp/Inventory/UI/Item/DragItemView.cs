using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit
{
    [UxmlElement]
    public partial class DragItemView : VisualElement
    {
        public ItemInstance instance { get; private set; }
        VisualElement itemRoot;
        Image itemIcon;

        InventoryController m_Controller;

        public void Initialize(VisualTreeAsset template, InventoryController controller, ItemInstance instance, Texture image)
        {
            Clear();
            template.CloneTree(this);

            this.instance = instance;

            itemRoot = this.Q<VisualElement>("item-root");
            itemIcon = itemRoot.Q<Image>("item-icon");
            
            itemIcon.image = image;

            style.position = Position.Absolute;
            style.width = 64 * instance.Definition.ItemSize.X;
            style.height = 64 * instance.Definition.ItemSize.Y;
            style.backgroundColor = new Color(0.3f, 0.6f, 0.9f, 0.6f);

            m_Controller = controller;
        }

        public void RefreshFromPlacement()
        {
            // controller에 등록된 placement 정보 가져옴
            var placement = m_Controller.GetPlacement(instance);
            // 등록된 정보로 덮어쓰기
            var pos = placement.space.GridToPanelPositionLeftTop(placement.origin, parent);
            SetPosition(pos);
        }
        public void OnPlaceChanged(ItemInstance item)
        {
            if (item != instance) return;
            RefreshFromPlacement();
        }

        public void SetPosition(Vector2 pos)
        {
            style.left = pos.x;
            style.top = pos.y;
        }
    }
}
