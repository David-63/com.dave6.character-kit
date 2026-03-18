// using System.Collections.Generic;
// using Dave6.GridCore;
// using Dave6.ItemCore;

// namespace Dave6.CharacterKit.Item
// {
//     // 아이템 소유 개념
//     public class Inventory
//     {
//         public readonly List<GridSpace> spaces;
//         public readonly List<ItemInstance> items;
//         public readonly Dictionary<ItemInstance, GridEntryItem> itemEntries;

//         public void AddSpace(GridSpace space)
//         {
//             spaces.Add(space);
//         }

//         public ItemInstance AddItem(ItemCore.ItemDefinition definition)
//         {
//             var instance = new ItemInstance(definition);
//             items.Add(instance);
//             return instance;
//         }
//     }

//     public class GridSpaceController
//     {
//         GridSpace space;
//         public IEnumerable<GridEntryItem> entries => space.entries;

//         public bool TryPlace(ItemInstance item, GridCoord at) => space.TryPlace(item, at, out _);
//     }
// }