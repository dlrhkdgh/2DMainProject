using System.Collections.Generic;
using UnityEngine;
public enum UITypeRoot
{
    None = 0,
    BackgroundUI,
    MainUI,
    ContentUI,
    PopupUI,
    VeryFrontUI
}
public enum UIType
{
    None = 0,
    LobbyUI,
    InFieldUI,
    MainUI,
    GameoverUI,
    EnterStageUI,
    ResultUI,
    LevelUpRewardUI,
    ShopUI,
    EarnItemPopUp,
    TownInventoryPopUp,
    ExitGamePopUp,
    YesOrNoPopUp,
}
public class UIManager : MonoBehaviour
{
    [SerializeField] private Transform uiCanvasTransform;
    [SerializeField] private Transform popUpUiCanvasTransform;
    private Dictionary<UIType, UIBase> _createdUIDic = new Dictionary<UIType, UIBase>();
    public static UIManager Inst { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Inst = this;
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OpenCreatedUI(UIType uiType)
    {

        if (_createdUIDic.ContainsKey(uiType) == false)
            CreatUI(uiType);

        if (_createdUIDic.ContainsKey(uiType))
        {
            UIBase targetUI = _createdUIDic[uiType];
            if (targetUI.gameObject.activeSelf)
            {
                Debug.LogWarning($"{uiType} UI는 이미 화면에 열려 있습니다.");
                return;
            }
            targetUI.OpenUI();
        }
        else
        {
            Debug.LogError($"{uiType}에 해당하는 UI가 딕셔너리에 없습니다!");
        }

    }
    public void OpenCreatedUI(UITypeRoot uiTypRoot, UIType uiType)
    {

        if (_createdUIDic.ContainsKey(uiType) == false)
            CreatUI(uiTypRoot, uiType);

        if (_createdUIDic.ContainsKey(uiType))
        {
            UIBase targetUI = _createdUIDic[uiType];
            if (targetUI.gameObject.activeSelf)
            {
                Debug.LogWarning($"{uiType} UI는 이미 화면에 열려 있습니다.");
                return;
            }
            targetUI.OpenUI();
        }
        else
        {
            Debug.LogError($"{uiType}에 해당하는 UI가 딕셔너리에 없습니다!");
        }

    }
    public void CloseCreatedUI(UIType uiType)
    {

        if (_createdUIDic.ContainsKey(uiType))
        {
            UIBase targetUI = _createdUIDic[uiType];
            if (targetUI.gameObject.activeSelf == false)
            {
                Debug.LogWarning($"{uiType} UI는 이미 닫혀 있는 상태입니다.");
                return;
            }
            targetUI.CloseUI();
        }
        else
        {
            Debug.LogError($"{uiType}에 해당하는 UI가 딕셔너리에 없습니다!");
        }

    }
    public void CreatUI(UIType uiType)
    {

        if (_createdUIDic.ContainsKey(uiType) == false)
        {
            string path = GetUIPath(uiType);
            GameObject loadedObj = (GameObject)Resources.Load(path);
            GameObject newUIObj;
            if (loadedObj == null)
            {
                Debug.LogError($"[UI 로드 실패] Resources/{path} 경로에 프리팹이 존재하지 않습니다.");
                return;
            }

            newUIObj = Instantiate(loadedObj, uiCanvasTransform);
            if (newUIObj != null)
            {
                var uiBase = newUIObj.GetComponent<UIBase>();
                if (uiBase != null)
                {
                    newUIObj.SetActive(false);
                    _createdUIDic.Add(uiType, uiBase);
                }
                else
                {
                    Debug.LogError($"{newUIObj.name} 프리팹에 UIBase 스크립트가 누락되었습니다!");
                }

            }
        }

    }
    public void CreatUI(UITypeRoot uiTypRoot, UIType uiType)
    {

        if (_createdUIDic.ContainsKey(uiType) == false)
        {
            string path = GetUIPath(uiType);
            GameObject loadedObj = (GameObject)Resources.Load(path);
            GameObject newUIObj;
            if (loadedObj == null)
            {
                Debug.LogError($"[UI 로드 실패] Resources/{path} 경로에 프리팹이 존재하지 않습니다.");
                return;
            }


            newUIObj = Instantiate(loadedObj, popUpUiCanvasTransform);
            if (newUIObj != null)
            {
                var uiBase = newUIObj.GetComponent<UIBase>();
                if (uiBase != null)
                {
                    newUIObj.SetActive(false);
                    _createdUIDic.Add(uiType, uiBase);
                }
                else
                {
                    Debug.LogError($"{newUIObj.name} 프리팹에 UIBase 스크립트가 누락되었습니다!");
                }

            }
        }

    }
    public T GetUI<T>(UIType uiType) where T : UIBase
    {
        if (_createdUIDic.TryGetValue(uiType, out UIBase uiBase))
        {
            return uiBase as T;
        }
        return null;
    }
  
    public static string GetUIPath(UIType uiType)
    {
        string path = string.Empty;
        path = $"Prefabs/UI/{uiType}";
        return path;
    }
    public void OpenLobbyUI() {
        OpenCreatedUI(UIType.LobbyUI);
    }
    public void CloseLobbyUI() {
        CloseCreatedUI(UIType.LobbyUI);
    }
    public void OpenEarnItemPopUp() {
        OpenCreatedUI(UITypeRoot.PopupUI, UIType.EarnItemPopUp);
    }
    public void CloseEarnItemPopUp() {
        CloseCreatedUI(UIType.EarnItemPopUp);
    }
    public void OpenTownInventoryPopUp(){

        OpenCreatedUI(UITypeRoot.PopupUI, UIType.TownInventoryPopUp);
    }
    public void CloseTownInventoryPopUp() {

        CloseCreatedUI(UIType.TownInventoryPopUp);
    }
    public void OpenEnterStageUI() {
        OpenCreatedUI(UIType.EnterStageUI);
    }
    public void CloseEnterStageUI()
    {
        CloseCreatedUI(UIType.EnterStageUI);
    }
     public void OpenInFeildUI() {
        OpenCreatedUI(UIType.InFieldUI);
    }
    public void CloseInFeildUI() {
        CloseCreatedUI(UIType.InFieldUI);
    }
    public void OpenMainUI() {
        OpenCreatedUI(UIType.MainUI);
    }
    public void CloseMainUi() {
        CloseCreatedUI(UIType.MainUI);
    }
    public void OpenResultUI() {
        OpenCreatedUI(UIType.ResultUI);
    }
    public void CloseResultUI() {
        CloseCreatedUI(UIType.ResultUI);
    }
    public void OpenLevelUpRewardUI()
    {
        OpenCreatedUI(UITypeRoot.PopupUI, UIType.LevelUpRewardUI);
    }
    public void CloseLevelUpRewardUI()
    {
        CloseCreatedUI(UIType.LevelUpRewardUI);
    }
    public void OpenYesOrNoPopUp()
    {
        OpenCreatedUI(UITypeRoot.PopupUI, UIType.YesOrNoPopUp);
    }
    public void CloseYesOrNoPopUp()
    {
        CloseCreatedUI(UIType.YesOrNoPopUp);
    }
    public void OpenShopUI()
    {
        OpenCreatedUI(UIType.ShopUI);
    }
    public void CloseShopUI()
    {
        CloseCreatedUI(UIType.ShopUI);
    }
}
