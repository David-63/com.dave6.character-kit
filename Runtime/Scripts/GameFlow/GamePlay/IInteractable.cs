namespace Dave6.CharacterKit.GameFlow
{
    public interface IInteractable
    {
        bool CanInteract(IInteractor interactor);
        void Interact(IInteractor interactor);
        /// <summary>
        /// UI 출력용
        /// </summary>
        string GetPromptText(IInteractor interactor);
    }

}
