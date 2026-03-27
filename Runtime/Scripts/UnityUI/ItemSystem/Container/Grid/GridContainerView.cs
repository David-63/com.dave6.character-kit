using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    [UxmlElement]
    public partial class GridContainerView : ContainerBaseView
    {
        VisualElement _Contents;

        GridArea _GridArea;
        GridContainer _GridContainer;

        public override void Initialize(VisualTreeAsset template)
        {
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

            var size = _GridContainer.GetGridSize();
            _GridArea.Columns = size.X;
            _GridArea.Rows = size.Y;

            _GridArea.style.width = _GridArea.Columns * _GridArea.CellSize;
            _GridArea.style.height = _GridArea.Rows * _GridArea.CellSize;

            _GridArea.MarkDirtyRepaint();
        }
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
