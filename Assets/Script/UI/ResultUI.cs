using UnityEngine;
using UnityEngine.UI;

public class ResultUI : UIBase
{
    [SerializeField] UIButtonBase Button_Exit;
    [SerializeField] Text Text_Result;
    [SerializeField] Text Text_Coin;
    [SerializeField] UIResultSlot _slotPrefab;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        if (Button_Exit != null)
        {
            Button_Exit.BindOnClickButtonEvent(OnClick_ExitResultUI);
        }
        OpenResultUI();
    }
    
    public void OpenResultUI()
    {
        int resultGold = StageManager.Inst.StageGold;
        if (StageManager.Inst._isClear)
        {
            Text_Result.text = "Clear!";
            if (Text_Coin != null) Text_Coin.text = resultGold.ToString("N0");
        }
        else 
        {
            resultGold = (int)(resultGold * 0.5f);
            Text_Result.text = "Fail!";
            if (Text_Coin != null) Text_Coin.text = resultGold.ToString("N0");
        }

        _slotPrefab.DrawInventory(StageManager.Inst._stageInventoryDic);
    }
    void OnClick_ExitResultUI() {
        GameManager.Inst.ExitStage();
    }
}
