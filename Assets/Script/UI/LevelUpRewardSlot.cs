using UnityEngine;
using UnityEngine.UI;

public class LevelUpRewardSlot : UISelectedSolt
{
    [SerializeField] private Text Text_RewardName;
    [SerializeField] private Text Text_RewardDescription;
    [SerializeField] private Text Text_RewardLevel;
    [SerializeField] public Image Image_Base;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeNameText(string buttonStr)
    {
        // 혹시 이버튼을 동적으로, 코드에서 텍스트를 수정해야할 때 사용
        if (Text_RewardName == null) return;

        Text_RewardName.text = buttonStr;
    }
    public void ChangeDescriptionText(string buttonStr)
    {
        // 혹시 이버튼을 동적으로, 코드에서 텍스트를 수정해야할 때 사용
        if (Text_RewardDescription == null) return;

        Text_RewardDescription.text = buttonStr;
    }
    public void ChangeLevelText(string buttonStr)
    {
        // 혹시 이버튼을 동적으로, 코드에서 텍스트를 수정해야할 때 사용
        if (Text_RewardLevel == null) return;
        Text_RewardLevel.gameObject.SetActive(true);
        Text_RewardLevel.text = buttonStr;
    }
}
