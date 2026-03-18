using System.Collections.Generic;
using UnityEngine;

namespace Dave6.CharacterKit.Handler.Interact
{
    // 탐색만 진행
    public class BaseInteractor : MonoBehaviour, IInteractor
    {
        protected List<IInteractable> m_Interactables = new();
        protected IInteractable m_TargetInteractable;

        void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;
            if (!m_Interactables.Contains(interactable))
            {
                m_Interactables.Add(interactable);
            }
        }
        void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<IInteractable>(out var interactable)) return;
            m_Interactables.Remove(interactable);

            if (m_TargetInteractable == interactable)
            {
                m_TargetInteractable = null;
            }
        }
    }

    public interface IInteractable
    {
        void Interact(IInteractor interactor);
    }

    public interface IInteractor
    {
    }
}