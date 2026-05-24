using UnityEngine;

public class InFieldUI : UIBase
{
    [SerializeField] UIButtonBase Button_EarnItemPopUp;
    [SerializeField] UIButtonBase Button_ExitStage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Button_EarnItemPopUp.BindOnClickButtonEvent(OnClick_EarnItemPopUp);
        Button_ExitStage.BindOnClickButtonEvent(OnClick_ExitSatge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick_EarnItemPopUp()
    {

        UIManager.Inst.OpenEarnItemPopUp();
    }
    void OnClick_ExitSatge() {
        GameManager.Inst.FinishStage();
        UIManager.Inst.CloseInFeildUI();
    }
}
