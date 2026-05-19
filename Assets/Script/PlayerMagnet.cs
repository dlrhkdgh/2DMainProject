using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerMagnet : MonoBehaviour
{
    [SerializeField] private float _magnetRadius = 5f;
    [SerializeField] private Transform _playerTransform;

    private int _itemLayerMask;
    private Collider2D[] _hitItemArr = new Collider2D[100];
    
    void Awake()
    {
        _itemLayerMask = LayerMask.GetMask("DropItem");
        Debug.Log($"[레이어마스크 결과] : {_itemLayerMask}");
        if (_playerTransform == null) _playerTransform = transform.parent; 
    }

    void Start()
    {
        StartCoroutine(ScanItemsCo());
    }
    private IEnumerator ScanItemsCo()
    {
        Debug.Log("코루틴 시작");
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_itemLayerMask);
        filter.useLayerMask = true;
        filter.useTriggers = true;//istrigger인 오브젝트들도 체크

        while (true)
        {

            yield return new WaitForSeconds(2f); 
            Debug.Log("드롭템스캔");
            
            int count = Physics2D.OverlapCircle(transform.position, _magnetRadius, filter, _hitItemArr);//오버랩
            //Debug.Log($"count:{count}");
            for (int i = 0; i < count; i++)
            {
                DropItem droppedItem = _hitItemArr[i].GetComponent<DropItem>();
                if (droppedItem != null)
                {
                    droppedItem.StartAttracting(_playerTransform);
                }
                else
               {

                   Debug.Log("droppedItem없음");
                }
               
            }
        }

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _magnetRadius);
    }
}