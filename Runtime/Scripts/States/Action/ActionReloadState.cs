using Dave6.CharacterKit.Item;
using Dave6.StateMachine;
using UnityEngine;

namespace Dave6.CharacterKit.States
{
    /// <summary>
    /// 진입 조건은 `R` 입력
    /// 종료 조건은 애니메이션 이벤트
    /// 전환 조건은 좀 빡센데
    /// 
    /// </summary>
    public class ActionReloadState : BaseState<PlayerCharacter>
    {
        public ActionReloadState(PlayerCharacter controller) : base(controller) { }
        public override void OnEnter()
        {
            Debug.Log("Reload Enter");
            controller.combatHandler.TryReload();
        }

        public override void OnExit()
        {
            Debug.Log("Reload Exit");
        }

        public override  void Update()
        {
        }
    }
}
