using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class LevelUpRewardUI : UIBase
{
    [SerializeField] UIButtonBase Button_Apply;
    [SerializeField] GameObject _itemSlotPrefab;
    [SerializeField] private Transform _layoutGroupParent;
    public List<LevelUpRewardSlot> _rewardSlotList = new List<LevelUpRewardSlot>();
    public List<string> _rewardList;
    public int SelectedSlotId { get; set; } = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        Button_Apply.BindOnClickButtonEvent(OnClick_ApplyButton);
        SelectedSlotId = -1;
        _rewardSlotList.Clear();
        OpenRewardPopUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OpenRewardPopUp() {
        DrawInventory(StageManager.Inst._finalRewardList);
    }
    void OnClick_ApplyButton() {
        if (SelectedSlotId < 0 || SelectedSlotId >= _rewardSlotList.Count)
        {
            UIManager.Inst.OpenCreatedUI(UITypeRoot.PopupUI, UIType.YesOrNoPopUp);
            var yesOrNoPopUpi = UIManager.Inst.GetUI<UIYesOrNoPopUp>(UIType.YesOrNoPopUp);
            yesOrNoPopUpi.OpenYesOrNoPopUp("정말로 업그레이드를 그만두시겠습니까?", (bool isYes) =>
            {
                if (isYes)
                {
                    StageManager.Inst.FinishLevelUpReward();
                }
            });
        }
        else {
            StageManager.Inst.PlayerGetLevelUpReward(_rewardList[SelectedSlotId]);
            StageManager.Inst.FinishLevelUpReward();
        }
    }
    //public async UniTaskVoid DrawInventoryAsync(List<string> rewardList)
    //{
    //    if (rewardList == null || rewardList.Count == 0) return;
    //    _rewardList = new List<string>(rewardList);
    //    //this.gameObject.SetActive(true);

    //    foreach (Transform child in _layoutGroupParent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    if (rewardList != null)
    //    {
    //        for (int i = 0; i < _rewardList.Count; i++) {

    //            await CreateAndSetupSlotAsync(_rewardList[i],i);
    //        }
            
    //    }
    //}
    public void DrawInventory(List<string> rewardList)
    {
        if (rewardList == null || rewardList.Count == 0) return;
        _rewardList = new List<string>(rewardList);

        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }

        if (rewardList != null)
        {

            for (int i = 0; i < _rewardList.Count; i++)
            {
                CreateAndSetupSlot(_rewardList[i], i);
            }
        }
    }
    private void CreateAndSetupSlot(string Id, int index)
    {
        if (_itemSlotPrefab == null) return;
 GameObject newSlot = Instantiate(_itemSlotPrefab, _layoutGroupParent);
        newSlot.SetActive(true);
        LevelUpRewardSlot targetSlot = newSlot.GetComponent<LevelUpRewardSlot>();

        if (targetSlot == null)
        {
            Debug.LogError($"{newSlot.name} 프리팹에 UIButtonBase 스크립트가 누락되었습니다!");
            return;
        }

        LevelUpRewardData rewardData = DataManager.Inst.GetLevelUpRewardData(Id);
        if (rewardData == null) return;
        
        ResourceManager.Inst.LoadSprite(rewardData.IconPath, (loadedSprite) =>
        {
            if (targetSlot == null || targetSlot.gameObject == null)
            {
                // Debug.Log("이미지를 불러왔으나 슬롯이 이미 파괴되어 연산을 취소합니다.");
                return;
            }

            // 이미지 컴포넌트 자체도 한 번 더 체크
            if (targetSlot.Image_Base == null) return;
            if (loadedSprite != null)
            {
                targetSlot.Image_Base.sprite = loadedSprite;
            }
        });
        _rewardSlotList.Add(targetSlot);
        targetSlot.InitSelectedSolt(index, OnSlotSelect);
        targetSlot.ChangeNameText(rewardData.Name);
        targetSlot.ChangeDescriptionText(rewardData.Description);
        targetSlot.ChangeLevelText($"Lv.{StageManager.Inst._currentLevelUpRewardDic[Id].ToString()}");
    }
    //private async UniTask CreateAndSetupSlotAsync(string Id,int index)
    //{

    //    if (_itemSlotPrefab == null) return;

    //    GameObject newSlot = Instantiate(_itemSlotPrefab, _layoutGroupParent);
    //    newSlot.SetActive(true);

    //    LevelUpRewardSlot targetSlot = newSlot.GetComponent<LevelUpRewardSlot>();
    //    if (targetSlot == null)
    //    {
    //        Debug.LogError($"{newSlot.name} 프리팹에 UIButtonBase 스크립트가 누락되었습니다!");
    //        return;
    //    }

    //    LevelUpRewardData rewardData = DataManager.Inst.GetLevelUpRewardData(Id);
    //    if (rewardData == null) return;

    //    Sprite slotImage = await ResourceManager.Inst.LoadSprite(rewardData.IconPath);
    //    if (slotImage != null && targetSlot.Image_Base != null)
    //    {
    //        targetSlot.Image_Base.sprite = slotImage;
    //    }

    //    _rewardSlotList.Add(targetSlot);
    //    targetSlot.InitSelectedSolt(index, OnSlotSelect);
    //    targetSlot.ChangeNameText(rewardData.Name);
    //    targetSlot.ChangeDescriptionText(rewardData.Description);
    //    targetSlot.ChangeLevelText($"Lv.{StageManager.Inst._currentLevelUpRewardDic[Id].ToString()}");
    //}
    private void OnSlotSelect(int id)
    {

        if (id < 0 || id >= _rewardSlotList.Count) return;
        if (SelectedSlotId == id)
        {
            _rewardSlotList[SelectedSlotId].SetSelectedState(false);
            SelectedSlotId = -1;
            return;
        }
        if (SelectedSlotId != -1)
        {
            _rewardSlotList[SelectedSlotId].SetSelectedState(false);
        }

        SelectedSlotId = id;
        _rewardSlotList[SelectedSlotId].SetSelectedState(true);
    }
}
