using Dave6.Foundation.Math;
using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class GridContainerView : ContainerBaseView
    {
        GridContainer _GridContainer;

        float _CellSize = 64f;

        protected override void BuildArea()
        {
            _GridContainer = _Container as GridContainer;
            _ContainerArea = new GridArea();
            _VisualArea.Add(_ContainerArea);  // 이거 부모에서 함수로 제공해야함 protected api 느낌
            SetupGrid();
        }
        void SetupGrid()
        {
            _ContainerArea.Build(_GridContainer);
            GridArea gridArea = _ContainerArea as GridArea;
            gridArea.CellSize = _CellSize;
        }

        #region Input API
        public Vector2 PanelToLocal(Vector2 panelPos)
        {
            return _ContainerArea.WorldToLocal(panelPos);
        }
        public Int2 LocalToGrid(Vector2 localPos)
        {
            int x = Mathf.RoundToInt(localPos.x / _CellSize);
            int y = Mathf.RoundToInt(localPos.y / _CellSize);
            return new Int2(x, y);
        }
        public override ItemPlacement ResolvePlacement(Vector2 panelPos)
        {
            var localPos = PanelToLocal(panelPos);
            var gridPos = LocalToGrid(localPos);
            return new GridPlacement(gridPos, false);
        }
        #endregion

        #region Output API
        public Vector2 GridToLocal(Int2 gridPos)
        {
            float x = gridPos.X * _CellSize;
            float y = gridPos.Y * _CellSize;
            return new Vector2(x, y);
        }
        public Vector2 LocalToPanel(Vector2 localPos)
        {
            return _ContainerArea.LocalToWorld(localPos);
        }
        public override Vector2 PlacementToPanel(ItemPlacement placement)
        {
            if (placement is not GridPlacement gp) return Vector2.zero;
            var localPos = GridToLocal(gp.Position);
            return LocalToPanel(localPos);
        }
        #endregion
        public override bool OverlapView(Rect area) => _ContainerArea.worldBound.Overlaps(area);
    }
}
