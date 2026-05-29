using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUseableInventory : UIBase
{
    [SerializeField] GameObject _itemSlotPrefab;
    [SerializeField] private Transform _layoutGroupParent;
    [SerializeField] protected Text Text_Coin;
    public int _slotNum = 0;
    private void OnEnable()
    {
        _slotNum = 0;
       
    }
    public void DrawInventory(Dictionary<string, int> targetInven)
    {
        _slotNum = 0;
        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }

        if (targetInven != null)
        {
            foreach (KeyValuePair<string, int> item in targetInven)
            {
                ItemData data =DataManager.Inst.GetItemData(item.Key);
                if (data.IsUseable)
                {
                    _slotNum++;
                    CreateAndSetupSlot(item.Key, item.Value, _slotNum);
                }
            }
        }
    }
    private void CreateAndSetupSlot(string itemId, int count, int slotNum)
    {
        if (_itemSlotPrefab == null) return;

        ItemData itemData = DataManager.Inst.GetItemData(itemId);
        if (itemData == null) return;

        GameObject newSlot = Instantiate(_itemSlotPrefab, _layoutGroupParent);
        newSlot.SetActive(true);

        InventorySlotWithKey targetSlot = newSlot.GetComponent<InventorySlotWithKey>();
        if (targetSlot != null)
        {
            targetSlot.ChangeButtonText(count.ToString());
        }
        ResourceManager.Inst.LoadSprite(itemData.IconPath, (loadedSprite) =>
        {
            if (targetSlot == null || targetSlot.gameObject == null)
            {
                // Debug.Log("이미지를 불러왔으나 슬롯이 이미 파괴되어 연산을 취소합니다.");
                return;
            }

            // 이미지 컴포넌트 자체도 한 번 더 체크
            if (targetSlot.Image_Base == null) return;
            if (loadedSprite != null)
            {
                targetSlot.Image_Base.sprite = loadedSprite;
            }
        });
        targetSlot.ChangeKeyNumberText(slotNum);
    }

}
