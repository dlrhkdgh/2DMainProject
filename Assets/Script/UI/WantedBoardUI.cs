using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WantedBoardUI : UIBase
{
    [SerializeField] GameObject _wantedSlotPrefab;
    [SerializeField] private Transform _layoutGroupParent;
    [SerializeField] UIButtonBase Button_Exit;
    //[SerializeField] private Text Text_Name;
    Dictionary<string, MonsterData> _monsterList;
    private void OnEnable()
    {
        if (DataManager.Inst != null)
        {
            _monsterList = DataManager.Inst.MonsterDataList;
            DrawInventory(_monsterList);
        }
        Button_Exit.BindOnClickButtonEvent(OnClick_ExitButton);
    }
    public void DrawInventory(Dictionary<string, MonsterData> monsterList)
    {
        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }

        if (monsterList != null)
        {
            foreach (string monsterId in monsterList.Keys)
            {
                CreateAndSetupSlot(monsterId);
            }
        }
    }
    private void CreateAndSetupSlot(string monsterId)
    {
        if (_wantedSlotPrefab == null) return;

        MonsterData monsterData = DataManager.Inst.GetMonsterData(monsterId);
        if (monsterData == null) return;

        GameObject newSlot = Instantiate(_wantedSlotPrefab, _layoutGroupParent);
        newSlot.SetActive(true);

        UIWantedSlot targetSlot = newSlot.GetComponent<UIWantedSlot>();
        
        string iconPath;
        if (GameManager.Inst._monsterDic.Contains(monsterId))
        {
            iconPath = monsterData.IconPath;
            targetSlot.Text_Name.text = monsterData.Name;
        }
        else 
        {
            iconPath = monsterData.SilhouetteIconPath;
            targetSlot.Text_Name.text = "???";
        }
        ResourceManager.Inst.LoadSprite(iconPath, (loadedSprite) =>
        {
            if (targetSlot == null || targetSlot.gameObject == null)
            {
                // Debug.Log("이미지를 불러왔으나 슬롯이 이미 파괴되어 연산을 취소합니다.");
                return;
            }

            // 이미지 컴포넌트 자체도 한 번 더 체크
            if (targetSlot.Image_Icon == null) return;
            if (loadedSprite != null)
            {
                targetSlot.Image_Icon.sprite = loadedSprite;
            }
        });
    }
    public void OnClick_ExitButton() 
    {
        UIManager.Inst.CloseWantedBoardUI();
    }
}
