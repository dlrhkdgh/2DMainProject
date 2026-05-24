using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading;


public class MonsterSpawner : MonoBehaviour
{
    
    [SerializeField] private AssetReference _monsterAddressableRef;
    private string _monsterAddressKey;
    private Transform _playerTransform;
    [SerializeField] private int _poolSize = 100;
    [SerializeField] private string _spawnMonsterId = "monster_cowbombie_01";

    [Header("스폰 설정")]
    [SerializeField] private float _minSpawnDistance = 10f;
    [SerializeField] private float _maxSpawnDistance = 15f;
    [SerializeField] private int _spawnMonsterPerSec = 4;

    private List<Monster> _monsterPool = new List<Monster>();
    private int _currentPivot = 0;
    private bool _isSpawning = false;
    MonsterData _monsterdata;

    private CancellationTokenSource _spawnCts;

   
   private void OnEnable()
    {
        Debug.Log("스포너활성화");
        
        
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
        _isSpawning = true;
        AutoSpawnMonsterAsync(_spawnCts.Token).Forget();
    }
    private async UniTaskVoid AutoSpawnMonsterAsync(CancellationToken token)
    {

        int delayMilliseconds = Mathf.RoundToInt((1f / (float)_spawnMonsterPerSec) * 1000f);
        try
        {
            while (true)
            {
                // 오브젝트가 파괴되거나(_spawnCts 취소) 구역이 바뀔 때 안전하게 탈출합니다.
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
            monsterToSpawn.InitMonster(_monsterdata);
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
    public void InitMonsterSpawner(Transform playerTransform) {
        _playerTransform = playerTransform;
        _monsterdata = DataManager.Inst.GetMonsterData(_spawnMonsterId);

        _monsterAddressKey = _monsterdata.PrefabPath;
        Debug.Log($"<color=green>[성공] 패스 받아옴 성공: {_monsterAddressKey}</color>");
        CleanUpCts();
        _spawnCts = new CancellationTokenSource();
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
}
