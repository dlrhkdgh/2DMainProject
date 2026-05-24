using Cysharp.Threading.Tasks;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Inst { get; set; }
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
    private CancellationTokenSource _shootCts;

    void Awake()
    {
        Inst = this;
        _rigidBody = GetComponent<Rigidbody2D>();
        _animController = GetComponent<PlayerAnimController>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Start()
    {


    }


    void Update()
    {
        PlayerMove();
        PlayerFlipOnShoot();
        //PlayerFlip();
        AnimatePlayer();
    }
    private void OnDisable()
    {
        StopShooting();
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

        if (StageManager.Inst != null)
        {
            StageManager.Inst.StartFireBullet(_firePoint.position, shootDirection);
        }
    }

    private async UniTaskVoid AutoFireBulletAsync(CancellationToken token)
    {

        int delayMilliseconds = Mathf.RoundToInt((1f / (float)FireBulletPerSec) * 1000f);
        try
        {
            while (true)
            {

                await UniTask.Delay(delayMilliseconds, cancellationToken: token);


                Shoot();
            }
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("슈팅 루프가 안전하게 종료되었습니다.");
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
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.TryGetComponent<DropItem>(out DropItem item))
        {
            if (item.ItemId != "item_coin_01")
            {
                //StageManager.Inst.AcquireItem(item.ItemCode);
                GameManager.Inst.AddInventory(item.ItemId, 1);
               // GameManager.Inst.DebugPrintInventory();
            }
            else {
                GameManager.Inst.Gold = GameManager.Inst.Gold + item.GoldAmount;

            }
            Debug.Log($"아이템 먹음{item.ItemId} ");
            collision.gameObject.SetActive(false);
        }
    }
    public void StartShooting() {
        CleanUpCts();
        _shootCts = new CancellationTokenSource();
        AutoFireBulletAsync(_shootCts.Token).Forget();

    }
    public void StopShooting() {

        CleanUpCts();
    }
    private void CleanUpCts()
    {
        if (_shootCts != null)
        {
            _shootCts.Cancel();
            _shootCts.Dispose();
            _shootCts = null;
        }
    }
}
