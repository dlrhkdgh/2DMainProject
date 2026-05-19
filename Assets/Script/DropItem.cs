using System.Collections;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 8f;

    private Transform _targetTransform;
    private Coroutine _flyCoroutine;

   
    public void StartAttracting(Transform target)
    {
        //Debug.Log("atrrct strat");
        if (_flyCoroutine != null) return;

        _targetTransform = target;
        _flyCoroutine = StartCoroutine(MoveToPlayerCo());
    }

    private IEnumerator MoveToPlayerCo()
    {
        while (_targetTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetTransform.position, _moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnDisable()
    {
        if (_flyCoroutine != null)
        {
            StopCoroutine(_flyCoroutine);
            _flyCoroutine = null;
        }
        _targetTransform = null;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false); // 오브젝트 풀로 반환
        }
    }
}
