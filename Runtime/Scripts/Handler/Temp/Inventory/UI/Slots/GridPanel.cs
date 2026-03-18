using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit
{
    [UxmlElement]
    public partial class GridPanel : VisualElement
    {
        VisualElement contents;
        //public List<GridSpace> spaces = new List<GridSpace>();
        public Dictionary<string, GridSpace> spaces = new();
        public void Initialize(VisualTreeAsset template)
        {
            Clear();
            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = 0;
            template.CloneTree(this);

            contents = this.Q<VisualElement>("panel-contents");
        }
        public GridSpace AddSpace(string key, VisualTreeAsset space, int columns, int rows)
        {
            var gridSpace = new GridSpace();
            gridSpace.Initialize(key, space);
            spaces[key] = gridSpace;
            
            gridSpace.BuildGrid(columns, rows);
            contents.Add(gridSpace);

            return gridSpace;
        }

        public void RebuildSpace(string key, int columns, int rows)
        {
            if (!spaces.TryGetValue(key, out var target)) return;

            target.BuildGrid(columns, rows);
        }
        public void RemoveSpace(string key)
        {
            if (!spaces.TryGetValue(key, out var target)) return;

            contents.Remove(target);
            spaces.Remove(key);
        }
        public GridSpace GetSpace(string key)
        {
            if (!spaces.TryGetValue(key, out var target)) return null;
            return target;
        }
    }
}