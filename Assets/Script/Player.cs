using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    public static Player Inst { get; set; }
    [Header("이동 설정")]
    [SerializeField] private Transform _spriteTransform;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [Header("총알 발사")]
    // [SerializeField] private GameObject _bulletPrefab; 
    [SerializeField] private Transform _firePoint;
    [SerializeField] private int FireBulletPerSec = 5;
    [Header("스텟")]
    [SerializeField] private float _invincibleDuration = 0.5f;
    [SerializeField] private Slider _hpSlider;
    [Header("기본 스텟")]
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private int _attack = 5;
    [SerializeField] private float _magnetRange = 5f;
    [SerializeField] private int _armor = 0;
    [SerializeField] private float _criticalPercent = 0f;

    [SerializeField] private BombBase Bomdprefab;

    [Header("인게임 추가 능력치")]
    public float _stageAddMaxHpPercent;
    public float _stageAddMoveSpeed;
    public int _stageAddAttack;
    public float _stageAddMagnetRange;
    public int _stageAddArmor;
    public float _stageAddCriticalPercent;

    [Header("최종 스텟")]
    public int FinalMaxHp => GetFinalMaxHp();
    public float FinalMoveSpeed => _moveSpeed + _stageAddMoveSpeed;
    public int FinalAttack => _attack + _stageAddAttack;
    public float FinalMagnetRange => GetFinalMagnetRange();
    public int FinalArmor => _armor + _stageAddArmor;
    public float FinalCriticalPercent => _criticalPercent + _stageAddCriticalPercent;

    public int PlayerLevel { get; set; }
    public int PlayerExp { get; set; }
    public int PlayerCurrentHp { get; set; } 

    private PlayerAnimController _animController;
    private Rigidbody2D _rigidBody;

    private float _horizontalInput;
    private float _verticalInput;
    private Vector2 _moveDirection;
    private CancellationTokenSource _shootCts;
    
    public bool _isInvincible= false;
    public bool _isOnBattle = false;
    public Action OnHpChanged;

    private Coroutine _fireSpeedBuffCoroutine;
    void Awake()
    {
        Inst = this;
        _rigidBody = GetComponent<Rigidbody2D>();
        _animController = GetComponent<PlayerAnimController>();
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (_spriteTransform != null)
        {
            _spriteRenderer = _spriteTransform.GetComponent<SpriteRenderer>();
        }
        else
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        ResetAddedPlayerStageStat();
        OnHpChanged += UpdateHpBar;
        ResetHp();
        
    }
    void Start()
    {


    }
    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        int finalDmg = damage - FinalArmor;
        if (finalDmg < 1)
        {
            finalDmg = 1;
        }
        PlayerCurrentHp -= finalDmg;
        Debug.Log($"플레이어 피격! 남은 체력: {PlayerCurrentHp}");
        
        if (PlayerCurrentHp <= 0)
        {
            PlayerCurrentHp = 0;
            PlayerDie();
            return;
        }
        OnHpChanged?.Invoke();
        StartCoroutine(CoInvincibleTimer());
    }
    private IEnumerator CoInvincibleTimer()
    {
        _isInvincible = true; 

        
        float timer = 0f;
        while (timer < _invincibleDuration)
        {

            Color color = _spriteRenderer.color;
            color.a = (color.a == 1.0f) ? 0.2f : 1.0f;
            _spriteRenderer.color = color;


            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }


        Color finalColor = _spriteRenderer.color;
        finalColor.a = 1.0f;
        _spriteRenderer.color = finalColor;

        _isInvincible = false; 
       // Debug.Log("무적 종료");
    }
    void Update()
    {
        PlayerMove();
        PlayerFlipOnShoot();
        //PlayerFlip();
        AnimatePlayer();
        PlayerUseItem();
        PlayerUseBomb();
    }
    private void OnDisable()
    {
        StopShooting();
        OnHpChanged -= UpdateHpBar;
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
        _rigidBody.linearVelocity = _moveDirection * FinalMoveSpeed;

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
            _spriteTransform.localScale = new Vector3(-1, 1, 1);
        }
        else if (mousePos.x < transform.position.x)
        {
            _spriteTransform.localScale = new Vector3(1, 1, 1);
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

        //int delayMilliseconds = Mathf.RoundToInt((1f / (float)FireBulletPerSec) * 1000f);
        try
        {
            while (true)
            {
                int delayMilliseconds = Mathf.RoundToInt((1f / (float)FireBulletPerSec) * 1000f);

                //  주의: 연사 속도가 0 이하가 되어 디바이드 바이 제로(무한대 렉)가 걸리는 것을 방어합니다.
                if (delayMilliseconds <= 0) delayMilliseconds = 100;
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
                StageManager.Inst.AddStageInventory(item.ItemId, 1);
                // GameManager.Inst.DebugPrintInventory();
            }
            else {
                StageManager.Inst.AddStageGold(item.GoldAmount);

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
    public void ResetHp()
    {
        PlayerCurrentHp = _maxHp;
        OnHpChanged?.Invoke();
    }
    public void PlayerGetExp(int expAmount) {
        PlayerExp = PlayerExp + expAmount;
        if (PlayerExp >= 100) {
            PlayerExp = PlayerExp % 100;
            PlayerLevelUp();
        }
    }
    public void PlayerLevelUp() {
        PlayerLevel++;
    }
    public void ResetAddedPlayerStageStat() {
     _stageAddMaxHpPercent=0;
     _stageAddMoveSpeed=0f;
     _stageAddAttack=0;
     _stageAddMagnetRange=0f;
     _stageAddArmor=0;
     _stageAddCriticalPercent=0f;
    }
    public int GetFinalMaxHp() 
    {
        float calcHp = _maxHp * (_stageAddMaxHpPercent / 100f);
        int finalMaxHp = _maxHp + Mathf.RoundToInt(calcHp);
        return finalMaxHp;
    }
    public float GetFinalMagnetRange() {
    return _magnetRange + (_magnetRange * (_stageAddMagnetRange /100f));
    }
    public void GetLevelUpHp(float addMaxHpPercent) {
        Debug.Log($"변경전 체력 {PlayerCurrentHp}");
        float calcHp = _maxHp * (addMaxHpPercent / 100f);
        PlayerCurrentHp += Mathf.RoundToInt(calcHp);
        OnHpChanged?.Invoke();
        Debug.Log($"변경후 체력 {PlayerCurrentHp}");
    }
    private void UpdateHpBar()
    {
      
        if (_hpSlider == null) return;
        float hpRatio = (float)PlayerCurrentHp / FinalMaxHp;
        _hpSlider.value = hpRatio;
        Debug.Log(" cpfurqk qusrud");
    }
    public bool UseHpPotion()
    {
        int newHp;
        if (PlayerCurrentHp >= FinalMaxHp) return false;
        else
        {
            newHp = PlayerCurrentHp + 30;
            if (newHp > FinalMaxHp)
                newHp = FinalMaxHp;
            PlayerCurrentHp = newHp;
            OnHpChanged?.Invoke();
            return true;
        }
    }
    public bool UseFireSpeedPotion(float duration, int multiplier) 
    {
        if (_fireSpeedBuffCoroutine != null)
        {
            return false;
        }
        _fireSpeedBuffCoroutine = StartCoroutine(FireSpeedBuffCo(duration, multiplier));
        return true;
    }
    private IEnumerator FireSpeedBuffCo(float duration, int multiplier)
    {
        
        int originalSpeed = FireBulletPerSec;
        
        FireBulletPerSec = FireBulletPerSec * multiplier;
        Debug.Log($" 버프 시작! 현재 속도: {FireBulletPerSec} (원본 백업: {originalSpeed})");

       
        float remainTime = duration;
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            yield return null;
        }

        FireBulletPerSec = originalSpeed;
        Debug.Log($" 버프 정상 만료! 현재 속도: {FireBulletPerSec}");

        
        _fireSpeedBuffCoroutine = null;
    }
    void PlayerUseItem() {

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            StageManager.Inst.UseItemToKey(0);
        }
        
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StageManager.Inst.UseItemToKey(1);
        }
        // 키보드 상단의 '3' 번 키를 눌렀을 때
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StageManager.Inst.UseItemToKey(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StageManager.Inst.UseItemToKey(3);
        }

    }
    void UseBomb()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        Vector2 shootDirection = (mousePos - _firePoint.position).normalized;
        StageManager.Inst.ThrowBomb(transform.position, shootDirection);
       
    }
    void PlayerUseBomb() 
    
    {

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseBomb();
        }
    }
    void PlayerDie() 
    {
        GameManager.Inst.FinishStage(false);
    
    }
}
