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
            controller.horizontalSpeed = Mathf.Lerp(controller.horizontalSpeed, 1, deltaTime * 0.5f);
            controller.horizontalSpeed = Mathf.Round(controller.horizontalSpeed * 1000f) / 1000f;
        }

        public override void FreeLookRotate(float deltaTime)
        {
            if (!controller.HasMovementInput()) return;

            // 입력의 수평 수직 값을 기반으로 각을 구한 뒤, 카메라 회전(yaw)값 누적
            float inputAngle = Mathf.Atan2(controller.inputMove.x, controller.inputMove.z) * Mathf.Rad2Deg;
            m_TargetRotation = inputAngle + yawAngle;
            // 현재 transform의 yaw 값을 목표 값으로 보간
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_RotationSpeed, m_MovementProfile.RotationSmoothTime);
            // 캐릭터 회전시키기
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            // 이동방향 전달
            controller.moveDirection = CalculateFreeLockDirection(deltaTime);
        }

        Vector3 CalculateFreeLockDirection(float deltaTime)
        {
            float lerpRotation = m_TargetRotation;
            if (!isGrounded)
            {
                float currentYaw = Mathf.Atan2(controller.moveDirection.x, controller.moveDirection.z) * Mathf.Rad2Deg;
                lerpRotation = Mathf.Lerp(currentYaw, m_TargetRotation, deltaTime * 0.5f);
            }

            return (Quaternion.Euler(0.0f, lerpRotation, 0.0f) * Vector3.forward).normalized;
        }

        public override void StrafeMoveRotate(float deltaTime)
        {
            // 카메라 회전값으로 고정!!
            m_TargetRotation = yawAngle;

            // 현재 transform의 yaw 값을 목표 값으로 보간
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, m_TargetRotation, ref m_RotationSpeed, m_MovementProfile.RotationSmoothTime);
            // 캐릭터 회전 시키기
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            // 이동방향 전달
            if (controller.HasMovementInput())
            {
                controller.moveDirection = CalculateStrafeMoveDirection(deltaTime);
            }
        }

        Vector3 CalculateStrafeMoveDirection(float deltaTime)
        {
            Vector3 cameraDirection = Quaternion.Euler(0f, yawAngle, 0f) * controller.inputMove;
            cameraDirection.Normalize();

            float targetYaw = Mathf.Atan2(cameraDirection.x, cameraDirection.z) * Mathf.Rad2Deg;
            float lerpRotation = targetYaw;

            if (!isGrounded)
            {
                float currentYaw = Mathf.Atan2(controller.moveDirection.x, controller.moveDirection.z) * Mathf.Rad2Deg;
                lerpRotation = Mathf.LerpAngle(currentYaw, targetYaw, deltaTime * 0.5f);
            }

            return Quaternion.Euler(0f, lerpRotation, 0f) * Vector3.forward;
        }
    }
}
