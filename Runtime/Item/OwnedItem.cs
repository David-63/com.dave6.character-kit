namespace Dave6.CharacterKit.Item
{
    /// <summary>
    /// 인벤토리상에 들어갈 순수 아이템 데이터
    /// </summary>
    public class OwnedItem
    {
        public ItemDefinition definition { get; }
        public int stack { get; private set; }

        public OwnedItem(ItemDefinition definition, int stack)
        {
            this.definition = definition;
            this.stack = stack;
        }
    }
}
