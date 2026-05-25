using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryBase : UIBase
{
    [SerializeField] GameObject _itemSlotPrefab;
    [SerializeField] private Transform _layoutGroupParent;
    [SerializeField] protected Text Text_Coin;

    private void OnEnable()
    {
       
    }
    public async UniTaskVoid DrawInventoryAsync(Dictionary<string, int> targetInven)
    {
        
        this.gameObject.SetActive(true);

        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }


        if (targetInven != null)
        {
            foreach (KeyValuePair<string, int> item in targetInven)
            {
                await CreateAndSetupSlotAsync(item.Key, item.Value);
            }
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
}
