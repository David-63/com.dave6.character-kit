using Dave6.Foundation.Math;
using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class GridContainerView : ContainerBaseView
    {
        VisualElement _Contents;

        GridContainer _GridContainer;
        GridArea _GridArea;

        public GridArea GetGridArea() => _GridArea;

        public override void Initialize(VisualTreeAsset template, ItemInteractionController interactionController)
        {
            _InteractionController = interactionController;
            //Clear();
            style.flexGrow = 1;
            style.flexShrink = 1;
            style.flexBasis = 0;
            if (template != null)
            {
                template.CloneTree(this);
            }

            _Contents = this.Q<VisualElement>("grid-root");
            if (_Contents == null)
            {
                Debug.LogError("root 못찾음");
            }

            _GridArea = new GridArea{Columns = 10, Rows = 6, CellSize = 64f};
            _GridArea.style.position = Position.Relative;
            _Contents.Add(_GridArea);
        }

        public override void Bind(IItemContainer container)
        {
            _Container = container;
            _GridContainer = container as GridContainer;

            SetupGrid();
        }

        void SetupGrid()
        {
            var size = _GridContainer.GetGridSize();
            _GridArea.Columns = size.X;
            _GridArea.Rows = size.Y;

            _GridArea.style.width = _GridArea.Columns * _GridArea.CellSize;
            _GridArea.style.height = _GridArea.Rows * _GridArea.CellSize;

            _GridArea.MarkDirtyRepaint();
        }
        #region Input API
        public Vector2 PanelToLocal(Vector2 panelPos)
        {
            return _GridArea.WorldToLocal(panelPos);
        }
        public Int2 LocalToGrid(Vector2 localPos)
        {
            int x = Mathf.RoundToInt(localPos.x / _GridArea.CellSize);
            int y = Mathf.RoundToInt(localPos.y / _GridArea.CellSize);
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
            float x = gridPos.X * _GridArea.CellSize;
            float y = gridPos.Y * _GridArea.CellSize;
            return new Vector2(x, y);
        }
        public Vector2 LocalToPanel(Vector2 localPos)
        {
            return _GridArea.LocalToWorld(localPos);
        }
        public override Vector2 PlacementToPanel(ItemPlacement placement)
        {
            if (placement is not GridPlacement gp) return Vector2.zero;
            var localPos = GridToLocal(gp.Position);
            return LocalToPanel(localPos);
        }
        #endregion
        public override bool OverlapView(Rect area) => _GridArea.worldBound.Overlaps(area);
        
    }
    [UxmlElement]
    public partial class GridArea : VisualElement
    {
        public int Columns;
        public int Rows;
        public float CellSize = 64f;

        public GridArea()
        {
            generateVisualContent += OnGenerateVisualContent;
        }

        void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            var painter = ctx.painter2D;

            painter.strokeColor = new Color(1f,1f,1f,0.3f);
            painter.lineWidth = 1;

            float width = Columns * CellSize;
            float height = Rows * CellSize;

            // vertical lines
            for (int x = 0; x <= Columns; x++)
            {
                float px = x * CellSize;

                painter.BeginPath();
                painter.MoveTo(new Vector2(px, 0));
                painter.LineTo(new Vector2(px, height));
                painter.Stroke();
            }

            // horizontal lines
            for (int y = 0; y <= Rows; y++)
            {
                float py = y * CellSize;

                painter.BeginPath();
                painter.MoveTo(new Vector2(0, py));
                painter.LineTo(new Vector2(width, py));
                painter.Stroke();
            }
        }
    }
}
