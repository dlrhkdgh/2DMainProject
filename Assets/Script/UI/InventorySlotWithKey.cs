using UnityEngine;
using UnityEngine.UI;

public class InventorySlotWithKey : UIButtonBase
{
    [SerializeField] Text Text_KeyNumber;
    public int KeyNumber { get; set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeKeyNumberText(int keyNum)
    {
        if (Text_KeyNumber == null) return;

        Text_KeyNumber.text = Text_KeyNumber.ToString();
    }
}
