using UnityEngine;

public class EarnItemPopUp : UIInventoryBase
{
    [SerializeField] UIButtonBase Button_Exit;

    private void OnEnable()
    {
        Button_Exit.BindOnClickButtonEvent(OnClick_ExitButton);
        OpenEarnItemPopUp();
    }
    public void OpenEarnItemPopUp()
    {
        //this.gameObject.SetActive(true);
       
        if (Text_Coin != null) Text_Coin.text = StageManager.Inst.StageGold.ToString("N0");

        DrawInventory(StageManager.Inst._stageInventoryDic);
    }
    public void OnClick_ExitButton() 
    {
        UIManager.Inst.CloseEarnItemPopUp();
    }
}
