using System;
using System.Collections.Generic;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.CharacterKit.Handler.Stats;
using Dave6.StatSystem2.Application;
using Dave6.StatSystem2.Domain;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class StatusView : VisualElement
    {
        VisualElement _Root;
        BaseStat _TargetStat;
        Label _RoleLabel;
        Dictionary<StatTag, StatRowView> _StatViews = new();

        public void Initialize(VisualTreeAsset template)
        {
            if (template == null) throw new ArgumentNullException(nameof(template));
            template.CloneTree(this);
            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = 0;

            _Root = this.Q<VisualElement>("status-root");
            if (_Root == null) throw new InvalidOperationException("status-root not found");

            _RoleLabel = this.Q<Label>("role-label");
            _RoleLabel.text = "Status";
        }
        public void Bind(BaseStat target)
        {
            _StatViews.Clear();
            if (_TargetStat != null) _TargetStat.OnStatChanged -= HandleStatChanged;

            _TargetStat = target;
            _TargetStat.OnStatChanged += HandleStatChanged;
            
            foreach (var group in _TargetStat.GetStatGroups())
            {
                var sectionView = GameplayHub.Instance.Get<ViewFactory>().CreateStatusSectionView();
                sectionView.SetLabel(group.GroupName);
                _Root.Add(sectionView);
                foreach (var tag in group.Tags)
                {
                    var statView = GameplayHub.Instance.Get<ViewFactory>().CreateStatRowView();
                    _TargetStat.TryGetStatValue(tag, out var stat);
                    statView.Bind(tag, stat.Calculate());
                    sectionView.AddRow(tag, statView);
                    _StatViews[tag] = statView;
                }
                // var label = new Label();
                // label.text = $"{tag.TagName}: ";
                // _Labels[tag] = label;
                //_Root.Add(label);
            }
        }

        void HandleStatChanged(StatTag statTag, float value)
        {
            if (!_StatViews.TryGetValue(statTag, out var statView)) return;

            statView.SetValue(value);

            //label.text = $"{statTag.TagName}: {value}";
        }
    }
}