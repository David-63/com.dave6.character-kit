using Dave6.Foundation.Collections;
using Dave6.ItemSystem.Domain.Item;

namespace Dave6.CharacterKit
{
    public class ItemPlacement
    {
        public ItemInstance instance;
        public GridSpace space;
        public GridCoord origin;

        public GridRect GetRect()
        {
            return new GridRect(origin, new GridCoord(instance.Definition.ItemSize));
        }
    }
}
