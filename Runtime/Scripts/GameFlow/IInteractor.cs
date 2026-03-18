
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow
{
    public interface IInteractor
    {
        Transform Origin { get; }
        void ClearInteractable();
    }

}
