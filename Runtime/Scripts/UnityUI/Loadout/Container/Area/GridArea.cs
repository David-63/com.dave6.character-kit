using Dave6.ItemSystem.Domain.Container;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public partial class GridArea : ContainerArea
    {
        GridContainer _GridContainer;
        public int Columns;
        public int Rows;
        public float CellSize = 64f;

        public GridArea()
        {
            generateVisualContent += OnGenerateVisualContent;
            style.position = Position.Relative;
        }
        public override void Build(IItemContainer container)
        {
            _GridContainer = container as GridContainer;
            Columns = _GridContainer.GetGridSize().X;
            Rows = _GridContainer.GetGridSize().Y;
            style.width = Columns * CellSize;
            style.height = Rows * CellSize;
            MarkDirtyRepaint();
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
