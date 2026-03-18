// using System.Collections.Generic;
// using Dave6.GridCore;
// using Dave6.ItemCore;
// using UnityEngine;

// namespace Dave6.CharacterKit.Item
// {
//     // Grid 영역
//     public class GridSpace
//     {
//         //public readonly Inventory owner;

//         readonly Grid2D<GridEntryItem> m_Grid;
//         readonly InventoryGridRule m_Rule;

//         public readonly List<GridEntryItem> entries = new();

//         public GridSpace(int width, int height)
//         {
//             m_Grid = new Grid2D<GridEntryItem>(width, height);
//             m_Rule = new InventoryGridRule(m_Grid);
//         }

//         public bool TryPlace(ItemInstance item, GridCoord at, out GridEntryItem entry)
//         {
//             var size = new GridCoord(item.definition.width, item.definition.height);

//             var rect = new GridRect(at, size);
//             if (!m_Rule.CanPlace(rect))
//             {
//                 entry = null;
//                 return false;
//             }

//             entry = new GridEntryItem(item, this, rect);
//             m_Rule.Place(entry);
//             entries.Add(entry);
//             return true;
//         }

//         public bool Remove(GridEntryItem entry)
//         {
//             if (!entries.Remove(entry)) return false;

//             m_Rule.Clear(entry.rect);
//             return true;
//         }
//     }


// }