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

        StatHandler m_StatHandler;
        public StatHandler statHandler => m_StatHandler;

        public ResourceStat health { get; set; }
        #endregion

        #region Action StateMachine
        MinimumStateMachine m_ActionStateMachine;
        #endregion

        #region CombatHandler
        CombatHandler m_CombatHandler;
        public CombatHandler combatHandler => m_CombatHandler;
        #endregion

        #region GameFlow
        [SerializeField] string LobbySceneName = "Lobby";
        #endregion

        #region Item & Inventory
        Inventory m_Inventory;
        public Inventory inventory => m_Inventory;
        OwnedItem m_SelectedItem;
        int m_CurrentIndex = -1;
        int m_PrevIndex;


        public float inputScroll => m_Input.InputScroll;

        #endregion

        #region Interact field
        public Transform origin => transform;
        #endregion


        public override void Awake()
        {
            base.Awake();
            InitializeStat();
            m_CombatHandler = GetComponent<CombatHandler>();
            m_CombatHandler.RegisterCombat(this);
            m_Inventory = new();
        }

        public override void Start()
        {
            health = m_StatHandler.GetHealthStat();
            m_Input.EnablePlayerAction();
            // 애니메이션 이벤트 바인딩
            m_AnimEventProxy.onAttackFinishEvent += animatorHandler.OnAttackAnimationEnd;
            m_AnimEventProxy.onAttackImpulseEvent += animatorHandler.OnAttackImpulse;

            // 상태 처리
            SetupStateMachine();

            m_LocomotionStateMachine.SetState(m_LocomotionStateMachine.GetStateByType(typeof(FreeLookState)));
            m_ActionStateMachine.SetState(m_ActionStateMachine.GetStateByType(typeof(ActionIdleState)));
        }

        public override void Update()
        {
            if (m_Paused) return;
            m_LocomotionStateMachine.Update();

            m_Mover.OnUpdate();

            m_ActionStateMachine.Update();
            CheckInteract();
            HandleSelectionItem();
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

            m_CameraHandler.OnLateUpdate();
            m_CombatHandler.OnLateUpdate();

            m_ActionStateMachine.LateUpdate();

        }

        protected override void SetupStateMachine()
        {
            if (showInitialDebug)
            {
                Debug.Log("상태 초기화");
            }

            // Locomotion
            m_LocomotionStateMachine = new();
            var freeLook = new FreeLookState(this);
            var strafeMove = new StrafeMoveState(this);
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

        }

        #region Stat System
        public void InitializeStat()
        {
            m_StatHandler = new StatHandler(m_StatDatabase);
            m_StatHandler.InitializeStat();
        }
        public void Accept(IStatInvoker invoker)
        {
            invoker.Invoke(this);
        }
        public void CheckHealth()
        {
            ResourceStat health = statHandler.GetHealthStat();

            if (health.currentValue <= 0)
            {
                ResetHealth();
                SceneDirector.instance.RequestSceneLoad(LobbySceneName, LobbySceneName + "Enter");
                // 로비로 돌아가기
            }
        }
        public void ResetHealth()
        {
            statHandler.GetHealthStat().ResetCurrentValue();
        }
        #endregion

        #region 상태 제어
        public bool enterAttackFlag;
        public bool exitMeleeFlag;
        public bool exitRangeFlag;
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
        #endregion

        #region Inventory

        public void HandleSelectionItem()
        {
            var items = m_Inventory.items;

            if (items.Count == 0)
            {
                m_SelectedItem = null;
                m_CurrentIndex = -1;
                return;
            }
            if (inputScroll == 0) return;
            int prevIndex = m_CurrentIndex;

            if (m_CurrentIndex < 0)
            {
                m_CurrentIndex = 0;
            }
            else if (inputScroll > 0)
            {
                // select next
                m_CurrentIndex = (m_CurrentIndex + 1) % items.Count;
            }
            else if (inputScroll < 0)
            {
                // select prev
                m_CurrentIndex = (m_CurrentIndex - 1 + items.Count) % items.Count;
            }
            if (prevIndex != m_CurrentIndex)
            {
                Debug.Log($"Current select index: {m_CurrentIndex}");
                m_SelectedItem = items[m_CurrentIndex];
                Debug.Log($"Selected: {m_SelectedItem.definition.displayName}");
                m_PrevIndex = m_CurrentIndex;
            }
        }
        public void DropSelected()
        {
            if (m_SelectedItem == null) return;
            m_Inventory.Drop(m_SelectedItem, transform.position);
            m_SelectedItem = null;
            m_CurrentIndex = -1;
        }
        #endregion
    }
}
