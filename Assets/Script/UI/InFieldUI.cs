using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InFieldUI : UIBase
{
    [Header("자식 UI 참조")]
    [SerializeField] private UIUseableInventory _uiInventory;

    [SerializeField] UIButtonBase Button_EarnItemPopUp;
    [SerializeField] UIButtonBase Button_ExitStage;
    [Header("경험치 UI 요소")]
    [SerializeField] private Slider _expSlider;
    [SerializeField] private Text Text_Level;
    //[SerializeField] private Text _textExpPercent;
    [Header("플레이어 현재 스텟")]
    [SerializeField] private Text Text_MaxHp;
    [SerializeField] private Text Text_MoveSpeed;
    [SerializeField] private Text Text_Attack;
    [SerializeField] private Text Text_MagnetRange;
    [SerializeField] private Text Text_Armor;
    [SerializeField] private Text Text_CriticalPercent;
    private Dictionary<string, int> _myInventoryDic;
    void OnEnable()
    {

        if (StageManager.Inst != null) 
        {
            StageManager.Inst.OnExpChanged += UpdateExpBar;
            StageManager.Inst.OnPlayerStatChanged += UpdatePlayerStat;
            StageManager.Inst.OnUseableItemChanged += OpenAndRefreshInventory;
            UpdatePlayerStat();
            UpdateExpBar(StageManager.Inst.PlayerExp,StageManager.Inst.maxExp,StageManager.Inst.PlayerLevel);
            _myInventoryDic = StageManager.Inst._useableItemInventoryDic;
        }
        OpenAndRefreshInventory();
        Button_EarnItemPopUp.BindOnClickButtonEvent(OnClick_EarnItemPopUp);
        Button_ExitStage.BindOnClickButtonEvent(OnClick_ExitSatge);
    }
    private void OnDisable()
    {
        if (StageManager.Inst != null) 
        {
            StageManager.Inst.OnExpChanged -= UpdateExpBar;
            StageManager.Inst.OnPlayerStatChanged -= UpdatePlayerStat;
            StageManager.Inst.OnUseableItemChanged -= OpenAndRefreshInventory;

        }
    }
    private void UpdateExpBar(int currentExp, int maxExp, int currentLevel)
    {
        if (_expSlider == null) return;

        
        float expRatio = (float)currentExp / maxExp;

        _expSlider.value = expRatio;

       
        if (Text_Level != null)
        {
            Text_Level.text = $"Lv.{currentLevel}";
        }


        //if (_textExpPercent != null)
        //{
        //    _textExpPercent.text = $"{expRatio * 100f:F1}%";
        //}
    }
    void OnClick_EarnItemPopUp()
    {

        UIManager.Inst.OpenEarnItemPopUp();
    }
    void OnClick_ExitSatge() {
        GameManager.Inst.FinishStage();
        //UIManager.Inst.CloseInFeildUI();
    }
    void UpdatePlayerStat() {
        Debug.Log("플레이어스텟 업데이트");
        Text_MaxHp.text=Player.Inst.FinalMaxHp.ToString();
        Text_Attack.text=Player.Inst.FinalAttack.ToString();
        Text_MoveSpeed.text = Player.Inst.FinalMoveSpeed.ToString();
        Text_MagnetRange.text = Player.Inst.FinalMagnetRange.ToString();
        Text_Armor.text = Player.Inst.FinalArmor.ToString();
        Text_CriticalPercent.text = Player.Inst.FinalCriticalPercent.ToString();
    }
    public void OpenAndRefreshInventory()
    {
        if (_uiInventory == null) return;
        _uiInventory.gameObject.SetActive(true);
        _uiInventory.DrawInventory(_myInventoryDic);
    }
}
