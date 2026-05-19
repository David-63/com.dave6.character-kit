using System;
using Dave6.CharacterKit.GameFlow;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.CharacterKit.ItemStat;
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

            var asset = GameplayHub.Instance.Get<ItemFactory>().GetItemDefinitionAsset(item.Definition.ItemId);

            if (asset.Image != null)
            {
                AddPreview(asset.Image);
            }

            if (GameplayHub.Instance.Get<ItemStatApplier>().TryGetItemStat(item, out var statDef))
            {
                if (statDef.Modifiers.Count > 0) AddStatSection(statDef);
            }
            
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
            _SectionLayer.Clear();
            _Title.text = string.Empty;
        }

        void AddPreview(Texture image)
        {
            var section = new VisualElement();
            section.AddToClassList("i-section");

            section.style.width = 256f;
            section.style.height = 256f;

            var previewIcon = new Image
            {
                image = image
            };
            section.Add(previewIcon);

            _SectionLayer.Add(section);
        }
        void AddStatSection(ItemStatDefinition statDef)
        {
            var section = new VisualElement();
            section.AddToClassList("i-section");

            var title = new Label("Stats");
            section.Add(title);

            foreach (var modifier in statDef.Modifiers)
            {
                var label = new Label($"{modifier.Tag.TagName}: {modifier.Value}");
                section.Add(label);
            }
            _SectionLayer.Add(section);
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