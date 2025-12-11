using UnityEngine;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Stat;

namespace Dave6.CharacterKit.States
{
    public class FreeLookState : BaseState<BasicPlayerController>
    {
        StatHandler m_StatHandler;

        public FreeLookState(BasicPlayerController controller) : base(controller)
        {
            PlayerController playerController = controller as PlayerController;
            m_StatHandler = playerController.statHandler;
            if (m_StatHandler == null)
            {
                Debug.Log("잘못연결된것같아요");
            }
        }
        public override void OnEnter()
        {
            controller.GetMover().SetFreeLookMode();
        }

        public override void OnExit()
        {
        }

        public override  void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateTargetSpeed();
            controller.GetMover().CalculateSpeed(deltaTime);
            controller.GetMover().FreeLookRotate(deltaTime);
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            SecondaryStat moveStat = m_StatHandler.GetStat("S_MoveSpeed") as SecondaryStat;

            if (controller.HasMovementInput())
            {
                if (controller.shiftInput)
                {
                    targetSpeed = moveStat.finalValue * 1.8f;
                }
                else
                {
                    targetSpeed = moveStat.finalValue;
                }
            }

            controller.targetSpeed = targetSpeed;
        }
    }
}