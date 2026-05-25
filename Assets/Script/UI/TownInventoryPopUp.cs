using UnityEngine;

public class TownInventoryPopUp : UIInventoryBase
{
    [SerializeField] UIButtonBase Button_Exit;

    private void OnEnable()
    {
        Button_Exit.BindOnClickButtonEvent(OnClick_ExitButton);
        OpenTownInventoryPopUp();
    }
    public void OpenTownInventoryPopUp()
    {
       // this.gameObject.SetActive(true);

        if (Text_Coin != null) Text_Coin.text = GameManager.Inst.Gold.ToString("N0");

        DrawInventoryAsync(GameManager.Inst._inventoryDic).Forget();
    }
    public void OnClick_ExitButton()
    {
        UIManager.Inst.CloseTownInventoryPopUp();   
    }
}
