using UnityEngine;

public class WantedBoardSensor : MonoBehaviour
{
    [SerializeField] private GameObject InfoUICanvus;
    bool _isOnSensor = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
                UIManager.Inst.OpenWantedBoardUI();
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
