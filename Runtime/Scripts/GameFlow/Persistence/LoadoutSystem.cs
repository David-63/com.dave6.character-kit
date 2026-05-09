using System;
using System.Collections.Generic;
using System.IO;
using Dave6.CharacterKit.GameFlow.Factory;
using Dave6.CharacterKit.Handler.Loadout;
using Dave6.CharacterKit.UnityUI.ItemSystem;
using Dave6.ItemSystem.Application.Container;
using Dave6.ItemSystem.Application.Item;
using Dave6.ItemSystem.Application.Mapper;
using Dave6.ItemSystem.Domain.Item;
using Dave6.ItemSystem.Persistence.Dto;
using Dave6.ItemSystem.Persistence.Mapper;
using UnityEngine;

namespace Dave6.CharacterKit.GameFlow
{
    public class LoadoutSystem : MonoBehaviour
    {
        [Header("Dependencies")]
        //[SerializeField] ItemDatabaseAsset _DatabaseAsset;
        [SerializeField] List<ItemDefinitionAsset> _StarterItems;

        LoadoutService _LoadoutService;
        string _SavePath;

        ILoadoutProvider _PlayerLoadout;

        public Action OnLoadComplete;

        void Awake()
        {
            // DB 로드 및 서비스 초기화
            var itemFactory = GameplayHub.Instance.Get<ItemFactory>();
            _LoadoutService = new LoadoutService(itemFactory);

            _SavePath = Path.Combine(Application.persistentDataPath, "Player_Loadout.json");
            GameplayHub.Instance.Register(this);
        }

        public void BindContext(ILoadoutProvider provider) => _PlayerLoadout = provider;

        public void Save()
        {
            var saveData = _LoadoutService.ExportLoadout(_PlayerLoadout.GetContext());

            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(_SavePath, json);
            Debug.Log("로드아웃 저장");
        }

        public void Load()
        {
            // 1 세이브 파일이 없음
            if (!File.Exists(_SavePath))
            {
                Debug.Log("초기 세이브 데이터 없음");
                // 초기 아이템 지급 로직
                GiveInitializeItems();
                Save();
                return;
            }

            // 2 불러오기 수행
            string json = File.ReadAllText(_SavePath);
            var saveData = JsonUtility.FromJson<SaveData>(json);
            _LoadoutService.ImportLoadout(_PlayerLoadout, saveData);

            // UI 갱신 요청
            OnLoadComplete?.Invoke();

            Debug.Log("로드아웃 불러오기");
        }

        void GiveInitializeItems()
        {
            foreach (var iDef in _StarterItems)
            {
                Debug.Log($"초기 아이템 지급: {iDef.DisplayName}");

                _PlayerLoadout.Add(new ItemInstance(iDef.Create()), ExtensionRole.Inventory);
            }
        }
    }

    public class LoadoutComposition
    {
        public LoadoutSystem Manager;
        public PlayerLoadout Loadout;
        public LoadoutMain UI;
    }


}