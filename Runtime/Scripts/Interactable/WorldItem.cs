using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.CharacterKit.Handler.Loadout;
using Dave6.ItemSystem.Domain.Container;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
namespace Dave6.CharacterKit.Interactable
{
    public class WorldItem : WorldActor
    {
        [SerializeField] string _ItemId;
        ItemInstance _Item;

        void Awake()
        {
            if (_Item != null) return;

            var factory = GameplayHub.Instance.Get<ItemFactory>();
            _Item = factory.CreateInstance(_ItemId);
        }

        /// <summary>
        /// 외부에서 세팅하는 경우
        /// </summary>
        public void Initialize(ItemInstance item) => _Item = item;

        public override bool CanInteract(IInteractor interactor)
        {
            return base.CanInteract(interactor);
        }

        public override string GetPromptText(IInteractor interactor)
        {
            return "Pickup";
        }

        protected override void OnInteract(IInteractor interactor)
        {
            Debug.Log("Pickup Item");
            // ILoadoutProvider 같은 인터페이스는 제네릭에 못써서 그냥 일반 클래스 씀
            var loadout = GameplayHub.Instance.Get<PlayerLoadout>();
            var result = loadout.Add(_Item, ExtensionRole.Inventory);
            if (!result.Success)
            {
                Debug.Log($"result: {result.Error}");
                return;
            }

            Destroy(gameObject);
        }
    }
    /*
    드랍 아이템은 이런식으로
    var item = factory.CreateInstance("sword_001");

    var go = Instantiate(worldItemPrefab, pos, rot);
    go.GetComponent<WorldItemActor>().Initialize(item);
    */
}
