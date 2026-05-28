using System;
using UnityEngine;
using UnityEngine.UI;

public class UISelectedSolt : MonoBehaviour
{
    [SerializeField] UIButtonBase Button_Selected;
    [SerializeField] private Image Image_Select;
    
    private Action<int> _onClickCallback;
    public int SlotId { get; set; } = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InitSelectedSolt(int id, Action<int> callback) {
        SlotId = id;
        _onClickCallback = callback;
        if (Button_Selected != null)
        {
            Button_Selected.BindOnClickButtonEvent(OnClick_SelectSlot);
        }

    }
    public void SetSelectedState(bool isSelected) {

        Image_Select.gameObject.SetActive(isSelected);

    }
    public void OnClick_SelectSlot()
    {
        _onClickCallback?.Invoke(SlotId);
    }

}   
