using Dave6.CharacterKit.Combat;
using Dave6.CharacterKit.Item;
using Dave6.CharacterKit.States;
using Dave6.GameStateFlow;
using Dave6.StateMachine;
using Dave6.StatSystem;
using Dave6.StatSystem.Interaction;
using Dave6.StatSystem.Stat;
using UnityEngine;

namespace Dave6.CharacterKit
{
    public class PlayerController : BasicPlayerController, IStatController, IStatReceiver, IInteractor
    {
        #region stat field
        [SerializeField] StatDatabase m_StatDatabase;
        public StatDatabase statDatabase => m_StatDatabase;

        public StatHandler statHandler { get; private set; }

        public ResourceStat myHealth { get; set; }
        #endregion

        #region Action StateMachine
        MinimumStateMachine m_ActionStateMachine;
        #endregion

        #region CombatHandler
        public CombatHandler combatHandler { get; private set; }
        #endregion

        #region GameFlow
        [SerializeField] string LobbySceneName = "Lobby";
        #endregion

        #region Item & Inventory
        Inventory m_Inventory;
        public Inventory inventory => m_Inventory;
        EquipHandler m_EquipHandler;

        public float inputScroll => m_Input.InputScroll;
        public bool equipInputTap {get; private set;}
        public bool dropInputTap {get; private set;}

        #endregion

        #region Interact field
        public Transform origin => transform;
        #endregion

        #region Stat Tag Reference

        [Header("참조 스탯 태그")]
        [SerializeField] StatTag m_HealthStatTag;
        [SerializeField] StatTag m_MoveSpeedStatTag;

        public ResourceStat playerHealth {get; private set;}

        #endregion


        public override void Awake()
        {
            base.Awake();
            InitCoreSystem();
            InitHandler();
        }

        #region 초기화

        void InitCoreSystem()
        {
            Init_StatHandler();
            combatHandler = GetComponent<CombatHandler>();
            combatHandler.RegisterCombat(this);
        }
        void InitHandler()
        {
            m_Inventory = new();
            m_EquipHandler = new(this, m_Inventory);
        }

        #endregion

        void OnDestroy()
        {
            m_Inventory.onItemEquipped -= m_EquipHandler.EquipItem;
            m_Inventory.onItemUnEquipped -= m_EquipHandler.UnequipItem;
            playerHealth.onCurrentValueChanged -= CheckHealth;
        }

        public override void Start()
        {
            BindEvent();

            // State Machine 설정 (가장 마지막에 할것)
            SetupStateMachine();
        }

        #region 외부 시스템 연결
        void BindEvent()
        {
            // 인벤토리 기능
            m_Inventory.onItemEquipped += m_EquipHandler.EquipItem;
            m_Inventory.onItemUnEquipped += m_EquipHandler.UnequipItem;
            // 인풋 기능
            m_Input.EnablePlayerAction();
            // 스텟 기능
            statHandler.TryGetStat(m_HealthStatTag, out var health);
            playerHealth = health as ResourceStat;
            playerHealth.onCurrentValueChanged -= CheckHealth;
            // 애니메이션 이벤트 바인딩
            m_AnimEventProxy.onAttackFinishEvent += animatorHandler.OnAttackAnimationEnd;
            m_AnimEventProxy.onAttackImpulseEvent += animatorHandler.OnAttackImpulse;
        }
        #endregion

        public override void Update()
        {
            if (m_Paused) return;
            m_LocomotionStateMachine.Update();
            mover.OnUpdate();
            m_ActionStateMachine.Update();

            CheckInteract();

            m_EquipHandler.OnUpdate();
            statHandler.OnUpdate();
        }

        public override void FixedUpdate()
        {
            if (m_Paused) return;
            m_LocomotionStateMachine.FixedUpdate();
            m_ActionStateMachine.FixedUpdate();
        }
        public override void LateUpdate()
        {
            ClearTapInput();

            if (m_Paused) return;
            m_LocomotionStateMachine.LateUpdate();

            cameraHandler.OnLateUpdate();
            combatHandler.OnLateUpdate();

            m_ActionStateMachine.LateUpdate();

        }


