using System;
using UnityEngine;
using UnityEngine.UI;

public class UIYesOrNoPopUp : UIBase
{
    [SerializeField] UIButtonBase Button_Yes;
    [SerializeField] UIButtonBase Button_No;
    [SerializeField] Text Text_Message;
    private Action<bool> _onResultCallback;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        if (Button_Yes == null) Debug.LogError($" [Yes 버튼]이 인스펙터에 안 꽂혔습니다!");
        if (Button_No == null) Debug.LogError($" [No 버튼]이 인스펙터에 안 꽂혔습니다!");

        if (Button_Yes == null || Button_No == null) return;
        Button_Yes.BindOnClickButtonEvent(OnClick_YesButton);
        Button_No.BindOnClickButtonEvent(OnClick_NoButton);
    }
    public void OpenYesOrNoPopUp(string message, Action<bool> callback)
    {
        this.gameObject.SetActive(true);

        Text_Message.text = message;
        _onResultCallback = callback; 
    }
    void OnClick_YesButton() 
    {
        _onResultCallback?.Invoke(true);
        UIManager.Inst.CloseYesOrNoPopUp();
    }
    void OnClick_NoButton()
    {
        _onResultCallback?.Invoke(false);
        UIManager.Inst.CloseYesOrNoPopUp();
    }
}
