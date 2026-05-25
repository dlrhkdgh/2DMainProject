using UnityEngine;

public class MainUI : UIBase
{
    [SerializeField] private UIButtonBase Button_Inven;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        Button_Inven.BindOnClickButtonEvent(OnClick_InVenButton);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnClick_InVenButton() {

        UIManager.Inst.OpenTownInventoryPopUp();
    }
}
