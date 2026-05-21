using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Inst { get; private set; }
   
    private void Awake()
    {
        Inst = this;

        
       LoadFullData();
    }

    [Serializable]
    private class SerializationWrapper<T>
    {
        public List<T> items; // JSON 파일의 루트 키 이름이 "items"여야 함
    }
    public Dictionary<string, MonsterData> MonsterDataList { get; private set; } = new Dictionary<string, MonsterData>();
    public Dictionary<string, ItemData> ItemDataList { get; private set; } = new Dictionary<string, ItemData>();
    public Dictionary<string, BulletData> BulletDataList { get; private set; } = new Dictionary<string, BulletData>();
    public Dictionary<string, DropTableData> DropTableDataList { get; private set; } = new Dictionary<string, DropTableData>();
    private Dictionary<string, T> LoadData<T>(string tableName) where T : GameDataBase
    {
        // 1. 경로 설정 (확장자 .json 제외!)
        // Resources/JsonOutput 폴더
        string resourcePath = $"JsonOutput/{tableName}";

        // 2. 리소스 로드
        TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);

        // 3. 파일 존재 여부 체크
        if (textAsset == null)
        {
            Debug.LogError($"[Error] 리소스를 찾을 수 없습니다: Resources/{resourcePath}");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = textAsset.text;

            // 4. JsonUtility용 Wrapper 트릭 적용
            string wrappedJson = "{\"items\":" + jsonString + "}";
            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper != null && wrapper.items != null)
            {
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다.");
                // ToDictionary를 사용하려면 각 클래스(T)에 Id 필드가 있어야 합니다.
                return wrapper.items.ToDictionary(item => item.Id.ToString());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} JSON 로드 오류] {ex.Message}");
        }

        return new Dictionary<string, T>();
    }
    public void LoadMonsterData(string jsonPath) {
        MonsterDataList = LoadData<MonsterData>(jsonPath);
    }
    public void LoadItemData(string jsonPath)
    {
        ItemDataList = LoadData<ItemData>(jsonPath);
    }
    public void LoadBulletData(string jsonPath)
    {
        BulletDataList = LoadData<BulletData>(jsonPath);
    }
    public void LoadDropTableData(string jsonPath)
    {
        DropTableDataList = LoadData<DropTableData>(jsonPath);
    }
    public MonsterData GetMonsterData(string id) {

        if (MonsterDataList == null || string.IsNullOrEmpty(id)) return null; 
        return MonsterDataList.TryGetValue(id, out var data) ? data : null;
    
    }
    public ItemData GetItemData(string id)
    {

        if (ItemDataList == null || string.IsNullOrEmpty(id)) return null;
        return ItemDataList.TryGetValue(id, out var data) ? data : null;

    }
    public BulletData GetBulletData(string id)
    {

        if (BulletDataList == null || string.IsNullOrEmpty(id)) return null;
        return BulletDataList.TryGetValue(id, out var data) ? data : null;

    }
    public DropTableData GetDropTableData(string id)
    {

        if (DropTableDataList == null || string.IsNullOrEmpty(id)) return null;
        return DropTableDataList.TryGetValue(id, out var data) ? data : null;

    }
    public void LoadFullData()
    {
        LoadMonsterData("MonsterData");
        LoadItemData("ItemData");
        LoadBulletData("BulletData");
        LoadDropTableData("DropTableData");
    }
}
