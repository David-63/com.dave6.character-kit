using Dave6.CharacterKit.GameFlow;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit
{
    public class InteractPanel : MonoBehaviour
    {
        VisualElement _Root;
        Label _Key;
        Label _Text;
        void Awake()
        {
            var doc = GetComponent<UIDocument>();
            _Root = doc.rootVisualElement.Q<VisualElement>("main-root");
            _Key = _Root.Q<Label>("key");
            _Text = _Root.Q<Label>("text");

            _Key.text = "[F]";
            GameplayHub.Instance.Register(this);
        }

        public void Show(string text)
        {
            _Text.text = text;
            _Root.style.display = DisplayStyle.Flex;
            Debug.Log("Show");
        }

        public void Hide()
        {
            _Root.style.display = DisplayStyle.None;
            Debug.Log("Hide");
        }
    }
}
