using System.Collections;
using UnityEngine;

public class BombBase : MonoBehaviour
{

    private Rigidbody2D _rigidBody;
    private Coroutine _explosionCoroutine;
   
   // [SerializeField] private GameObject _explosionEffectPrefab; 
    

    private int _itemLayerMask;
    private Collider2D[] _hitMonsterArr = new Collider2D[100];
    public float ExplosionRadius { get; set; } = 0f;
    public float MoveSpeed { get; set; } = 0f;
    public float ExplosionTime { get; set; } = 1.0f;
    public int BombDamage { get; set; } = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _itemLayerMask = LayerMask.GetMask("Monster");
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnDisable()
    {
        if (_explosionCoroutine != null)
        {
            StopCoroutine(_explosionCoroutine);
            _explosionCoroutine = null;
        }
    }
    public void ThrowBomb(Vector2 direction)
    {
        gameObject.SetActive(true);
        if (_rigidBody == null)
        {
            _rigidBody = GetComponent<Rigidbody2D>();
        }
        Vector2 normalizedDirection = direction.normalized;

        _rigidBody.linearVelocity = normalizedDirection * MoveSpeed;
        float angle = Mathf.Atan2(normalizedDirection.y, normalizedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        if (_explosionCoroutine != null)
        {
            StopCoroutine(_explosionCoroutine);
        }
        _explosionCoroutine = StartCoroutine(ExplosionAfterTimeCo());
    }
    private IEnumerator ExplosionAfterTimeCo()
    {
        yield return new WaitForSeconds(ExplosionTime);
        BombExplosion();
    }
    public void DestroyBomb()
    {

        gameObject.SetActive(false);

    }
    protected virtual void BombExplosion()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_itemLayerMask);
        filter.useLayerMask = true;
        //filter.useTriggers = true;

            int count = Physics2D.OverlapCircle(transform.position, ExplosionRadius, filter, _hitMonsterArr);//오버랩
                                                                                                             //Debug.Log($"count:{count}");
        if (count > 0)
        {
            string monsterNames = "";
            for (int i = 0; i < count; i++)
            {
                if (_hitMonsterArr[i] != null)
                {
                    // 걸린 몬스터들의 이름을 쉼표(,)로 이어 붙입니다.
                    monsterNames += _hitMonsterArr[i].gameObject.name + (i < count - 1 ? ", " : "");
                }
            }
            Debug.Log($" [폭발 감지] 총 {count}마리의 몬스터가 범위 내에 걸림! -> ({monsterNames})");
        }
        else
        {
            Debug.LogWarning(" [폭발 빗나감] 범위 내에 감지된 몬스터가 아무도 없습니다.");
        }
        for (int i = 0; i < count; i++)
            {
            if (_hitMonsterArr[i] == null) continue;

            Monster monster = _hitMonsterArr[i].GetComponent<Monster>();
                if (monster != null)
                {
                Debug.LogWarning($"{BombDamage}의 폭발데미지 ");
                monster.MonsterTakeDamage(BombDamage,false);
                }
                else
                {
               
                Debug.Log("monster없음");
                }
            }
        StageManager.Inst.SpawnEffect(transform.position);
        DestroyBomb();
    }
    public void InitBomb(BombData data)
    {
        BombDamage = data.BombDamage;
        ExplosionRadius = data.ExplosionRadius;
        MoveSpeed = data.MoveSpeed;
    }
    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.green;
       
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
