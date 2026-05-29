using System.Collections.Generic;
using UnityEngine;

public class ShopItemDescriptionPopUp : UIBase
{
    [SerializeField] UIButtonBase Button_Exit;
    [SerializeField] GameObject _itemSlotPrefab;
    [SerializeField] private Transform _layoutGroupParent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnEnable()
    {
        var itmeList = new List<string>( DataManager.Inst.ShopItemDataList.Keys);
        Button_Exit.BindOnClickButtonEvent(OnClick_ExitButtpn);
        DrawInventory(itmeList);
    }
    public void DrawInventory(List<string> itmeList)
    {
        if (itmeList == null || itmeList.Count == 0) return;

        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }

        if (itmeList != null)
        {

            for (int i = 0; i < itmeList.Count; i++)
            {
                CreateAndSetupSlot(itmeList[i]);
            }
        }
    }
    private void CreateAndSetupSlot(string id)
    {
        if (_itemSlotPrefab == null) return;
        GameObject newSlot = Instantiate(_itemSlotPrefab, _layoutGroupParent);
        newSlot.SetActive(true);
        LevelUpRewardSlot targetSlot = newSlot.GetComponent<LevelUpRewardSlot>();

        if (targetSlot == null)
        {
            Debug.LogError($"{newSlot.name} 프리팹에 UIButtonBase 스크립트가 누락되었습니다!");
            return;
        }

        ShopItemData itemData = DataManager.Inst.GetShopItemData(id);
        if (itemData == null) return;

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

        targetSlot.ChangeNameText(itemData.Name);
        targetSlot.ChangeDescriptionText(itemData.Description);
    }
        
    void OnClick_ExitButtpn() {
        UIManager.Inst.CloseShopItemDescriptionPopUp();
    }
}
