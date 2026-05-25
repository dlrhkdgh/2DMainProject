using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript : UIBase
{
    [SerializeField] UIButtonBase Button_GameStart;
    [SerializeField] UIButtonBase Button_GameExit;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button_GameStart.BindOnClickButtonEvent(OnClick_GameStart);
        Button_GameExit.BindOnClickButtonEvent(OnClick_GameExit);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick_GameStart() {
        UIManager.Inst.CloseLobbyUI();
        UIManager.Inst.OpenMainUI();
    }
    void OnClick_GameExit() {
    
    UIManager.Inst.CloseLobbyUI();
    }
   
}
