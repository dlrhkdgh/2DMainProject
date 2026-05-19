using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets; 
using UnityEngine.ResourceManagement.AsyncOperations;

public class MonsterSpawner : MonoBehaviour
{
    
    [SerializeField] private AssetReference _monsterAddressableRef; 
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private int _poolSize = 100; 

    [Header("스폰 설정")]
    [SerializeField] private float _minSpawnDistance = 10f;
    [SerializeField] private float _maxSpawnDistance = 15f;
    [SerializeField] private int _spawnMonsterPerSec = 10;

    private List<Monster> _monsterPool = new List<Monster>();
    private int _currentPivot = 0; 
    private Transform _spawnTransform;
    private Coroutine _spawnCoroutine;
    private void Start()
    {
        StartCoroutine(AsyncMonsterPoolCo());
    }
    private IEnumerator AsyncMonsterPoolCo()//비동기 오브젝트 풀링
    {
        for (int i = 0; i < _poolSize; i++)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(_monsterAddressableRef, transform);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Monster monster = handle.Result.GetComponent<Monster>();
                monster.gameObject.SetActive(false); 
                _monsterPool.Add(monster);
            }
        }
        StartCoroutine(AutoSpawnMonsterCo());
    }
    private void OnEnable()
    {
        Debug.Log("스포너활성화");
        _spawnCoroutine = StartCoroutine(AutoSpawnMonsterCo());
    }
    private void OnDisable()
    {
        
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
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
            monsterToSpawn.gameObject.SetActive(true);
            //Debug.Log($"{_currentPivot}번쨰 몬스터 소환");
        }
        else
        {
            Debug.LogWarning("몬스터 풀이 가득 찼습니다!");
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
    private IEnumerator AutoSpawnMonsterCo()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(5f / (float)_spawnMonsterPerSec);
            SpawnMonsterFromPool();
        }
    }
    public Vector3 GetRandomTransformFromCircle2D(float maxSize, float minSize ,Vector3 centerPos ) {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSize, maxSize);
        Vector3 resultPosition = centerPos + (Vector3)(randomDirection * randomDistance);
        resultPosition.z = 0f;
        return resultPosition;
    }
}
