namespace Dave6.CharacterKit
{

    /// <summary>
    /// 알고보니 여기서는 딱히 스텟에 연관된게 없다¿
    /// </summary>
    public class PlayerMover : BasicMover
    {
        PlayerCharacter m_PlayerController;

        protected override void Setup()
        {
            base.Setup();
            m_PlayerController = controller as PlayerCharacter;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
