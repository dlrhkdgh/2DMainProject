using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _townprefab;
    private GameObject _town;
    public static GameManager Inst { get; private set; }
    public int Gold { get; set; } = 1000;
    public Dictionary<string, int> _inventoryDic = new Dictionary<string, int>();
    public Action<Dictionary<string, int>, Transform> OnItemSell;
    public Action<Dictionary<string, ShopItemData>, Transform> OnItemBuy;
    public string _playerSkillId = "bomb_normal_01";
    private void Awake()
    {
        Inst = this;
        _town = Instantiate(_townprefab, Vector3.zero,Quaternion.identity, transform);
    }
    void Start()
    {
        StartMainMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartMainMenu() {

        UIManager.Inst.OpenLobbyUI();
    }
    public void StartGame()
    {
        GoToTown();
        UIManager.Inst.CloseLobbyUI();
    }
    public bool AddGold(int getGold) {

        int newGold = Gold + getGold;
        if (newGold<0)
        {
            return false;
        }
        else {
        Gold= newGold;
            return true;
        
        }
    }
    public void AddInventory(string itemId, int itemCount) {

        if (string.IsNullOrEmpty(itemId) || itemCount == 0) return;

        if (_inventoryDic.ContainsKey(itemId))
        {
            _inventoryDic[itemId] = _inventoryDic[itemId] + itemCount;
            //DebugPrintInventory();
        }
        else {
        _inventoryDic.Add(itemId, itemCount);
            //DebugPrintInventory();
        }
    }
    public void DebugPrintInventory()
    {
        if (_inventoryDic == null || _inventoryDic.Count == 0)
        {
            return;
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("============  CURRENT INVENTORY LIST ============");
        sb.AppendLine($"총 아이템 종류 수: {_inventoryDic.Count}종");
        sb.AppendLine("--------------------------------------------------");

       
        foreach (KeyValuePair<string, int> item in _inventoryDic)
        {
            sb.AppendLine($"▶ ID: {item.Key,-15} | 수량: {item.Value}개");
        }

        sb.AppendLine("==================================================");

        Debug.Log(sb.ToString());
    }
    public void StartStage(int stageNum) 
    {
        ExitTown();
        ResetPlayerPosition();
        StageManager.Inst.StageStart(stageNum);
        UIManager.Inst.OpenInFeildUI();
    }
    public void FinishStage(bool isClear) 
    {
        PauseGame();
        UIManager.Inst.CloseInFeildUI();
        StageManager.Inst._isClear = isClear;
        UIManager.Inst.OpenResultUI();
        StageManager.Inst.StageFinish();
    }
    public void ExitStage() 
    {
        UIManager.Inst.CloseResultUI();
        GoToTown();
        ResumeGame();
    }
    public void GoToTown() {
        ResetPlayerPosition();
        Player.Inst.ResetHp();
        _town.gameObject.SetActive(true);
        UIManager.Inst.OpenMainUI();
    }
    public void ExitTown() 
    {
        _town.gameObject.SetActive(false);
        UIManager.Inst.CloseMainUI();

    }
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    public void SellItem(string id, Transform layoutParent) {
        if (_inventoryDic.ContainsKey(id) && _inventoryDic[id] > 0)
        {
            ItemData data = DataManager.Inst.GetItemData(id);

            Gold = Gold + data.SellingPrice;
            _inventoryDic[id]--;
            if (_inventoryDic[id] <= 0)
            {
                _inventoryDic.Remove(id);
            }
            OnItemSell?.Invoke(_inventoryDic, layoutParent);
        }
    }
    public void BuyItem(string id, Transform layoutParent) 
    {
        if (DataManager.Inst.ShopItemDataList.ContainsKey(id)) 
        {
            int itemPrice = DataManager.Inst.ShopItemDataList[id].SellingPrice;
            if (itemPrice <= Gold) 
            {
                Gold = Gold - itemPrice;
                if (_inventoryDic.ContainsKey(id))
                {
                    _inventoryDic[id]++;
                }
                else 
                {
                    _inventoryDic.Add(id, 1);
                }
                OnItemSell?.Invoke(_inventoryDic, layoutParent);
            }
        }
    }
    public void ResetPlayerPosition() 
    {
        Player.Inst.transform.position = Vector3.zero;    
    }
    public void UpgradeBullet()
    {
        var currentBulletData = DataManager.Inst.GetBulletData(Player.Inst._bulletId);
        var bulletUpgradeTableData = DataManager.Inst.GetBulletUpgradeTableData(Player.Inst._bulletId);
        string nextBulletId = currentBulletData.NextBulletId;
        var nextBulletData = DataManager.Inst.GetBulletData(nextBulletId);
        if (CheckUpgradeAvailable(nextBulletData,bulletUpgradeTableData)) 
        {
            UpgradePlayerBullet(bulletUpgradeTableData);
            Player.Inst._bulletId = nextBulletId;
        }
    }
    public bool CheckUpgradeAvailable(BulletData nextBulletData, BulletUpgradeTableData bulletUpgradeTableData)
    {
        if (nextBulletData == null || bulletUpgradeTableData == null) return false;
        if (bulletUpgradeTableData.CoinMount > Gold) return false;

        if (!HasEnoughIngredient(bulletUpgradeTableData.ItemId1, bulletUpgradeTableData.Item1Amount)) return false;
        if (!HasEnoughIngredient(bulletUpgradeTableData.ItemId2, bulletUpgradeTableData.Item2Amount)) return false;
        if (!HasEnoughIngredient(bulletUpgradeTableData.ItemId3, bulletUpgradeTableData.Item3Amount)) return false;
        
        return true;
    }
    private bool HasEnoughIngredient(string itemId, int requiredAmount)
    {
        if (string.IsNullOrEmpty(itemId) || requiredAmount <= 0) return true;
       
        _inventoryDic.TryGetValue(itemId, out int currentAmount);
        
        if (currentAmount < requiredAmount) return false;

        return true;
    }
    public void UpgradePlayerBullet(BulletUpgradeTableData bulletUpgradeTableData)
    {
        Gold -= bulletUpgradeTableData.CoinMount;
        ConsumeIngredient(bulletUpgradeTableData.ItemId1, bulletUpgradeTableData.Item1Amount);
        ConsumeIngredient(bulletUpgradeTableData.ItemId2, bulletUpgradeTableData.Item2Amount);
        ConsumeIngredient(bulletUpgradeTableData.ItemId3, bulletUpgradeTableData.Item3Amount);
    }
    private void ConsumeIngredient(string itemId, int amount)
    {
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return;
      
        if (_inventoryDic.ContainsKey(itemId))
        {
            _inventoryDic[itemId] -= amount;
            if (_inventoryDic[itemId] <= 0)
            {
                _inventoryDic.Remove(itemId);
            }
        }
    }
}
