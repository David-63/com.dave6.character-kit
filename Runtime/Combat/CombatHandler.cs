using System;
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
        PlayerController m_Controller;
        public PlayerController controller => m_Controller;
        #region 외부 프리팹 필드
        [SerializeField] GameObject m_MeleeHitPrefab;
        public GameObject meleeHitPrefab => m_MeleeHitPrefab;
        MeleeHitbox m_Hitbox;

        [SerializeField] GameObject m_ProjectilePrefab;
        public GameObject projectilePrefab => m_ProjectilePrefab;
        #endregion

        public Transform muzzle;


        #region 밀리 히트박스 필드
        int m_ComboStep = 0;
        [SerializeField] const int m_ComboEnd = 3;
        Timer m_StepTimer;                              // 각 공격은 0.8 ~ 1.2초 걸림
        float m_StepDuration = 3f;
        Timer m_HitboxExistTimer;
        float m_HitboxDuration = 0.5f;

        
        #endregion

        void Awake()
        {
            m_Controller = GetComponent<PlayerController>();

            // 타이머 세팅
            m_StepTimer = new Countdown(m_StepDuration);
            m_StepTimer.OnTimerStop += ComboReset;
            m_HitboxExistTimer = new Countdown(m_HitboxDuration);
            m_HitboxExistTimer.OnTimerStop += HitboxReset;

            m_Hitbox = m_Controller.InstantiatePrefab(m_MeleeHitPrefab).GetComponent<MeleeHitbox>();
            if (m_Hitbox == null)
            {
                Debug.Log("m_Hitbox 초기화 안됨");
            }
            m_Hitbox.Initialize(m_Controller);
            m_Hitbox.transform.localPosition = new Vector3(0, 1, 1);
            m_Hitbox.gameObject.SetActive(false);
        }

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
            if (!m_Controller.animatorHandler.attackReady) return false;
            m_Controller.animatorHandler.attackReady = false;
            
            bool isComboEnd = false;
            // 충돌체 활성화
            m_Hitbox.gameObject.SetActive(true);

            // 콤보에 따른 애니메이션 호출
            if (m_ComboStep == 0)
            {
                controller.animatorHandler.ChangeAnimation("LeftPunch", 0.1f);
            }
            else if (m_ComboStep == m_ComboEnd -1)
            {
                controller.animatorHandler.ChangeAnimation("CrossPunch");
                isComboEnd = true;
            }
            else
            {
                controller.animatorHandler.ChangeAnimation("RightHook");
            }

            // 움직임 제어
            controller.movementLocked = true;

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

    }
}
