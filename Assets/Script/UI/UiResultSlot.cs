using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIResultSlot : UIBase
{
    [SerializeField] GameObject _itemSlotPrefab;
    [SerializeField] Transform _layoutGroupParent;

    private bool _isClear;
    private void OnEnable()
    {
        _isClear = StageManager.Inst._isClear;
    }
    public void DrawInventory(Dictionary<string, int> targetInven)
    {
        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }

        if (targetInven != null)
        {
            foreach (KeyValuePair<string, int> item in targetInven)
            {
                CreateAndSetupSlot(item.Key, item.Value);
            }
        }
    }
    private void CreateAndSetupSlot(string itemId, int count)
    {
        if (_itemSlotPrefab == null) return;

        ItemData itemData = DataManager.Inst.GetItemData(itemId);
        if (itemData == null) return;

        GameObject newSlot = Instantiate(_itemSlotPrefab, _layoutGroupParent);
        newSlot.SetActive(true);

        UIButtonBase targetSlot = newSlot.GetComponent<UIButtonBase>();
        if (targetSlot != null)
        {
            if (_isClear == false)
            {
                int failCount = (int)(count * 0.5f);
                targetSlot.ChangeButtonText(failCount.ToString());
                targetSlot.Text_Base.color = Color.red;
            }
            else
            {
                targetSlot.ChangeButtonText(count.ToString());
            }
        }
        ResourceManager.Inst.LoadSprite(itemData.IconPath, (loadedSprite) =>
        {
            if (targetSlot == null || targetSlot.gameObject == null)
            {
                return;
            }

            if (targetSlot.Image_Base == null) return;
            if (loadedSprite != null)
            {
                targetSlot.Image_Base.sprite = loadedSprite;
            }
        });
    }
}
