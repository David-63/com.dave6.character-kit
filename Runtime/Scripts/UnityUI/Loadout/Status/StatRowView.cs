using System;
using Dave6.StatSystem2.Application;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class StatRowView : VisualElement
    {
        //VisualElement _Root;
        Label _NameLabel;
        Label _ValueLabel;
        StatTag _Tag;

        public void Initialize(VisualTreeAsset template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            template.CloneTree(this);

            _NameLabel = this.Q<Label>("tag-name");
            _ValueLabel = this.Q<Label>("tag-value");
        }

        public void Bind(StatTag tag, float statValue)
        {
            _Tag = tag;
            _NameLabel.text = tag.TagName;
            SetValue(statValue);
        }

        public void SetValue(float value)
        {
            _ValueLabel.text = value.ToString();
        }
        public StatTag GetTag() => _Tag;
    }
}