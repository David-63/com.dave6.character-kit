// using Dave6.GridCore;
// using Dave6.ItemCore;

// namespace Dave6.CharacterKit.Item
// {
//     /// <summary>
//     /// 배치 규칙
//     /// </summary>
//     public class InventoryGridRule
//     {
//         readonly Grid2D<GridEntryItem> grid;
//         public InventoryGridRule(Grid2D<GridEntryItem> grid)
//         {
//             this.grid = grid;
//         }
//         public bool CanPlace(GridRect rect)
//         {
//             foreach (var coord in rect.Cells())
//             {
//                 if (!grid.IsInside(coord)) return false;
//                 if (grid.TryGetCell(coord, out var exist) && exist != null) return false;
//             }
//             return true;
//         }
//         public bool Place(GridEntryItem entry)
//         {
//             if (!CanPlace(entry.rect)) return false;

//             foreach (var coord in entry.rect.Cells())
//             {
//                 grid.SetCell(coord, entry);
//             }
//             return true;
//         }

//         public void Clear(GridRect rect)
//         {
//             foreach (var coord in rect.Cells())
//             {
//                 grid.ClearCell(coord);
//             }
//         }
//     }
// }