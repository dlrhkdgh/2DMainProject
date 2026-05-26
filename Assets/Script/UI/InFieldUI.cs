using UnityEngine;
using UnityEngine.UI;

public class InFieldUI : UIBase
{
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

    void OnEnable()
    {
        if (StageManager.Inst != null) 
        {
            StageManager.Inst.OnExpChanged += UpdateExpBar;
            UpdateExpBar(StageManager.Inst.PlayerExp,StageManager.Inst.maxExp,StageManager.Inst.PlayerLevel);
        }
        Button_EarnItemPopUp.BindOnClickButtonEvent(OnClick_EarnItemPopUp);
        Button_ExitStage.BindOnClickButtonEvent(OnClick_ExitSatge);
    }
    private void OnDisable()
    {
        if (StageManager.Inst != null) 
        {
            StageManager.Inst.OnExpChanged -= UpdateExpBar;
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
        UIManager.Inst.CloseInFeildUI();
    }
}
