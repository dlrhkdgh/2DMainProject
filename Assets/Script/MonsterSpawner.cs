using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading;


public class MonsterSpawner : MonoBehaviour
{
    
    
    private string _monsterAddressKey;
    private Transform _playerTransform;
    [SerializeField] private int _poolSize = 100;

    [Header("스폰 설정")]
    [SerializeField] private float _minSpawnDistance = 10f;
    [SerializeField] private float _maxSpawnDistance = 15f;
    [SerializeField] private float _spawnMonsterPerSec = 1f;
    [Header("스폰 빈도 성장 가중치 (1분당 초당 스폰수 증가량)")]
    [SerializeField] private float _spawnGrowthScale = 0.5f; // 1분마다 초당 스폰 마릿수를 0.5마리씩 늘림
    [SerializeField] private float _baseSpawnPerSec = 1f;    // 게임 시작 시 최초 초당 스폰 마릿수

    [Header("난이도 가중치")]
    [SerializeField] private float _hpGrowthScale = 0.2f;    // 1분당 체력 20% 증가 수치
    [SerializeField] private float _speedGrowthScale = 0.05f;// 1분당 속도 5% 증가 수치

    private List<Monster> _monsterPool = new List<Monster>();
    private int _currentPivot = 0;
    private bool _isSpawning = false;
    MonsterData _monsterdata;

    private CancellationTokenSource _spawnCts;

   
   private void OnEnable()
    {
      
    }
    private void OnDisable()
    {

        _isSpawning = false;
        CleanUpCts(); // 스포너가 비활성화되면 스폰 무조건 종료
       
        foreach (var monster in _monsterPool)
        {
            if (monster != null && monster.gameObject.activeSelf)
            {
                monster.gameObject.SetActive(false);
            }
        }
    }
    private async UniTaskVoid AsyncMonsterPool() {
        _isSpawning = false;
        if (_monsterPool.Count == 0)
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject monsterResource = await ResourceManager.Inst.InstantiateAsync(_monsterAddressKey, transform);

                if (monsterResource != null)
                {
                    Monster monster = monsterResource.GetComponent<Monster>();
                    monster.gameObject.SetActive(false);
                    _monsterPool.Add(monster);
                }
            }
        }
       
    }
    private async UniTaskVoid AutoSpawnMonsterAsync(CancellationToken token)
    {
        _isSpawning = true;
        try
        {
            while (true)
            {
                float passedMinutes = StageManager.Inst.StageTimer / 60f;
                _spawnMonsterPerSec = _baseSpawnPerSec + (passedMinutes * _spawnGrowthScale);
                _spawnMonsterPerSec = Mathf.Min(_spawnMonsterPerSec, 10f);
                int delayMilliseconds = Mathf.RoundToInt((1f / (float)_spawnMonsterPerSec) * 1000f);
                await UniTask.Delay(delayMilliseconds, cancellationToken: token);
                if (_isSpawning)
                {
                    SpawnMonsterFromPool();
                }
            }
        }
        catch (System.OperationCanceledException)
        {
            Debug.Log("몬스터 스폰 루프가 안전하게 종료되었습니다.");
        }
    }
   
    public void SpawnMonsterFromPool() {
        if (_monsterPool.Count == 0 || _playerTransform == null)
        {
            return;
        }

        Monster monsterToSpawn = null;
        
        for (int i = 0; i < _monsterPool.Count; i++)//리스트의 피벗 위치에 몬스터가 비활성화 되어있으면 소환, 없으면 피벗을 옮김
        {
            int checkIndex = (_currentPivot + i) % _monsterPool.Count;
            if (_monsterPool[checkIndex].gameObject.activeSelf==false)
            {
                monsterToSpawn = _monsterPool[checkIndex];
                _currentPivot = (checkIndex + 1) % _monsterPool.Count;
                break;
            }
        }
        if (monsterToSpawn != null)
        {
            Vector3 spawnPosition = GetRandomTransformFromCircle2D(_maxSpawnDistance, _minSpawnDistance, _playerTransform.position);

            monsterToSpawn.transform.position = spawnPosition;
            monsterToSpawn.SetTargetTransform(_playerTransform);

            float currentSeconds = StageManager.Inst.StageTimer;
            float passedMinutes = currentSeconds / 30f;
            float hpMultiplier = 1.0f + (passedMinutes * _hpGrowthScale);
            float speedMultiplier = 1.0f + (passedMinutes * _speedGrowthScale);

            float currentEliteChance = 0.02f + (passedMinutes * 0.01f);
            bool isElite = Random.value < currentEliteChance;

            monsterToSpawn.InitMonster(_monsterdata, hpMultiplier, speedMultiplier, isElite);
            monsterToSpawn.gameObject.SetActive(true);
           
        }
        else
        {
            Debug.LogWarning("몬스터 풀이 가득 찼습니다!");
        }
    }
    private void CleanUpCts()
    {
        if (_spawnCts != null)
        {
            _spawnCts.Cancel();
            _spawnCts.Dispose();
            _spawnCts = null;
        }
    }
    private void OnDrawGizmosSelected()
    {
         if (_playerTransform == null) return;

         Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_playerTransform.position, _minSpawnDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_playerTransform.position, _maxSpawnDistance);
       
    }
  
    public Vector3 GetRandomTransformFromCircle2D(float maxSize, float minSize ,Vector3 centerPos ) {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSize, maxSize);
        Vector3 resultPosition = centerPos + (Vector3)(randomDirection * randomDistance);
        resultPosition.z = 0f;
        return resultPosition;
    }
    public void InitMonsterSpawner(Transform playerTransform , string monsterId) {

        _playerTransform = playerTransform;
        _monsterdata = DataManager.Inst.GetMonsterData(monsterId);
        if (_monsterdata == null) return;
        _monsterAddressKey = _monsterdata.PrefabPath;
        Debug.Log($"<color=green>[성공] 패스 받아옴 성공: {_monsterAddressKey}</color>");
        CleanUpCts();
        _spawnCts = new CancellationTokenSource();
       

    }
    public void StartSpawn() 
    {
        AsyncMonsterPool().Forget();
        AutoSpawnMonsterAsync(_spawnCts.Token).Forget();
    }
    public void OnlySetSpawn()
    {
        AsyncMonsterPool().Forget();
    }
    public void ClearAndReleaseSpawner()
    {
        CleanUpCts();

        for (int i = 0; i < _monsterPool.Count; i++) {

            if (_monsterPool[i] != null) 
            {
                Addressables.ReleaseInstance(_monsterPool[i].gameObject);
            }
        }
        _monsterPool.Clear();
        _currentPivot = 0;
        _isSpawning = false;
    }
    public void ResumeSpawn()
    {
        if (_isSpawning) return; // 이미 돌고 있다면 무시

        _isSpawning = true;

        // 토큰 소스 안전하게 초기화 후 다시 루프 가동
        CleanUpCts();
        _spawnCts = new CancellationTokenSource();

        AutoSpawnMonsterAsync(_spawnCts.Token).Forget();
        Debug.Log($" [{gameObject.name}] 몬스터 스폰이 재개되었습니다.");
    }
    public void StopSpawn()
    {
        _isSpawning = false;
        CleanUpCts(); // 루프를 도는 UniTask.Delay 토큰을 취소하여 즉시 스폰을 멈춥니다.
        Debug.Log($" [{gameObject.name}] 몬스터 스폰이 일시 중지되었습니다.");
    }
}
