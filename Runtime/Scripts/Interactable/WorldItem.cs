using Dave6.CharacterKit.GameFlow;
using UnityEngine;
namespace Dave6.CharacterKit.Interactable
{
    public class WorldItem : WorldActor
    {
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
            // world Item이 Container에 배치되어야함
            // 인스턴스..를 가지고 있어야할까?

        }
    }
}
