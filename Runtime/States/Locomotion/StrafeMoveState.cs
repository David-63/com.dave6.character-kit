using UnityEngine;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Stat;

namespace Dave6.CharacterKit.States
{
    public class StrafeMoveState : BaseState<PlayerController>
    {
        StatHandler m_StatHandler;
        bool ShiftToggle = false;

        public StrafeMoveState(PlayerController controller) : base(controller)
        {
            m_StatHandler = controller.statHandler;
            if (m_StatHandler == null)
            {
                Debug.Log("잘못연결된것같아요");
            }
        }

        public override void OnEnter()
        {
            ShiftToggle = true;
            controller.GetMover().SetStrafeMode(ShiftToggle);
            controller.GetInputReader().ShiftToggleChanged += OnShiftToggled;
        }

        public override void OnExit()
        {
            controller.GetInputReader().ShiftToggleChanged -= OnShiftToggled;
        }

        public override void Update()
        {
            float deltaTime = Time.deltaTime;
            UpdateTargetSpeed();
            controller.GetMover().CalculateSpeed(deltaTime);
            controller.GetMover().StrafeMoveRotate(deltaTime);
        }

        void UpdateTargetSpeed()
        {
            float targetSpeed = 0;

            SecondaryStat moveStat = m_StatHandler.GetStat("S_MoveSpeed") as SecondaryStat;
            float moveSpeed = moveStat.finalValue;

            if (controller.HasMovementInput())
            {
                targetSpeed = moveSpeed - moveSpeed * 0.4f;
            }

            controller.targetSpeed = targetSpeed;
        }

        void OnShiftToggled()
        {
            ShiftToggle = !ShiftToggle;
            controller.GetMover().SetStrafeMode(ShiftToggle);
        }
    }
}