using Dave6.ItemSystem.Domain.Item;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public abstract partial class SectionView : VisualElement
    {
        VisualElement _Root;
        protected VisualElement _VisualArea;

        protected ItemInstance _Item;

        public virtual void Initialize(VisualTreeAsset template)
        {
            Clear();
            template.CloneTree(this);
            _Root = this.Q<VisualElement>("section-root");
        }
        public virtual void Bind(ItemInstance item)
        {
            _Item = item;

            //if (_otherArea != null) _VisualArea.Remove(_otherArea);

            BuildArea();
        }
        protected abstract void BuildArea();

    }
}