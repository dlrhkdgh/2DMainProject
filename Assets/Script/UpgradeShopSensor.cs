using UnityEngine;

public class UpgradeShopSensor : MonoBehaviour
{
    [SerializeField] private GameObject InfoUICanvus;
    bool _isOnSensor = false;

    void Start()
    {
        if (InfoUICanvus != null)
        {

            InfoUICanvus.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isOnSensor)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                UIManager.Inst.OpenUpgradeShopUI();
            }

        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InfoUICanvus.SetActive(true);
            _isOnSensor = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InfoUICanvus.SetActive(false);
            _isOnSensor = false;
        }
    }
}
