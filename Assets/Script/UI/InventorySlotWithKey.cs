using UnityEngine;
using UnityEngine.UI;

public class InventorySlotWithKey : UIButtonBase
{
    [SerializeField] Text Text_KeyNumber;
    public int KeyNumber { get; set; }
  
    public void ChangeKeyNumberText(int keyNum)
    {
        if (Text_KeyNumber == null) return;

        Text_KeyNumber.text = keyNum.ToString();
    }
   
}
