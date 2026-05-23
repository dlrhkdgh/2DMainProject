using UnityEngine;

public class PortalSensor : MonoBehaviour
{
    [SerializeField] private GameObject InfoUICanvus;
    bool _isOnPortal= false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (InfoUICanvus != null) {

            InfoUICanvus.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InfoUICanvus.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InfoUICanvus.SetActive(false);
        }
    }
}
