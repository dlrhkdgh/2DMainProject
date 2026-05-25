using UnityEngine;

public class LevelUpRewardUI : UIInventoryBase
{
    [SerializeField] UIButtonBase Button_Apply;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        Button_Apply.BindOnClickButtonEvent(OnClick_ApplyButton);
        OpenRewardPopUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OpenRewardPopUp() {
        DrawInventoryAsync(StageManager.Inst._stageInventoryDic).Forget();
    }
    void OnClick_ApplyButton() {
        StageManager.Inst.FinishLevelUpReward();
    
    }
}
