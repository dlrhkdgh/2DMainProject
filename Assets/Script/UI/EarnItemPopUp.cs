using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EarnItemPopUp : UIBase
{
    private UIButtonBase _itemSlot;
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
    {// 1. 혹시 인스펙터 디자인 창에서 미리 만들어둔 자식 슬롯 찌꺼기가 있다면 싹 밀고 시작합니다 (슬롯 2개 방지)
        foreach (Transform child in _layoutGroupParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 버튼 이벤트를 안전하게 연결합니다 (아까 배운 if문 방어 코드 적용!)
        if (Button_Exit != null)
        {
            Button_Exit.BindOnClickButtonEvent(OnClick_ExitEarnItemPopUp);
        }

        // 3. 순서 중요: 슬롯을 먼저 '생성'하고, 데이터가 다 들어간 '후'에 텍스트를 세팅합니다.
        CreatEarnItemSlot();
        SetItemSlot();
    }
    private void OnDisable()
    {
        // 4. 리스너를 지울 때도 안전하게 if문으로 체크
        if (Button_Exit != null)
        {
            // 만약 UI 시스템에 RemoveAllListeners가 없다면 주석 해제하여 사용하세요.
            // Button_Exit.onClick.RemoveAllListeners(); 
        }

        // 5. [핵심 해결] 스크립트가 아니라 슬롯의 'gameObject' 자체를 파괴해야 화면에서 사라집니다!
        if (_itemSlot != null)
        {
            Destroy(_itemSlot.gameObject);
            _itemSlot = null; // 방 비워주기
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void SetItemSlot() {
        if (_itemSlot == null) return;

        // 게임매니저에서 골드 데이터를 실시간으로 가져옵니다.
        int gold = GameManager.Inst.Gold;
        string earnCoinStr = gold.ToString();

        // 텍스트를 바꾼 뒤, 오브젝트가 확실히 켜져 있도록 강제 확인해 줍니다.
        _itemSlot.ChangeButtonText(earnCoinStr);
        _itemSlot.gameObject.SetActive(true);
    }
    void OnClick_ExitEarnItemPopUp() {
        UIManager.Inst.ExitEarnItemPopUp();
    
    }
    void CreatEarnItemSlot() {

        string path = "Prefabs/UI/EarnItemSlot1";
        GameObject loadedObj = (GameObject)Resources.Load(path);

        if (loadedObj == null)
        {
            Debug.LogError($"[UI 로드 실패] Resources/{path} 경로에 프리팹이 존재하지 않습니다.");
            return;
        }

        // 프리팹을 레이아웃 그룹의 자식으로 소환!
        GameObject newUIObj = Instantiate(loadedObj, _layoutGroupParent);

        if (newUIObj != null)
        {
            // 프리팹 자체가 꺼져있을 수 있으므로 소환하자마자 먼저 켜줍니다 (텍스트 변경 버그 해결)
            newUIObj.SetActive(true);

            var uiBaseBtn = newUIObj.GetComponent<UIButtonBase>();
            if (uiBaseBtn != null)
            {
                _itemSlot = uiBaseBtn;
            }
            else
            {
                Debug.LogError($"{newUIObj.name} 프리팹에 UIButtonBase 스크립트가 누락되었습니다!");
            }
        }
    }
}
