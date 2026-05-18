using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _destroyTime = 3f;
    [SerializeField] private int _bulletDamage = 50;
    // [SerializeField] private Vector2 _moveDirection;
    private Rigidbody2D _rigidBody;
    private Coroutine _destroyCoroutine;
    //public int BulletDamage { get; private set; }
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Update()
    {
       
    }
    public void Launch(Vector2 direction)
    {
       
        Vector2 normalizedDirection = direction.normalized;
        Debug.Log($"x:{normalizedDirection.x} y:{normalizedDirection.y}");
       
        _rigidBody.linearVelocity = normalizedDirection * _moveSpeed;
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
        }
        _destroyCoroutine = StartCoroutine(DestroyAfterTimeCo());
    }
    private IEnumerator DestroyAfterTimeCo()
    {
         yield return new WaitForSeconds(_destroyTime);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
           Monster monster = collision.GetComponent<Monster>();
            if (monster != null)
            {
                
                monster.MonsterTakeDamage(_bulletDamage);
            }

            gameObject.SetActive(false);
        }
    }
}
