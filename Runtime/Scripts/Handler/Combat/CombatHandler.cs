using System;
using System.Collections.Generic;
using Dave6.CharacterKit.Item;
using Dave6.ObjectPoolingSystem;
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
        PlayerCharacter m_Controller;
        #region 외부 프리팹 필드
        [Header("외부 프리팹 필드")]
        [SerializeField] GameObject m_MeleeHitPrefab;
        public GameObject meleeHitPrefab => m_MeleeHitPrefab;
        MeleeHitbox m_Hitbox;

        [SerializeField] GameObject crosshairPrefab;
        [SerializeField] GameObject targetMarkPrefab;
        Transform m_AimTargetTransform;
        #endregion

        


        #region 밀리 히트박스 필드
        [Header("밀리 히트 필드")]
        int m_ComboStep = 0;
        [SerializeField] const int m_ComboEnd = 3;
        [SerializeField] float m_MeleeImpulse = 10f;
        Timer m_StepTimer;                              // 각 공격은 0.8 ~ 1.2초 걸림
        float m_StepDuration = 3f;
        Timer m_HitboxExistTimer;
        float m_HitboxDuration = 0.3f;
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

        public bool attacking {get; set;}
        bool m_AttackFinished = true;
        public bool reloading = false;
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

            CrosshairCanvas crosshair = m_Controller.InstantiatePrefabSetParent(crosshairPrefab).GetComponent<CrosshairCanvas>();
            m_BodyCrosshairUI = crosshair.bodyCrosshairUI;
            m_CameraCrosshairUI = crosshair.cameraCrosshairUI;

            m_TargetMarkUI = m_Controller.InstantiatePrefabSetParent(targetMarkPrefab).GetComponent<TargettingCanvas>().targettingUI;
            m_TargetMarkUI.gameObject.SetActive(false);

            m_AimTargetTransform ??= transform.Find("AimTarget");

            if (m_AimTargetTransform == null)
            {
                Debug.Log("AimTarget Transform 못찾음");
            }

        }

        public void OnLateUpdate()
        {
            if (m_Controller.equipHandler.HasFirearm())
            {
                var curFirearm = m_Controller.equipHandler.selectedFirearm as Firearm;

                Vector3 muzzlePos = curFirearm.GetMuzzle().position;
                // Pitch 계산!!
                float targetPitch = m_Controller.mover.CalcTargetPitchByAimPoint(muzzlePos);
                m_Controller.mover.SmoothPitchUpdate(targetPitch, Time.deltaTime);
                m_Controller.mover.CharacterAimUpdate();

                Vector3 origin = muzzlePos;
                Vector3 direction = m_Controller.mover.characterAim * Vector3.forward;
                m_CharacterAimPoint = m_Controller.CalculateAimPoint(origin, direction, m_Controller.characterAimLayerMask);

                m_BodyCrosshairUI?.LateUpdateCrosshair(m_CharacterAimPoint);
            }

            var baseAimPoint = m_Controller.cameraHandler.baseAimPoint;            
            m_AimTargetTransform.position = baseAimPoint;
            m_CameraCrosshairUI?.LateUpdateCrosshair(baseAimPoint);
        }

        public void RegisterCombat(PlayerCharacter controller)
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
            if (!m_AttackFinished) return false;
            m_AttackFinished = false;
            attacking = true;

            m_Controller.attackTimer.RestartTimer(); // Attack State 유지 시간 갱신

            m_HitboxExistTimer.RestartTimer();      // 히트박스 유지시간
            m_Hitbox.gameObject.SetActive(true);

            ComboCount();

            return true;
        }

        void ComboCount()
        {
            Debug.Log($"{m_ComboStep} Combo Attack!");
            m_StepTimer.RestartTimer(); // 콤보 시간 초기화
            string[] comboAnims = {"RightHook","LeftPunch","CrossPunch"};
            m_Controller.animatorHandler.ChangeAnimation(comboAnims[m_ComboStep], 0.1f);

            bool isLast = m_ComboStep == comboAnims.Length - 1;
            if (isLast)
            {
                ComboReset();
            }
            else
            {
                m_ComboStep++;
            }
        }
        #endregion

        #region 애니메이션 콜백 함수
        public void AddAttackImpulse()
        {
            m_Controller.impulseSpeed = m_MeleeImpulse;
        }
        public void HandleAttackEnd()
        {
            m_AttackFinished = true;
            attacking = false;
        }
        public void HandleReloadEnd()
        {
            reloading = false;
            var firearm = m_Controller.equipHandler.selectedFirearm as Firearm;
            firearm.RefillAmmo();
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



        float m_LastFireTime;
        /// <summary>
        /// state로 부터 요청을 받음
        /// 현재 장착 아이템 찾기 / 얻기
        /// 아이템에게
        ///     탄약 소모
        ///     머즐 트랜스폼 참조
        ///     투사체 구성 요청
        /// 성공 시
        ///     애니메이션
        ///     반동
        ///     타이머
        /// 
        /// 즉, 연출 및 실행 흐름만 진행
        /// </summary>
        public void TryFireProjectile()
        {
            // 아이템 찾기
            var curWeapon = m_Controller.equipHandler.selectedFirearm as Firearm;

            if (Time.time - m_LastFireTime < 60f / curWeapon.GetFireRate()) return;

            m_LastFireTime = Time.time;

            // 탄약 체크
            if (curWeapon.TryConsume())
            {
                // 투사체 생성
                ProjectileMover projectile = CreateProjectile(curWeapon);

                // 아이템에게 투사체 구성 요청
                curWeapon.BuildProjectile(projectile, characterAimPoint);

                // 반동 강도는 아이템이 가지고 있어야함
                float amplitude = 6f;
                m_Controller.cameraHandler.PlayRecoil(amplitude);
            }

            // 애니메이션 (성공 실패에 따라 달라져야함)
            m_Controller.animatorHandler.ChangeAnimation("Firearm_Fire", 0f, true);

            // 타이머 진행
            m_Controller.attackTimer.RestartTimer();
            // =========================
        }

        ProjectileMover CreateProjectile(Firearm firearm)
        {
            GameObject projectileOjb = ObjectPoolService.instance.Get(firearm.GetProjectilePrefab());
            var projectile = projectileOjb.GetComponent<ProjectileMover>();
            projectile.BindOwner(m_Controller);
            return projectile;
        }

        public void TryReload()
        {
            reloading = true;
            if (m_Controller.focusInput)
            {
                m_Controller.animatorHandler.ChangeAnimation("Firearm_Reload_Strafe", 0f, true);
            }
            else
            {
                m_Controller.animatorHandler.ChangeAnimation("Firearm_Reload_Freelook", 0f, true);
            }
        }
    }

    // 이건 안씀
    public class FireContext
    {
        //public IWeapon weapon;
        //public ISkill activeSkill;
        public List<IHitModifier> extraModifiers;
    }
}
