using System;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class ItemView : VisualElement
    {
        ItemInstance _Item;

        VisualElement _Root;
        Image _ItemImage;

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


            _Root = this.Q<VisualElement>("item-root");
            if (_Root == null) throw new InvalidOperationException("item-root not found");
            _ItemImage = this.Q<Image>("item-image");
        }
        public void Bind(ItemInstance item)
        {
            _Item = item;
            style.width = _Item.Definition.ItemSize.X * 64f;
            style.height = _Item.Definition.ItemSize.Y * 64f;

            var asset = GameplayHub.Instance.Get<ItemFactory>().GetItemDefinitionAsset(_Item.Definition.ItemId);
            _ItemImage.image = asset.Image;

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