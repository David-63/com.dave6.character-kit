using Dave6.CharacterKit.GameFlow;
using UnityEngine;

namespace Dave6.CharacterKit.Interactable
{
    /// <summary>
    /// 하는 일:
    /// - 기본 Prompt 제공
    /// - 기본 CanInteract 처리
    /// - 활성/비활성 관리
    /// - 공통 Interact flow 제공
    /// </summary>
    public abstract class WorldActor : MonoBehaviour, IInteractable
    {
        [Header("Interaction")]
        [SerializeField] protected string _PromptText = "Interact";
        [SerializeField] protected bool _IsEnabled = true;

        public virtual bool CanInteract(IInteractor interactor) => _IsEnabled;

        public virtual  string GetPromptText(IInteractor interactor) => _PromptText;

        public virtual void Interact(IInteractor interactor)
        {
            if (!CanInteract(interactor)) return;
            OnInteract(interactor);
        }

        protected abstract void OnInteract(IInteractor interactor);
        public virtual void SetInteractable(bool value) => _IsEnabled = value;
    }
}