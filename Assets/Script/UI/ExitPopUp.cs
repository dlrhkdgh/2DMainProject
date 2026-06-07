using System;
using UnityEngine;
using UnityEngine.UI;

public class ExitPopUp : UIBase
{
    [SerializeField] UIButtonBase Button_Yes;
    [SerializeField] UIButtonBase Button_No;
    [SerializeField] Text Text_Message;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        if (Button_Yes == null) Debug.LogError($" [Yes 버튼]이 인스펙터 없습니다!");
        if (Button_No == null) Debug.LogError($" [No 버튼]이 인스펙터에 없습니다!");

        if (Button_Yes == null || Button_No == null) return;
        Button_Yes.BindOnClickButtonEvent(OnClick_YesButton);
        Button_No.BindOnClickButtonEvent(OnClick_NoButton);
    }
    void OnClick_YesButton()
    {
       Application.Quit();
       
    }
    void OnClick_NoButton()
    {
        UIManager.Inst.CloseExitPopUp();
    }
}
