using System.Collections;
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
       // Debug.Log($"[레이어마스크 결과] : {_itemLayerMask}");
        if (_playerTransform == null) _playerTransform = transform.parent; 
    }
    private void OnEnable()
    {
       
    }
    private void OnDisable()
    {
        if (StageManager.Inst != null)
        {
            StageManager.Inst.OnPlayerStatChanged -= UpdateColliderRadius;
        }
    }
    void Start()
    {
        StageManager.Inst.OnPlayerStatChanged += UpdateColliderRadius;
        UpdateColliderRadius();
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

            yield return new WaitForSeconds(0.2f); 
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
    private void UpdateColliderRadius()
    {
        if (Player.Inst == null) return;

        // 플레이어가 들고 있는 최신 실시간 자석 범위 데이터를 가져와 콜라이더에 꽂아줍니다!
        _magnetRadius = Player.Inst.FinalMagnetRange;

        Debug.Log($"[자석] 플레이어 스텟 변경 방송 수신! 현재 자석 범위: {_magnetRadius}");
    }
}