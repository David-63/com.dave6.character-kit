using Dave6.StatSystem;
using UnityEngine;

namespace Dave6.CharacterKit
{

    /// <summary>
    /// 알고보니 여기서는 딱히 스텟에 연관된게 없다¿
    /// </summary>
    public class PlayerMover : BasicMover
    {
        PlayerController m_PlayerController;

        protected override void Setup()
        {
            base.Setup();
            m_PlayerController = controller as PlayerController;
        }

        public override void CalculateSpeed(float deltaTime)
        {
            if (isGrounded)
            {
                GroundSpeed(deltaTime);
            }
            else
            {
                AirborneSpeed(deltaTime);
            }
        }

        void GroundSpeed(float deltaTime)
        {
            if (Mathf.Abs(controller.horizontalSpeed - controller.targetSpeed) > m_SpeedOffset)
            {
                controller.horizontalSpeed = Mathf.Lerp
                (
                    controller.horizontalSpeed, controller.targetSpeed, deltaTime * m_MovementProfile.SpeedChangeRate
                );
                controller.horizontalSpeed = Mathf.Round(controller.horizontalSpeed * 1000f) / 1000f;
            }
            else
            {
                controller.horizontalSpeed = controller.targetSpeed;
            }
        }

        void AirborneSpeed(float deltaTime)
        {
            controller.horizontalSpeed = Mathf.Lerp(controller.horizontalSpeed, 0, deltaTime * 0.5f);
            controller.horizontalSpeed = Mathf.Round(controller.horizontalSpeed * 1000f) / 1000f;
        }
    }
}
