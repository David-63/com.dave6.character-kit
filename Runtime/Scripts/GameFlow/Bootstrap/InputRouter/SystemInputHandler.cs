using Dave6.CharacterKit.Inputs;
using UnityEngine;
using UnityEngine.Events;

namespace Dave6.CharacterKit.GameFlow.Input
{
    // 디버깅용 임시 인풋
    public class SystemInputHandler : MonoBehaviour
    {
        [SerializeField] InputReader _Input;
        UnityAction _OnSave;
        UnityAction _OnLoad;

        public void Inject(UnityAction onSave, UnityAction onLoad)
        {
            _OnSave = onSave;
            _OnLoad = onLoad;
        }

        void OnEnable()
        {
            _Input.Save += HandleSave;
            _Input.Load += HandleLoad;
        }

        void OnDisable()
        {
            _Input.Save -= HandleSave;
            _Input.Load -= HandleLoad;
        }

        void HandleSave()
        {
            _OnSave?.Invoke();
        }

        void HandleLoad()
        {
            _OnLoad?.Invoke();
        }
    
    }
}