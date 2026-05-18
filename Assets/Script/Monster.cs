using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] Transform _targetTransform;
    [SerializeField] float _moveSpeed = 4f;
    [SerializeField] int _maxHp = 100;

    private Rigidbody2D _rigidBody;
    private Vector2 _moveDirection;
    public int CurrentHp { get; private set; }

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    private void OnEnable()
    {
        CurrentHp = _maxHp;
    }
    void Update()
    {
        if (_targetTransform != null)
        {

            _moveDirection = (_targetTransform.position - transform.position).normalized;
            MonsterFlip();
        }
        else
        {
            _moveDirection = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        _rigidBody.linearVelocity = _moveDirection * _moveSpeed;
    }

    public void SetTarget(Transform newTarget)
    {
        _targetTransform = newTarget;
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

        int newHp = CurrentHp - damage;

        if (newHp < 0) {
         newHp = 0;
        }
        CurrentHp = newHp;
        if (CurrentHp == 0) 
        {
            MonsterDie();
        }
    }
    public void MonsterDie() {
    
    gameObject.SetActive(false);
    
    }
}
