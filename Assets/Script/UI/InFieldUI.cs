using UnityEngine;

public class InFieldUI : UIBase
{
    [SerializeField] UIButtonBase Button_EarnItemPopUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button_EarnItemPopUp.BindOnClickButtonEvent(OnClick_EarnItemPopUp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClick_EarnItemPopUp()
    {

        UIManager.Inst.OpenEarnItemPopUp();
    }
}
