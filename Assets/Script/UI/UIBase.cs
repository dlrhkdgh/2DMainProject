using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] private UIType _uiType;
    public UIType uiType => _uiType;

    public virtual void OpenUI()
    {
        Debug.Log("OpenUI");

        gameObject.SetActive(true);
    }
    public virtual void CloseUI()
    {
        Debug.Log("CLoseUI");

        gameObject.SetActive(false);
    }
}
