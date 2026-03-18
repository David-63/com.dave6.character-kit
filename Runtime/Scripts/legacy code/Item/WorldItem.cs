// using Dave6.GameStateFlow;
// using UnityEngine;

// namespace Dave6.CharacterKit.Item
// {
//     /// <summary>
//     /// 월드상에서 상호작용 할 수 있는 아이템
//     /// </summary>
//     public class WorldItem : MonoBehaviour, IInteractable
//     {
//         [SerializeField] ItemDefinition m_Definition;
//         [SerializeField] int m_Stack = 1;

//         public ItemDefinition definition => m_Definition;
//         public int stack => m_Stack;
//         bool consumed = false;

//         public void Interact(IInteractor interactor)
//         {
//             // 이 객체를 인벤토리에서 픽업하게 함수 호출함
//             // IInventoryUser 이런걸로 체크하게 변경해도 좋을듯?
//             if (interactor is PlayerCharacter owner)
//             {
//                 if (consumed) return;
//                 consumed = true;
//                 interactor.ClearInteractable();

//                 if (owner.inventory.AddOwned(this))
//                 {
//                     // 직후 이 객체는 제거됨
//                     Destroy(gameObject);
//                 }
//             }
//         }
//     }
// }
