using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 4f;
    [Header("총알 발사")]
   // [SerializeField] private GameObject _bulletPrefab; 
    [SerializeField] private Transform _firePoint;
    [SerializeField] private int FireBulletPerSec = 5;
    private PlayerAnimController _animController;
    private Rigidbody2D _rigidBody;

    private float _horizontalInput;
    private float _verticalInput;
    private Vector2 _moveDirection;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _animController = GetComponent<PlayerAnimController>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Start()
    {
        StartCoroutine(AutoFireBulletCo());
    }

    
    void Update()
    {
        PlayerMove();
        PlayerFlipOnShoot();
        //PlayerFlip();
        AnimatePlayer();
    }
    private void PlayerMove()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
        _moveDirection = new Vector2(_horizontalInput, _verticalInput);

        if (_moveDirection.magnitude > 1)
        {
            _moveDirection = _moveDirection.normalized;
        }
        _rigidBody.linearVelocity = _moveDirection * _moveSpeed;
      
    }
    private void PlayerFlip() 
    {
        if (_horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (_horizontalInput < 0) 
        { 
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    private void PlayerFlipOnShoot()
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        
        if (mousePos.x > transform.position.x)
        {
            
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (mousePos.x < transform.position.x)
        {
            
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    private void Shoot()//마우스방향따라
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 shootDirection = (mousePos - _firePoint.position).normalized; 
       
        if (BulletManager.Inst != null)
        {
            BulletManager.Inst.FireBullet(_firePoint.position,shootDirection);
        }
    }
    private IEnumerator AutoFireBulletCo()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / (float)FireBulletPerSec);
            Shoot();
        }
    }
    private void AnimatePlayer()
    {
        if (_animController == null) return;

         if (_horizontalInput == 0 && _verticalInput == 0)
        {
            _animController.SetAnimState(PlayerAnimState.Shoot);
        }
        else
        {
            _animController.SetAnimState(PlayerAnimState.ShootAndWalk);
        }
    }
}
