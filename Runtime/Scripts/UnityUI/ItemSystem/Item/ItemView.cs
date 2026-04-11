using Dave6.Foundation.Collections;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class ItemView : VisualElement
    {
        ItemInstance _Item;

        public ItemInstance GetItem() => _Item;

        public void Initialize(VisualTreeAsset template)
        {
            if (template != null)
            {
                template.CloneTree(this);
            }
            style.position = Position.Absolute;
            style.width = 64f;
            style.height = 64f;
            style.backgroundColor = new Color(1,1,1,0.2f);
        }
        public void Bind(ItemInstance item)
        {
            _Item = item;
            style.width = _Item.Definition.ItemSize.X * 64f;
            style.height = _Item.Definition.ItemSize.Y * 64f;
        }

        #region API
        public Vector2 GetLeftTop()
        {
            return new Vector2(worldBound.xMin, worldBound.yMin);
        }
        public Rect GetItemWorldArea()
        {
            return new Rect(worldBound.position, worldBound.size);
        }
        #endregion
    }
}