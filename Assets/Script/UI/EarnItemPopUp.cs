using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum InventoryType
{
    Town,  
    Stage   
}
public class EarnItemPopUp : UIBase
{
   // [SerializeField] private string _itemSlotPrefabAddress = "Prefab/UI/EarnItemSlot1";
    [SerializeField] GameObject _itemSlotPrefab;
    [SerializeField] UIButtonBase Button_Exit;
    [SerializeField] private Transform _layoutGroupParent;
    [SerializeField] Text Text_Coin;

    private InventoryType _currentType;
    private Dictionary<string, int> _inven;
    
    private void OnEnable()
    {
        if (Button_Exit != null)
        {
            Button_Exit.BindOnClickButtonEvent(OnClick_ExitEarnItemPopUp);
        }
    }
    public void OpenInventory(InventoryType type) 
    {
      _currentType = type;
        this.gameObject.SetActive(true);

        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }
       
        if (_currentType == InventoryType.Town)
        {
            _inven = GameManager.Inst._inventoryDic; // 마을 전체 데이터
            if (Text_Coin != null) Text_Coin.text = GameManager.Inst.Gold.ToString("N0");
        }
        else if (_currentType == InventoryType.Stage)
        {
            _inven = StageManager.Inst._stageInventoryDic; // 스테이지 획득 데이터
            if (Text_Coin != null) Text_Coin.text = StageManager.Inst.StageGold.ToString("N0");
        }
        
        if (_inven != null)
        {
            SetEarnItemInventoryPopUpAsync().Forget();
        }
    }
    private async UniTaskVoid SetEarnItemInventoryPopUpAsync()
    {
        foreach (KeyValuePair<string, int> item in _inven)
        {
            await CreateAndSetupSlotAsync(item.Key, item.Value);
        }
    }

    
    private async UniTask CreateAndSetupSlotAsync(string itemId, int count)
    {
        if (_itemSlotPrefab == null) return;

        GameObject newSlot = Instantiate(_itemSlotPrefab, _layoutGroupParent);
        newSlot.SetActive(true);
       
        UIButtonBase targetSlot = newSlot.GetComponent<UIButtonBase>();
        if (targetSlot == null)
        {
            Debug.LogError($"{newSlot.name} 프리팹에 UIButtonBase 스크립트가 누락되었습니다!");
            return;
        }

        ItemData itemData = DataManager.Inst.GetItemData(itemId);
        if (itemData == null) return;

        Sprite slotImage = await ResourceManager.Inst.LoadSprite(itemData.IconPath);
        if (slotImage != null && targetSlot.Image_Base != null)
        {
            targetSlot.Image_Base.sprite = slotImage;
        }

        targetSlot.ChangeButtonText(count.ToString());
    }
    
    void OnClick_ExitEarnItemPopUp() {
        UIManager.Inst.ExitEarnItemPopUp();
    
    }
}
