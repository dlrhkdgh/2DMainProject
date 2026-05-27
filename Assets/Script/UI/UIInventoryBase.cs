using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryBase : UIBase
{
    [SerializeField] GameObject _itemSlotPrefab;
    [SerializeField] private Transform _layoutGroupParent;
    [SerializeField] protected Text Text_Coin;
    private CancellationTokenSource _cts;
    private void OnEnable()
    {
      
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
    }

}
