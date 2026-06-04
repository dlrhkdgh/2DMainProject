
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopUI : UIBase
{
    [SerializeField] LevelUpRewardSlot _playerBulletSlot;
    [SerializeField] LevelUpRewardSlot _upgradeBulletSlot;
    [SerializeField] Text Text_Gold;
    [SerializeField] Text Text_NeedGold;
    [SerializeField] GameObject _ingredientSlotPrefab;
    [SerializeField] GameObject _InvenitemSlotPrefab;
    [SerializeField] private Transform _ingredientLayoutGroupParent;
    [SerializeField] private Transform _playerInvenLayoutGroupParent;

    public BulletData _currentBulletData;
    public BulletUpgradeTableData _currentUpgradeTableData;
    public string _playerBulletId;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        if (Player.Inst != null)
        {
            _playerBulletId = Player.Inst._bulletId;
            _currentBulletData = DataManager.Inst.GetBulletData(_playerBulletId);
            _currentUpgradeTableData = DataManager.Inst.GetBulletUpgradeTableData(_currentBulletData.UpgradeIngredientTableId);

            Text_NeedGold.text = _currentUpgradeTableData.CoinMount.ToString();
            DrawUpgradeIngredientSlot(_currentUpgradeTableData, _ingredientLayoutGroupParent);            
            DrawPlayerInventory(GameManager.Inst._inventoryDic, _playerInvenLayoutGroupParent);
            SetPlayerBulletSlot();
            SetUpgradeBulletSlot();
        }
    }
    public void DrawUpgradeIngredientSlot(BulletUpgradeTableData data, Transform layoutParent) 
    {
        if (data.ItemId1 != null) 
        {
            CreateAndSetupUpgradeIngredientSlot(data.ItemId1, data.Item1Amount, layoutParent);
        }
        if (data.ItemId2 != null)
        {
            CreateAndSetupUpgradeIngredientSlot(data.ItemId2, data.Item2Amount, layoutParent);
        }
        if (data.ItemId3 != null)
        {
            CreateAndSetupUpgradeIngredientSlot(data.ItemId3, data.Item3Amount, layoutParent);
        }
    }
    private void CreateAndSetupUpgradeIngredientSlot(string itemId, int count, Transform layoutParent) 
    {
        if (_ingredientSlotPrefab == null) return;

        ItemData itemData = DataManager.Inst.GetItemData(itemId);
        if (itemData == null) return;

        GameObject newSlot = Instantiate(_ingredientSlotPrefab, layoutParent);
        newSlot.SetActive(true);

        UIButtonBase targetSlot = newSlot.GetComponent<UIButtonBase>();////////////////

        ResourceManager.Inst.LoadSprite(itemData.IconPath, (loadedSprite) =>
        {
            if (targetSlot == null || targetSlot.gameObject == null)
            {
                // Debug.Log("이미지를 불러왔으나 슬롯이 이미 파괴되어 연산을 취소합니다.");
                return;
            }
            targetSlot.ChangeButtonText(count.ToString());
            // 이미지 컴포넌트 자체도 한 번 더 체크
            if (targetSlot.Image_Base == null) return;
            if (loadedSprite != null)
            {
                targetSlot.Image_Base.sprite = loadedSprite;
            }
        });
    }
    public void DrawPlayerInventory(Dictionary<string, int> targetInven, Transform layoutParent)
    {

        foreach (Transform child in layoutParent)
        {
            Destroy(child.gameObject);
        }

        if (targetInven != null)
        {
            foreach (KeyValuePair<string, int> item in targetInven)
            {
                CreateAndSetupSlot(item.Key, item.Value, layoutParent);
            }
        }
        ChangeGoldText();
    }

    private void CreateAndSetupSlot(string itemId, int count, Transform layoutParent)
    {
        if (_InvenitemSlotPrefab == null) return;

        ItemData itemData = DataManager.Inst.GetItemData(itemId);
        if (itemData == null) return;

        GameObject newSlot = Instantiate(_InvenitemSlotPrefab, layoutParent);
        newSlot.SetActive(true);

        UIButtonBase targetSlot = newSlot.GetComponent<UIButtonBase>();////////////////
       
        ResourceManager.Inst.LoadSprite(itemData.IconPath, (loadedSprite) =>
        {
            if (targetSlot == null || targetSlot.gameObject == null)
            {
                return;
            }

            targetSlot.ChangeButtonText(count.ToString());
            if (targetSlot.Image_Base == null) return;
            if (loadedSprite != null)
            {
                targetSlot.Image_Base.sprite = loadedSprite;
            }
        });
    }
    void ChangeGoldText()
    {
        Text_Gold.text = GameManager.Inst.Gold.ToString();
    }
    public void SetPlayerBulletSlot() 
    {
        _playerBulletSlot.ChangeNameText(_currentBulletData.Name);
        _playerBulletSlot.ChangeDescriptionText(_currentBulletData.Description);
        ResourceManager.Inst.LoadSprite(_currentBulletData.IconPath, (loadedSprite) =>
        {
            if (_playerBulletSlot == null || _playerBulletSlot.gameObject == null)
            {
                return;
            }
            if (_playerBulletSlot.Image_Base == null) return;
            if (loadedSprite != null)
            {
                _playerBulletSlot.Image_Base.sprite = loadedSprite;
            }
        });
    }
    public void SetUpgradeBulletSlot() 
    {
       string nextBulletId = _currentBulletData.NextBulletId;
        var nextBulletData = DataManager.Inst.GetBulletData(nextBulletId);
        _upgradeBulletSlot.ChangeNameText(nextBulletData.Name);
        _upgradeBulletSlot.ChangeDescriptionText(nextBulletData.Description);
        ResourceManager.Inst.LoadSprite(nextBulletData.IconPath, (loadedSprite) =>
        {
            if (_upgradeBulletSlot == null || _upgradeBulletSlot.gameObject == null)
            {
                return;
            }
            if (_upgradeBulletSlot.Image_Base == null) return;
            if (loadedSprite != null)
            {
                _upgradeBulletSlot.Image_Base.sprite = loadedSprite;
            }
        });
    }
}