        protected override void InputEventBind()
        {
            if (showInitialDebug)
            {
                Debug.Log("인풋 초기화");
            }
            m_Input.Jump += (value) => jumpInput = value;
            m_Input.Aim += (value) => aimInput = value;
            m_Input.Shift += (value) => shiftInput = value;
            m_Input.Attack += (value) => attackInput = value;
            m_Input.AttackTap += () => attackInputTap = true;
            m_Input.InteractTap += () => interactInputTap = true;
            m_Input.EquipTap += () => equipInputTap = true;
            m_Input.DropTap += () => dropInputTap = true;
        }

        protected override void ClearTapInput()
        {
            attackInputTap = false;
            interactInputTap = false;
            equipInputTap = false;
            dropInputTap = false;
        }

        protected override void SetupStateMachine()
        {
            if (showInitialDebug)
            {
                Debug.Log("상태 초기화");
            }

            // Locomotion
            m_LocomotionStateMachine = new();
            statHandler.TryGetStat(m_MoveSpeedStatTag, out var moveStat);
            var freeLook = new FreeLookState(this, moveStat);
            var strafeMove = new StrafeMoveState(this, moveStat);
            m_LocomotionStateMachine.At(freeLook, strafeMove, new FuncPredicate(() => aimInput));
            m_LocomotionStateMachine.At(strafeMove, freeLook, new FuncPredicate(() => !aimInput));

            // Action
            m_ActionStateMachine = new();
            var actionIdle = new ActionIdleState(this);
            var actionMelee = new ActionMeleeState(this);
            var actionRange = new ActionRangeState(this);

            // 공격 진입
            m_ActionStateMachine.At(actionIdle, actionMelee, new FuncPredicate(() => attackInputTap && !aimInput));
            m_ActionStateMachine.At(actionIdle, actionRange, new FuncPredicate(() => aimInput));

            
            // 공격 전환
            m_ActionStateMachine.At(actionMelee, actionRange, new FuncPredicate(() => aimInput));
            m_ActionStateMachine.At(actionRange, actionMelee, new FuncPredicate(() => !aimInput && attackInputTap));

            
            // 공격 해제
            m_ActionStateMachine.At(actionMelee, actionIdle, new FuncPredicate(() => ConsumeExitMelee()));

            ///%%%
            m_ActionStateMachine.At(actionRange, actionIdle, new FuncPredicate(() => !aimInput && ConsumeExitRange()));


            // 초기 상태 설정
            m_LocomotionStateMachine.SetState(m_LocomotionStateMachine.GetStateByType(typeof(FreeLookState)));
            m_ActionStateMachine.SetState(m_ActionStateMachine.GetStateByType(typeof(ActionIdleState)));

        }

        #region Stat System
        public void Init_StatHandler()
        {
            statHandler = new StatHandler(m_StatDatabase);
            statHandler.InitializeStat();
        }
        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }
        public void CheckHealth()
        {
            Debug.Log($"player Helth: {playerHealth.currentValue}/{playerHealth.finalValue}");
            if (playerHealth.currentValue <= 0)
            {
                ResetHealth();
                SceneDirector.instance.RequestSceneLoad(LobbySceneName, LobbySceneName + "Enter");
                // 로비로 돌아가기
            }
        }
        public void ResetHealth()
        {
            playerHealth.ResetCurrentValue();
        }

        #endregion

        #region 상태 제어
        public bool enterAttackFlag { get; set; }
        public bool exitMeleeFlag { get; set; }
        public bool exitRangeFlag { get; set; }
        bool ConsumeExitMelee()
        {
            if (!exitMeleeFlag) return false;
            exitMeleeFlag = false;
            return true;
        }
        bool ConsumeExitRange()
        {
            if (!exitRangeFlag) return false;
            exitRangeFlag = false;
            return true;
        }
        #endregion

        #region Create Prefab
        public GameObject InstantiatePrefab(GameObject obj)
        {
            return Instantiate(obj);
        }
        public GameObject InstantiatePrefabSetParent(GameObject obj)
        {
            return Instantiate(obj, transform);
        }
        public GameObject InstantiatePrefab(GameObject obj, Vector3 position, Quaternion rotation)
        {
            return Instantiate(obj, position, rotation);
        }
        #endregion

        #region Interaction
        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                m_CurrentInteractable = interactable;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                if (m_CurrentInteractable == interactable)
                    m_CurrentInteractable = null;
            }
        }
        void CheckInteract()
        {
            if (interactInputTap && currentInteractable != null)
            {
                currentInteractable.Interact(this);
            }
        }
        public void ClearInteractable()
        {
            m_CurrentInteractable = null;
        }
        #endregion

        #region Inventory

        #endregion
    }
}
