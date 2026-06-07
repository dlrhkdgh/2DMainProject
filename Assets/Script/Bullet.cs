using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    BulletData _defaultData;
    // [SerializeField] private Vector2 _moveDirection;
    
    private Rigidbody2D _rigidBody;
    private Coroutine _destroyCoroutine;
    public float MoveSpeed { get; set; } = 0f;
    public float DestroyTime { get; set; } = 0f;
    public int BulletDamage { get; set; } = 0;
    public bool _isCritical;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Update()
    {
       
    }
    private void OnDisable()
    {
        if (_destroyCoroutine != null)
        {
            StopCoroutine(_destroyCoroutine);
            _destroyCoroutine = null;
        }
    }
    public void Launch(Vector2 direction)
    {
       
        Vector2 normalizedDirection = direction.normalized;
       
        _rigidBody.linearVelocity = normalizedDirection * MoveSpeed;
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
         yield return new WaitForSeconds(DestroyTime);
        DestroyBullet();


    }
   
    public void DestroyBullet() {

        gameObject.SetActive(false);

    }
    public void InitBullet(BulletData bulletData ,int bonusDamage, float bonusSpeed , float bonusDestroyTime, bool iscritical) {
    
        _defaultData=bulletData;
        BulletDamage = _defaultData.Damage + bonusDamage;
        MoveSpeed = _defaultData.MoveSpeed + bonusSpeed;
        DestroyTime = _defaultData.DestroyTime + bonusDestroyTime;
        _isCritical = iscritical;
    
    }
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
       
        if (collision.CompareTag("Enemy"))
        {
           
            if (collision.TryGetComponent<Monster>(out var monster))
            {
                monster.MonsterTakeDamage(BulletDamage,_isCritical);
            }
            DestroyBullet();
        }
    }
}
