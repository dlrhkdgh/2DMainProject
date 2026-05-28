using UnityEngine;
using UnityEngine.UI;

public class UIShopSlot : UIButtonBase
{
    [SerializeField] Text Text_Price;
    [SerializeField] GameObject Img_Count;
    public string itemId;
    
   public int Price {  get; set; }
    
    public void ChangePriceButtonText() {

        Text_Price.text=Price.ToString();
    }
    public void SetFalseImage() {
        if (Img_Count == null) return;
        Img_Count.gameObject.SetActive(false);
    }
}
