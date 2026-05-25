using System.Collections;
using UnityEngine;


public class Monster : MonoBehaviour
{
    private MonsterData _defaultData;
    private DropTableData _dropTableData;
    public Transform _targetTransform;

    private Rigidbody2D _rigidBody;
    private MonsterAnimController _animController;
    private Vector2 _moveDirection;

    private bool _isDying = false;//죽는 중에 피격이나 애니메이션을 스킵
    private int _droppedCoin;

    public int CurrentHp { get; private set; } = 0;
    public int AttackDamage { get; private set; } = 0;
    public float MoveSpeed { get; private set; } = 0f;
    public string DropTableId{ get; set; }
    public int MonsterExp { get; set; }

    void Awake()
    {
        
        _rigidBody = GetComponent<Rigidbody2D>();
        _animController = GetComponent<MonsterAnimController>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void OnEnable()
    {
        _isDying = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = true;
        }
    }
    void Update()
    {
        if (_isDying) return;
        if (_targetTransform != null)
        {

            _moveDirection = (_targetTransform.position - transform.position).normalized;
            MonsterFlip();
            if (_animController != null)
            {
                _animController.SetAnimState(MonsterAnimState.Walk);
            }
        }
        else
        {
            _moveDirection = Vector2.zero;
            if (_animController != null)
            {
                _animController.SetAnimState(MonsterAnimState.Idle);
            }
        }
    }

    void FixedUpdate()
    {
        if (_isDying)
        {
            _rigidBody.linearVelocity = Vector2.zero;
            return;
        }
        _rigidBody.linearVelocity = _moveDirection * MoveSpeed;
    }

   
    private void MonsterFlip() {

        if (_moveDirection.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_moveDirection.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }
    public void MonsterTakeDamage(int damage) {

        if (_isDying) return;
        int newHp = CurrentHp - damage;

        if (newHp < 0) {
         newHp = 0;
        }
        CurrentHp = newHp;
        if (CurrentHp == 0)
        {
            MonsterDie();
        }
        else
        {
           
            if (_animController != null)
            {
                _animController.SetAnimState(MonsterAnimState.Damaged);
            }
        }
    }
    public void MonsterDie()
    {
        _isDying = true;
        StartCoroutine(DieSequenceCo());
    }

    private IEnumerator DieSequenceCo()
    {
        if (_animController != null)
        {
            _animController.SetAnimState(MonsterAnimState.Die);
        }

        // 콜라이더 끄기 안전장치
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        //한프레임 기다린후 다음 애니메이션의 길이 가져옴
        yield return null;

        float animLength = 1f; 
        if (_animController != null)
        {
            animLength = _animController.GetCurrentAnimLength();
        }
        if (StageManager.Inst != null)
        {
            _droppedCoin = Random.Range(1, 5);

            StageManager.Inst.DropItemFromMonster(transform.position, DropTableId);
        }
        if (MonsterExp > 0) 
        {
            StageManager.Inst.PlayerGetExp(MonsterExp);
        }
        // 애니메이션 시간만큼 대기
        yield return new WaitForSeconds(animLength);
        
        gameObject.SetActive(false);
    }
    
    public void SetTargetTransform(Transform newTargetTransform) {

        if(newTargetTransform != null)
        _targetTransform= newTargetTransform;

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.TryGetComponent<Bullet>(out Bullet bullet))
        {
           
            MonsterTakeDamage(bullet.BulletDamage);

            bullet.DestroyBullet();
        }
    }
    public void InitMonster(MonsterData monsterData) {

        if (monsterData == null) return;

        _defaultData = monsterData;

        CurrentHp = _defaultData.MaxHp;
        MoveSpeed = _defaultData.MoveSpeed;
        AttackDamage = _defaultData.AttackDamage;
        DropTableId = _defaultData.DropTableId;
        MonsterExp= _defaultData.MonsterExp;
        Debug.Log($"[{_defaultData.Name}] 체력 {CurrentHp}, 속도 {MoveSpeed}로 초기화 완료!");
    }
    
}
