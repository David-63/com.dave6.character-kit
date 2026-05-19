using System;
using System.Collections.Generic;
using Dave6.StatSystem2.Application;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class StatusSectionView : VisualElement
    {
        VisualElement _Root;
        Label _SectionLabel;
        VisualElement _ContentLayer;
        Dictionary<StatTag, StatRowView> _RowViews = new();

        public void Initialize(VisualTreeAsset template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            template.CloneTree(this);

            _SectionLabel = this.Q<Label>("section-label");
            _ContentLayer = this.Q<VisualElement>("section-contents");
        }
        public void SetLabel(string text)
        {
            _SectionLabel.text = text;
        }
        public void AddRow(StatTag tag, StatRowView row)
        {
            _RowViews[tag] = row;
            _ContentLayer.Add(row);
        }
        public bool TryGetRow(StatTag tag, out StatRowView row)
        {
            return _RowViews.TryGetValue(tag, out row);
        }
    }
}