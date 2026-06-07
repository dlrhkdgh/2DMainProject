using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] private UIButtonBase Button_Inven;
    [SerializeField] private UIButtonBase Button_Exit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        Button_Inven.BindOnClickButtonEvent(OnClick_InVenButton);
        Button_Exit.BindOnClickButtonEvent(OnClick_ExitButton);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnClick_InVenButton() {

        UIManager.Inst.OpenTownInventoryPopUp();
    }
    private void OnClick_ExitButton()
    {

        UIManager.Inst.OpenExitPopUp();
    }
}
