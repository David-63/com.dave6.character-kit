using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils.Timer;

namespace Dave6.CharacterKit.Combat
{
    /// <summary>
    /// 필요한거
    /// 
    /// 1. 공격 관련 프리팹     | 이거 때문에 Mono로 해야함
    /// 2. 공격 방향 계산
    /// 3. 상태 제어
    /// 
    /// 4. 공격 판정 및 투사체 생성
    
    //콤보 규칙
    //공격 데이터
    //히트박스 관리
    //공격 쿨타임

    //“지금 공격 가능한가?” 판단
    /// </summary>
    public class CombatHandler : MonoBehaviour
    {
        public bool useDebugSphere = false;
        PlayerController m_Controller;
        #region 외부 프리팹 필드
        [Header("외부 프리팹 필드")]
        [SerializeField] GameObject m_MeleeHitPrefab;
        public GameObject meleeHitPrefab => m_MeleeHitPrefab;
        MeleeHitbox m_Hitbox;

        [SerializeField] GameObject m_ProjectilePrefab;
        public GameObject projectilePrefab => m_ProjectilePrefab;
        [SerializeField] GameObject crosshairPrefab;
        [SerializeField] GameObject targetMarkPrefab;
        public Transform muzzle;
        #endregion

        


        #region 밀리 히트박스 필드
        [Header("밀리 히트 필드")]
        int m_ComboStep = 0;
        [SerializeField] const int m_ComboEnd = 3;
        [SerializeField] float m_MeleeImpulse = 10f;
        Timer m_StepTimer;                              // 각 공격은 0.8 ~ 1.2초 걸림
        float m_StepDuration = 3f;
        Timer m_HitboxExistTimer;
        float m_HitboxDuration = 0.5f;
        #endregion

        #region 에임 계산        
        CrosshairController m_BodyCrosshairUI;
        CrosshairController m_CameraCrosshairUI;
        Vector3 m_CharacterAimPoint;
        public Vector3 characterAimPoint => m_CharacterAimPoint;
        #endregion


        #region 타겟 시스템
        [Header("타겟 서치 필드")]
        [SerializeField] float m_TargetFindRadius = 5f;
        [SerializeField] LayerMask m_TargetFindLayer;
        [SerializeField] float m_ViewAngleMin = -60;
        [SerializeField] float m_ViewAngleMax = 60;

        RectTransform m_TargetMarkUI;

        ITargetable m_CurrentTarget;

        #endregion

        void Start()
        {
            // 타이머 세팅
            m_StepTimer = new Countdown(m_StepDuration);
            m_StepTimer.OnTimerStop += ComboReset;
            m_HitboxExistTimer = new Countdown(m_HitboxDuration);
            m_HitboxExistTimer.OnTimerStop += HitboxReset;

            m_Hitbox = m_Controller.InstantiatePrefabSetParent(m_MeleeHitPrefab).GetComponent<MeleeHitbox>();
            if (m_Hitbox == null)
            {
                Debug.Log("m_Hitbox 초기화 안됨");
            }
            m_Hitbox.Initialize(m_Controller);
            m_Hitbox.transform.localPosition = new Vector3(0, 1, 1);
            m_Hitbox.gameObject.SetActive(false);

            CrosshairCanvas crosshair = m_Controller.InstantiatePrefab(crosshairPrefab).GetComponent<CrosshairCanvas>();
            m_BodyCrosshairUI = crosshair.bodyCrosshairUI;
            m_CameraCrosshairUI = crosshair.cameraCrosshairUI;

            m_TargetMarkUI = m_Controller.InstantiatePrefab(targetMarkPrefab).GetComponent<TargettingCanvas>().targettingUI;
            m_TargetMarkUI.gameObject.SetActive(false);
        }

        public void OnLateUpdate()
        {
            // Pitch 계산!!
            float targetPitch = m_Controller.mover.CalcTargetPitchByAimPoint(m_Controller.combatHandler.muzzle.position);
            m_Controller.mover.SmoothPitchUpdate(targetPitch, Time.deltaTime);
            m_Controller.mover.CharacterAimUpdate();

            Vector3 origin = m_Controller.combatHandler.muzzle.position;
            Vector3 direction = m_Controller.mover.characterAim * Vector3.forward;
            m_CharacterAimPoint = m_Controller.CalculateAimPoint(origin, direction);


            m_BodyCrosshairUI?.LateUpdateCrosshair(m_CharacterAimPoint);
            m_CameraCrosshairUI?.LateUpdateCrosshair(m_Controller.cameraHandler.baseAimPoint);
        }

