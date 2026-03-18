using System.Collections.Generic;
using Dave6.ItemSystem.Application.Item;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit
{
    /// <summary>
    /// 연결만 할래요
    /// </summary>
    public class InventoryMain : MonoBehaviour
    {
        VisualElement root;

        [SerializeField] VisualTreeAsset gridPanel;
        [SerializeField] VisualTreeAsset gridSpace;
        [SerializeField] VisualTreeAsset dragViewAsset;

        VisualElement dragLayer;
        [SerializeField] List<ItemDefinitionAsset> testItems = new();

        GridPanel lootPanel;

        GridSpace lootSpace;

        List<GridSpace> allSpaces = new();

        InventoryController controller;


        void Awake()
        {
            var doc = GetComponent<UIDocument>();
            root = doc.rootVisualElement.Q<VisualElement>("main-root");
            DragSetting();
            controller = new InventoryController(dragLayer, allSpaces);

            EquipmentSetting();
            InventorySetting();
            LootSetting();
        }

        void DragSetting()
        {
            dragLayer = root.Q<VisualElement>("drag-layer");
            dragLayer.pickingMode = PickingMode.Ignore;
            dragLayer.style.position = Position.Absolute;
            dragLayer.style.top = 0;
            dragLayer.style.bottom = 0;
            dragLayer.style.left = 0;
            dragLayer.style.right = 0;
        }

        void LootSetting()
        {
            lootPanel = root.Q<GridPanel>("right-panel");
            lootPanel.Initialize(gridPanel);
            
            lootSpace = AddSpace(lootPanel, "loot", 6, 8);

            lootSpace.gridCells.RegisterCallback<GeometryChangedEvent>(OnGridCellsGeometryChanged);
        }

        void OnGridCellsGeometryChanged(GeometryChangedEvent evt)
        {
            // 첫 번째 호출은 종종 NaN → 무시
            if (float.IsNaN(evt.newRect.width) || evt.newRect.width <= 0) return;

            // 이제 안전하게 위치 계산 가능
            // (필요 시 한 번만 실행되도록 unregister)
            lootSpace.gridCells.UnregisterCallback<GeometryChangedEvent>(OnGridCellsGeometryChanged);

            foreach (var item in testItems)
            {
                var definition = item.Create();
                AddItem(item, lootSpace);
            }
        }

        void InventorySetting()
        {
            var inventory = root.Q<GridPanel>("middle-panel");
            inventory.Initialize(gridPanel);
            AddSpace(inventory, "main", 4, 2);
        }

        void EquipmentSetting()
        {
            var equipment = root.Q<GridPanel>("left-panel");
            equipment.Initialize(gridPanel);

            AddSpace(equipment, "head", 3, 2);
            AddSpace(equipment, "upper-body", 3, 2);
            AddSpace(equipment, "lower-body", 3, 2);
        }

        GridSpace AddSpace(GridPanel panel, string spaceName, int column, int row)
        {
            var space = panel.AddSpace(spaceName, gridSpace, column, row);
            allSpaces.Add(space);            
            return panel.GetSpace(spaceName);
        }

        void AddItem(ItemDefinitionAsset definitionAsset, GridSpace space)
        {
            
            var instance = new ItemInstance(definitionAsset.Create());
            if (!controller.TryAutoPlace(space, instance)) return;

            var view = new DragItemView();
            view.Initialize(dragViewAsset, controller, instance, definitionAsset.Image);
            controller.onPlacementChanged += view.OnPlaceChanged;
            dragLayer.Add(view);

            view.RefreshFromPlacement();
            var drag = new DragManipulator(ResolveSpace);
            view.AddManipulator(drag);

            drag.onDrop = () =>
            {
                if (!controller.TryDrop(instance, view))
                {
                    view.RefreshFromPlacement();
                }
            };
        }

        GridSpace ResolveSpace(Vector2 panelPos)
        {
            foreach (var space in allSpaces)
            {
                if (space.worldBound.Contains(panelPos))
                {
                    return space;
                }
            }
            return null;
        }
    }
}
