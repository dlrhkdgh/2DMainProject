using System.Collections;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    [SerializeField] float _moveSpeed = 4f;
    [SerializeField] int _maxHp = 100;

    private Rigidbody2D _rigidBody;
    private MonsterAnimController _animController;
    private Vector2 _moveDirection;
    private bool _isDying = false;//죽는 중에 피격이나 애니메이션을 스킵
    public int CurrentHp { get; private set; }

    void Awake()
    {
        
        _rigidBody = GetComponent<Rigidbody2D>();
        _animController = GetComponent<MonsterAnimController>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void OnEnable()
    {
        CurrentHp = _maxHp;
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
        _rigidBody.linearVelocity = _moveDirection * _moveSpeed;
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
        if (DropItemManager.Inst != null)
        {
            DropItemManager.Inst.DropItemInField(transform.position, "1");
        }
        // 애니메이션 시간만큼 대기
        yield return new WaitForSeconds(animLength);
        
        gameObject.SetActive(false);
    }
    
    public void SetTargetTransform(Transform newTargetTransform) {

        if(newTargetTransform != null)
        _targetTransform= newTargetTransform;

    }
}
