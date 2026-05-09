using System;
using Dave6.CharacterKit.GameFlow;
using Dave6.ItemSystem.Domain.Item;
using UnityEngine;
using UnityEngine.UIElements;

namespace Dave6.CharacterKit.UnityUI.ItemSystem
{
    public class ItemInspector : MonoBehaviour
    {
        VisualElement _Root;
        Label _Title;
        VisualElement _SectionLayer;
        ItemInstance _TargetItem;

        void Awake()
        {
            var doc = GetComponent<UIDocument>();
            _Root = doc.rootVisualElement.Q<VisualElement>("item-inspector-root");
            Initialize();
            GameplayHub.Instance.Register(this);
        }
        public virtual void Initialize()
        {
            _Title = _Root.Q<Label>("item-title");
            _SectionLayer = _Root.Q<VisualElement>("section-layer");
            Hide();
        }
        public void Bind(ItemInstance item)
        {
            ClearSections();
            _TargetItem = item;
            _Title.text = item.Definition.DisplayName;

            AddPreview();
            AddStatSection();
            AddSocketSection();
        }
        public void Unbind()
        {
            ClearSections();
            _TargetItem = null;
        }
        public void Show()
        {
            if (_TargetItem == null) return;
            Debug.Log($"{_TargetItem.Definition.DisplayName} Inspector");
            _Root.style.display = DisplayStyle.Flex;
        }
        public void Hide()
        {
            Debug.Log("Close Inspector");
            _Root.style.display = DisplayStyle.None;
        }


        void ClearSections()
        {
            
        }

        void AddPreview()
        {
            return;
            // ItemFactory 로부터 _Database.GetItemEntry(id).ItemDefinitionAsset 참조해서 스프라이트 혹은 텍스쳐 가져오기
            // 위 기능을 viewFectory에서 만들라는데

        }
        void AddStatSection()
        {
            return;
            // 스텟을 가지고 있는지 체크해야함..

            // ItemStatApplier 여기에 참조해서 id에 해당하는 스텟이 있는지 검사
            // 있다면 있는 만큼 텍스트 라벨을 추가해줘야함
        }
        void AddSocketSection()
        {
            return;
            // 아이템 definition에 OwnershipDescriptors 컨테이너가 있는지 체크

            //AddAddonSection();
        }
        void AddAddonSection()
        {
            return;
            // allow slot이 있는지 체크
            // inventory role에 해당하는 collection의 모든 컨테이너를 조회해서 
            // allow slot에 해당하는 아이템 찾기

            // 찾은 아이템들을 리스트로 보여주기
        }

        

        
    }
}