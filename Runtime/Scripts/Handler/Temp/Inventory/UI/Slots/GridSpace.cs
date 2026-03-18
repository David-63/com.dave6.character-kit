using System.Collections.Generic;
using Dave6.Foundation.Collections;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit
{
    public enum PlacementResultType
    {
        Invalid,    // 영역 밖 or 다중 겹침
        Empty,      // 문제 없음
        SingleOverlap
    }
    public struct PlacementCheckResult
    {
        public PlacementResultType type;
        public ItemInstance overlapItem;
    }
    [UxmlElement]
    public partial class GridSpace : VisualElement
    {
        VisualElement spaceRoot;
        Label gridLabel;
        #region UI 요소
        public VisualElement gridCells;
        VisualElement[,] cells;
        VisualElement debugLayer;
        #endregion

        #region 데이터
        public Grid2D<ItemInstance> grid {get; private set; }
        public int columns;
        public int rows;
        public float CELL_SIZE = 64f;
        #endregion

        public void Initialize(string key, VisualTreeAsset template)
        {
            Clear();
            template.CloneTree(this);

            spaceRoot = this.Q<VisualElement>("space-root");
            spaceRoot.userData = this;  // Visual Element에 GridSpace 클래스를 등록해서 인식가능
            gridLabel = spaceRoot.Q<Label>("grid-label");
            gridCells = spaceRoot.Q<VisualElement>("grid-cells");

            gridLabel.text = key;

            debugLayer = new VisualElement();
            debugLayer.style.position = Position.Absolute;
            debugLayer.pickingMode = PickingMode.Ignore;

            Add(debugLayer);
        }

        public void BuildGrid(int columns, int rows, float cellSize = 64f)
        {
            gridCells.Clear();

            grid = new Grid2D<ItemInstance>(columns, rows);

            this.columns = columns;
            this.rows = rows;
            CELL_SIZE = cellSize;

            cells = new VisualElement[columns, rows];

            gridCells.style.width = columns * CELL_SIZE;
            gridCells.style.height = rows * CELL_SIZE;

            for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
            {
                gridCells.Add(CreateCell(x, y));
            }
        }

        #region API
        /// <summary>
        /// 공간 판별용
        /// </summary>
        public bool Contains(Vector2 panelPos)
        {
            return worldBound.Contains(panelPos);
        }
        public IEnumerable<GridCoord> AllCoords()
        {
            for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
            {
                yield return new GridCoord(x, y);
            }
        }
        public ItemInstance GetItemAt(GridCoord coord)
        {
            return grid.TryGetCell(coord, out var value) ? value : null;
        }
        /// <summary>
        /// Drop 위치 계산
        /// </summary>
        public GridCoord PanelToGrid(Vector2 otherLayerPos)
        {
            Vector2 local = gridCells.WorldToLocal(otherLayerPos);

            int x = Mathf.FloorToInt(local.x / CELL_SIZE);
            int y = Mathf.FloorToInt(local.y / CELL_SIZE);
            return new GridCoord(x, y);
        }
        public GridCoord PanelToGridSnapped(Vector2 otherLayerPos)
        {
            Vector2 local = gridCells.WorldToLocal(otherLayerPos);
            int x = Mathf.RoundToInt(local.x / CELL_SIZE);
            int y = Mathf.RoundToInt(local.y / CELL_SIZE);
            return new GridCoord(x, y);
        }
        /// <summary>
        /// Snap 위치 계산
        /// </summary>
        public Vector2 GridToPanelPositionCenter(GridCoord coord, VisualElement parent)
        {
            Vector2 local = new Vector2(coord.X * CELL_SIZE + CELL_SIZE * 0.5f, coord.Y * CELL_SIZE + CELL_SIZE * 0.5f);
            Vector2 world = gridCells.LocalToWorld(local);
            return parent.WorldToLocal(world);
        }
        public Vector2 GridToPanelPositionLeftTop(GridCoord coord, VisualElement parent)
        {
            Vector2 local = new Vector2(coord.X * CELL_SIZE, coord.Y * CELL_SIZE);
            Vector2 world = gridCells.LocalToWorld(local);
            return parent.WorldToLocal(world);
        }
        public PlacementCheckResult EvaluatePlacement(GridCoord origin, ItemInstance item)
        {
            var rect = new GridRect(origin, new GridCoord(item.Definition.ItemSize));

            ItemInstance found = null;

            foreach (var cell in rect.Cells())
            {
                // 영역 밖이면 즉시 실패
                if (!grid.IsInside(cell)) return new PlacementCheckResult { type = PlacementResultType.Invalid };

                var occupying = GetItemAt(cell);

                // 비였거나 ignore 대상이면 통과
                if (occupying == null || occupying == item) continue;

                if (found == null)
                {
                    found = occupying;
                }
                else if (found != occupying)
                {
                    return new PlacementCheckResult { type = PlacementResultType.Invalid };
                }
            }
            if (found == null)
            {
                return new PlacementCheckResult { type = PlacementResultType.Empty };
            }
            
            return new PlacementCheckResult { type = PlacementResultType.SingleOverlap, overlapItem = found };
        }
        
        public void Occupy(GridCoord origin, ItemInstance item)
        {
            var rect = new GridRect(origin, new GridCoord(item.Definition.ItemSize));
            grid.SetCellRect(rect, item);
            RefreshDebug();
        }
        public void ReleaseItem(GridCoord origin, ItemInstance item)
        {
            var rect = new GridRect(origin, new GridCoord(item.Definition.ItemSize.X, item.Definition.ItemSize.Y));
            grid.ClearCellRect(rect);
            RefreshDebug();
        }

        #endregion
        void RefreshDebug()
        {
            debugLayer.Clear();

            foreach (var coord in AllCoords())
            {
                var item = GetItemAt(coord);
                if (item == null) continue;

                var cell = new VisualElement();
                cell.style.position = Position.Absolute;

                Vector2 pos = GridToPanelPositionCenter(coord, debugLayer);

                cell.style.left = pos.x;
                cell.style.top = pos.y;
                cell.style.width = CELL_SIZE;
                cell.style.height = CELL_SIZE;

                cell.style.backgroundColor = new Color(1f, 0f, 0f, 0.25f); // 반투명 빨강

                debugLayer.Add(cell);
            }
        }

        VisualElement CreateCell(int x, int y)
        {
            var cell = new VisualElement();
            cell.AddToClassList("s-cell");
            cell.userData = new GridCoord(x, y);

            cell.style.left = x * CELL_SIZE;
            cell.style.top = y * CELL_SIZE;
            cell.style.width = CELL_SIZE;
            cell.style.height = CELL_SIZE;

            cell.RegisterCallback<ClickEvent>(_ =>
            {
                Debug.Log($"cell: {x}, {y}");
                Debug.Log($"worldBound: {cell.worldBound.position.x}, {cell.worldBound.position.y}");
            });

            cells[x, y] = cell;
            return cell;
        }


    }
}
