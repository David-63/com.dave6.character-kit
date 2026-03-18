using Dave6.Foundation.GameLogic.State;

namespace Dave6.CharacterKit.States
{
    public class EnemyIdleState : BaseState<EnemyController>
    {
        public EnemyIdleState(EnemyController controller) : base(controller) {}
    }
    public class EnemyChaseState : BaseState<EnemyController>
    {
        public EnemyChaseState(EnemyController controller) : base(controller) {}
    }
    public class EnemyAttackState : BaseState<EnemyController>
    {
        public EnemyAttackState(EnemyController controller) : base(controller) {}
    }
    public class EnemySearchState : BaseState<EnemyController>
    {
        public EnemySearchState(EnemyController controller) : base(controller) {}
    }
    public class EnemyReturnState : BaseState<EnemyController>
    {
        public EnemyReturnState(EnemyController controller) : base(controller) {}
    }
}