using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarnItemPopUp : UIBase
{
    [SerializeField] UIButtonBase _itemSlot;
    [SerializeField] UIButtonBase Button_Exit;
    [SerializeField] private Transform _layoutGroupParent;
    //private List<UIButtonBase> _createdSlotList = new List<UIButtonBase>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       //Button_Exit.BindOnClickButtonEvent(OnClick_ExitEarnItemPopUp);
       // SetItemSlot();
    }
    private void OnEnable()
    {
        Button_Exit.BindOnClickButtonEvent(OnClick_ExitEarnItemPopUp);
        CreatEarnItemSlot();
        SetItemSlot();
    }
    private void OnDisable()
    {
        //Button_Exit.onClick.RemoveAllListeners();
        Destroy(_itemSlot);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void SetItemSlot() {
        int gold = GameManager.Inst.Gold;

        string earnCoinStr = gold.ToString();
        _itemSlot.ChangeButtonText(earnCoinStr);
       // _itemSlot.SetActive(true);

    }
    void OnClick_ExitEarnItemPopUp() {
        UIManager.Inst.ExitEarnItemPopUp();
    
    }
    void CreatEarnItemSlot() {

        string path = "Prefabs/UI/EarnItemSlot1";
        GameObject loadedObj = (GameObject)Resources.Load(path);
        GameObject newUIObj;
        if (loadedObj == null)
        {
            Debug.LogError($"[UI 로드 실패] Resources/{path} 경로에 프리팹이 존재하지 않습니다.");
            return;
        }


        newUIObj = Instantiate(loadedObj, _layoutGroupParent);
        if (newUIObj != null)
        {
            var uiBaseBtn = newUIObj.GetComponent<UIButtonBase>();
            if (uiBaseBtn != null)
            {
                //newUIObj.SetActive(false);
                _itemSlot = uiBaseBtn;
               // _createdSlotList.Add(uiBaseBtn);
            }
            else
            {
                Debug.LogError($"{newUIObj.name} 프리팹에 UIBase 스크립트가 누락되었습니다!");
            }

        }
    }
}