        public void RegisterCombat(PlayerController controller)
        {
            m_Controller = controller;
        }

        #region 콤보 히트
        void ComboReset()
        {
            m_ComboStep = 0;
        }
        void HitboxReset()
        {
            m_Hitbox.gameObject.SetActive(false);
        }
        public bool TryMeleeAttack()
        {
            // 애니메이션으로 공격 준비 확인
            if (!m_Controller.animatorHandler.attackReady) return false;
            m_Controller.animatorHandler.attackReady = false;
            
            bool isComboEnd = false;
            // 충돌체 활성화
            m_Hitbox.gameObject.SetActive(true);

            // 콤보에 따른 애니메이션 호출
            if (m_ComboStep == 0)
            {
                m_Controller.animatorHandler.ChangeAnimation("LeftPunch", 0.1f);
            }
            else if (m_ComboStep == m_ComboEnd -1)
            {
                m_Controller.animatorHandler.ChangeAnimation("CrossPunch");
                isComboEnd = true;
            }
            else
            {
                m_Controller.animatorHandler.ChangeAnimation("RightHook");
            }

            // 움직임 제어
            m_Controller.attacking = true;

            // 타이머 제어
            m_HitboxExistTimer.RestartTimer();
            m_StepTimer.RestartTimer();

            // 콤보 카운트 제어
            if (isComboEnd)
            {
                ComboReset();
            }
            else
            {
                m_ComboStep++;
            }
            return true;
        }
        #endregion
        
        #region 추가 기능
        public void AddAttackImpulse()
        {
            m_Controller.impulseSpeed = m_MeleeImpulse;
        }
        #endregion


        #region 타겟 마킹 기능
        public void UpdateTargetMark()
        {
            var targets = CollectValidTargets();
            m_CurrentTarget = SelectNearestTarget(targets);
            UpdateTargetUI(m_CurrentTarget);
        }

        public bool TryGetMeleeTargetYaw(out float yaw)
        {
            yaw = 0;
            if (m_CurrentTarget == null) return false;
            yaw = CalculateYawToTarget(m_CurrentTarget);
            return true;
        }

        List<ITargetable> CollectValidTargets()
        {
            List<ITargetable> result = new();

            Collider[] hits = Physics.OverlapSphere(m_Controller.transform.position, m_TargetFindRadius, m_TargetFindLayer);

            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent(out ITargetable target))
                    continue;

                // if (!IsInViewAngle(target))
                //     continue;

                result.Add(target);
            }

            return result;
        }

        ITargetable SelectNearestTarget(List<ITargetable> targets)
        {
            ITargetable nearest = null;
            float minDist = Mathf.Infinity;

            foreach (var target in targets)
            {
                if (target == null) continue;

                float dist = Vector3.Distance(m_Controller.transform.position, target.targetTransform.position);

                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = target;
                }
            }

            return nearest;
        }

        /// <summary>
        /// 타겟으로 회전하는 값 반환
        /// </summary>
        float CalculateYawToTarget(ITargetable target)
        {
            Vector3 dir = target.targetTransform.position - m_Controller.transform.position;
            dir.y = 0f;

            return Quaternion.LookRotation(dir).eulerAngles.y;
        }
        void UpdateTargetUI(ITargetable target)
        {
            if (target == null)
            {
                HideTargetMark();
            }
            else
            {
                m_TargetMarkUI.gameObject.SetActive(true);
                m_TargetMarkUI.transform.position = Camera.main.WorldToScreenPoint(target.targetTransform.position);
            }
        }
        public void HideTargetMark()
        {
            m_TargetMarkUI.gameObject.SetActive(false);
        }

        // 특정 각 이내에만 보임
        bool IsInViewAngle(ITargetable target)
        {
            Vector3 dir = target.targetTransform.position - m_Controller.transform.position;

            float angle = Vector3.Angle(dir, m_Controller.cameraHandler.baseAim * Vector3.forward);

            return angle >= m_ViewAngleMin && angle <= m_ViewAngleMax;
        }
        void OnDrawGizmos()
        {
            if (useDebugSphere)
            {
                Gizmos.DrawWireSphere(m_Controller.transform.position, m_TargetFindRadius);
            }
        }
        #endregion


    }
}
