using System;
using System.Collections.Generic;
using Dave6.Foundation.Collections;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.Temp
{
    /// <summary>
    /// 규칙과 계산만 하고싶어요
    /// </summary>
    public class InventoryController
    {
        List<GridSpace> spaces;
        VisualElement dragLayer;

        Dictionary<ItemInstance, ItemPlacement> placements = new();
        public event Action<ItemInstance> onPlacementChanged;

        public InventoryController(VisualElement dragLayer, List<GridSpace> spaces)
        {
            this.dragLayer = dragLayer;
            this.spaces = spaces;
        }

        #region 새로운 API

        public ItemPlacement GetPlacement(ItemInstance item) => placements[item];
        public bool TryAutoPlace(GridSpace space, ItemInstance item)
        {
            foreach (var coord in space.AllCoords())
            {
                if (CanPlace(space, coord, item))
                {
                    space.Occupy(coord, item);

                    var placement = new ItemPlacement
                    {
                        instance = item,
                        space = space,
                        origin = coord
                    };
                    placements.Add(item, placement);

                    return true;
                }
            }
            return false;
        }
        public bool TryDrop(ItemInstance dragItem, DragItemView dragView)
        {
            // 드래그 대상 찾기
            var dragPlacement = placements[dragItem];
            if (dragPlacement == null) return false;

            // 드롭할 공간 찾기
            Vector2 viewLeftTop = new Vector2(dragView.resolvedStyle.left, dragView.resolvedStyle.top);
            var dropSpace = ResolveSpace(viewLeftTop);
            if (dropSpace == null) return false;

            // 드롭 좌표 계산
            Vector2 dragLayerPos = dragLayer.WorldToLocal(dragView.worldBound.position);
            var dropOrigin = dropSpace.PanelToGridSnapped(dragLayerPos);

            // -----------------------------------------------------

            var result = dropSpace.EvaluatePlacement(dropOrigin, dragItem);

            Debug.Log(result.type);

            switch (result.type)
            {
                case PlacementResultType.Invalid:
                    return false;
                case PlacementResultType.Empty:
                {
                    MoveItemPlacement(dropSpace, dropOrigin, dragItem, dragPlacement);
                    return true;
                }
                case PlacementResultType.SingleOverlap:
                {
                    var swapItem = result.overlapItem;
                    return TrySwap(dragItem, swapItem);
                }
            }

            return false;
        }


        #endregion


        public GridSpace ResolveSpace(Vector2 panelPos)
        {
            foreach (var space in spaces)
            {
                if (space.Contains(panelPos)) return space;
            }
            return null;
        }
        

        void MoveItemPlacement(GridSpace destSpace, GridCoord destCoord, ItemInstance instance, ItemPlacement placement)
        {
            // 기존 점유 해제 (placement + instance)
            placement.space.ReleaseItem(placement.origin, instance);

            // 새로 점유 (space + originCoord + instance)
            destSpace.Occupy(destCoord, instance);

            // placement 갱신
            placement.space = destSpace;
            placement.origin = destCoord;
            onPlacementChanged?.Invoke(instance);
        }

        bool TrySwap(ItemInstance head, ItemInstance tail)
        {
            var placementHead = placements[head];
            var placementTail = placements[tail];

            var rectHead = placementHead.GetRect();
            var rectTail = placementTail.GetRect();

            // 크기가 동일한 경우에만 허용
            if (rectHead.Width != rectTail.Width || rectHead.Height != rectTail.Height) return false;

            var spaceHead = placementHead.space;
            var spaceTail = placementTail.space;

            var originHead = placementHead.origin;
            var originTail = placementTail.origin;


            // 둘 다 제거
            spaceHead.ReleaseItem(originHead, head);
            spaceTail.ReleaseItem(originTail, tail);

            // 교차 배치 가능 여부 검사
            bool canPlaceHead = spaceTail.EvaluatePlacement(originTail, head).type == PlacementResultType.Empty;
            bool canPlaceTail = spaceHead.EvaluatePlacement(originHead, tail).type == PlacementResultType.Empty;

            // 실패 판정
            if (!canPlaceHead || !canPlaceTail)
            {
                spaceHead.Occupy(originHead, head);
                spaceTail.Occupy(originTail, tail);
                return false;
            }

            // 배치
            spaceHead.Occupy(originHead, tail);
            spaceTail.Occupy(originTail, head);

            // placement 갱신
            placementHead.space = spaceTail;
            placementHead.origin = originTail;
            placementTail.space = spaceHead;
            placementTail.origin = originHead;

            onPlacementChanged?.Invoke(head);
            onPlacementChanged?.Invoke(tail);

            return true;
        }

        /// <summary>
        /// 해당 영역에 인스턴스 배치가 가능한지 검사
        /// </summary>
        /// <param name="targetSpace">목표 Space</param>
        /// <param name="origin">Space 내 좌표</param>
        /// <param name="sourceInstance">배치하려는 인스턴스</param>
        /// <param name="swapTarget">무시 대상</param>
        bool CanPlace(GridSpace targetSpace, GridCoord origin, ItemInstance sourceInstance, ItemInstance swapTarget = null)
        {
            var rect = new GridRect(origin, new GridCoord(sourceInstance.Definition.ItemSize));

            foreach (var cell in rect.Cells())
            {
                if (!targetSpace.grid.IsInside(cell)) return false;
                // cell에 등록된 value 가져오기
                var occupying = targetSpace.GetItemAt(cell);

                // value가 source와 같은 대상인지 검사, value가 ignore 대상인지 검사
                if (occupying != null && occupying != sourceInstance && occupying != swapTarget) return false;
            }
            return true;
        }

    }
}
