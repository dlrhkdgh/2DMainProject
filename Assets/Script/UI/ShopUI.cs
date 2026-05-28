using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : UIBase
{
    [SerializeField] private UIButtonBase Button_Exit;
    [SerializeField] private Transform _shopLayoutGroupParent;
    [SerializeField] private Transform _playerLayoutGroupParent;
    [SerializeField] GameObject _shopItemSlotPrefab;
    [SerializeField] private Text Text_Gold;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        
        Button_Exit.BindOnClickButtonEvent(OnClick_ExitButton);
        DrawInventory(GameManager.Inst._inventoryDic, _playerLayoutGroupParent);
        DrawInventory(DataManager.Inst.ShopItemDataList, _shopLayoutGroupParent);
        ChangeGoldText();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnClick_ExitButton()
    {

        UIManager.Inst.CloseShopUI();
    }
    public void DrawInventory(Dictionary<string, int> targetInven, Transform layoutParent )
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
    }
    
    private void CreateAndSetupSlot(string itemId, int count, Transform layoutParent)
    {
        if (_shopItemSlotPrefab == null) return;

        ItemData itemData = DataManager.Inst.GetItemData(itemId);
        if (itemData == null) return;

        GameObject newSlot = Instantiate(_shopItemSlotPrefab, layoutParent);
        newSlot.SetActive(true);

        UIShopSlot targetSlot = newSlot.GetComponent<UIShopSlot>();
        if (targetSlot != null)
        {
            targetSlot.itemId = itemId;
            targetSlot.Price = itemData.SellingPrice;
            targetSlot.ChangePriceButtonText();
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
    public void DrawInventory(Dictionary<string, ShopItemData> targetInven, Transform layoutParent)
    {

        foreach (Transform child in layoutParent)
        {
            Destroy(child.gameObject);
        }

        if (targetInven != null)
        {
            foreach (KeyValuePair<string, ShopItemData> item in targetInven)
            {
                CreateAndSetupSlot(item.Key,item.Value, layoutParent);
            }
        }
    }
    private void CreateAndSetupSlot(string itemId, ShopItemData itemData, Transform layoutParent)
    {
        if (_shopItemSlotPrefab == null) return;
        
        if (itemData == null) return;

        GameObject newSlot = Instantiate(_shopItemSlotPrefab, layoutParent);
        newSlot.SetActive(true);

        UIShopSlot targetSlot = newSlot.GetComponent<UIShopSlot>();
        if (targetSlot != null)
        {
            targetSlot.itemId = itemId;
            targetSlot.Price = itemData.SellingPrice;
            targetSlot.ChangePriceButtonText();
            targetSlot.SetFalseImage();
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
    void ChangeGoldText() 
    {
        Text_Gold.text = GameManager.Inst.Gold.ToString();
    }
}
