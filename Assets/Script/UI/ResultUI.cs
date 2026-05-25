using UnityEngine;

public class ResultUI : UIInventoryBase
{
    [SerializeField] UIButtonBase Button_Exit;
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
        this.gameObject.SetActive(true);

        if (Text_Coin != null) Text_Coin.text = StageManager.Inst.StageGold.ToString("N0");

        DrawInventoryAsync(StageManager.Inst._stageInventoryDic).Forget();
    }
    void OnClick_ExitResultUI() {
        GameManager.Inst.GoToTown();
    }
}
