using System.Collections.Generic;
using UnityEngine;

public class EnterStageUI : UIBase
{
    [SerializeField] private List<UISelectedSolt> _slotList;
    [SerializeField] UIButtonBase Button_Enter;
    [SerializeField] UIButtonBase Button_Exit;
    public int SelectedSlotId { get; set; } = -1;
    void Start()
    {
        for (int i = 0; i < _slotList.Count; i++) {

            _slotList[i].InitSelectedSolt(i, OnSlotSelect);
        }
        Button_Enter.BindOnClickButtonEvent(OnClick_ButtonEnter);
        Button_Exit.BindOnClickButtonEvent(OnClick_ButtonExit);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnSlotSelect(int id)
    {

        if (id < 0 || id >= _slotList.Count) return;
        if (SelectedSlotId == id)
        {
            _slotList[SelectedSlotId].SetSelectedState(false);
            SelectedSlotId = -1;
            return;
        }
        if (SelectedSlotId != -1)
        {
            _slotList[SelectedSlotId].SetSelectedState(false);
        }
       
        SelectedSlotId = id;
        _slotList[SelectedSlotId].SetSelectedState(true);
    }
    private void OnClick_ButtonEnter() {

        if(SelectedSlotId <0 || SelectedSlotId >= _slotList.Count)
            Debug.Log("현재선택된 스테이지 없음!");

        Debug.Log($"현재{SelectedSlotId+1}스테이지 선택됨");
    
    }
    private void OnClick_ButtonExit() {

        UIManager.Inst.ExitEnterStageUI();
    }
}
