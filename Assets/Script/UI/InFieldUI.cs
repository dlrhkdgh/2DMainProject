using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InFieldUI : UIBase
{
    [Header("자식 UI 참조")]
    [SerializeField] private UIUseableInventory _uiInventory;
    [SerializeField] GameObject _skillSlotPrefab;
    [SerializeField] private Transform _skillSlotLayoutGroupParent;

    [SerializeField] UIButtonBase Button_EarnItemPopUp;
    [SerializeField] UIButtonBase Button_ExitStage;
    [Header("경험치 UI 요소")]
    [SerializeField] private Slider _expSlider;
    [SerializeField] private Text Text_Level;
    [SerializeField] private Text _timerText;
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
            UpdateExpBar(StageManager.Inst.PlayerExp,StageManager.Inst._maxExp,StageManager.Inst.PlayerLevel);
            _myInventoryDic = GameManager.Inst._inventoryDic;
        }
        OpenAndRefreshInventory();
        InitSkillSlot(GameManager.Inst._playerSkillId);
        Button_EarnItemPopUp.BindOnClickButtonEvent(OnClick_EarnItemPopUp);
        Button_ExitStage.BindOnClickButtonEvent(OnClick_ExitSatge);
    }
    private void Update()
    {
        if(StageManager.Inst == null) return;

        float elapsedSeconds = StageManager.Inst.StageTimer;
        float maxSeconds = StageManager.Inst.MaxStageTime; 

        float remainingSeconds = maxSeconds - elapsedSeconds;

        if (remainingSeconds < 0f) 
        { 
            remainingSeconds = 0f; 
        }

        int minutes = (int)(remainingSeconds / 60f);
        int seconds = (int)(remainingSeconds % 60f);

        _timerText.text = $"{minutes:D2}:{seconds:D2}";
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
    }
    void OnClick_EarnItemPopUp()
    {
        UIManager.Inst.OpenEarnItemPopUp();
    }
    void OnClick_ExitSatge() {
        GameManager.Inst.FinishStage(true);
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
    public void InitSkillSlot(string skillId) 
    {
        DrawSkillSlot(skillId);
    }
    public void DrawSkillSlot(string skillId)
    {
       
        foreach (Transform child in _skillSlotLayoutGroupParent)
        {
            Destroy(child.gameObject);
        }
        BombData data = DataManager.Inst.GetBombData(skillId);
        CreateAndSetupSkillSlot(data);
        
    }
    public void CreateAndSetupSkillSlot(BombData data) 
    {
        if (_skillSlotPrefab == null) return;

        if (data == null) return;

        GameObject newSlot = Instantiate(_skillSlotPrefab, _skillSlotLayoutGroupParent);
        newSlot.SetActive(true);

        UISkillSlot targetSlot = newSlot.GetComponent<UISkillSlot>();

        targetSlot.CoolTime = data.CoolTime;
        
        ResourceManager.Inst.LoadSprite(data.IconPath, (loadedSprite) =>
        {
            if (targetSlot == null || targetSlot.gameObject == null)
            {
                return;
            }
            if (targetSlot.Image_Skill == null) return;
            if (loadedSprite != null)
            {
                targetSlot.Image_Skill.sprite = loadedSprite;
            }
        });
    }


}

