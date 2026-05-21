using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets; 
using UnityEngine.ResourceManagement.AsyncOperations;


public class MonsterSpawner : MonoBehaviour
{
    
    [SerializeField] private AssetReference _monsterAddressableRef;
    private string _monsterAddressKey;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private int _poolSize = 100;
    [SerializeField] private string _spawnMonsterId = "monster_catussilme_01";

    [Header("스폰 설정")]
    [SerializeField] private float _minSpawnDistance = 10f;
    [SerializeField] private float _maxSpawnDistance = 15f;
    [SerializeField] private int _spawnMonsterPerSec = 10;

    private List<Monster> _monsterPool = new List<Monster>();
    private int _currentPivot = 0;
    private bool _isSpawning = false;

    private void Start()
    {
        // 1. 안전장치: 데이터가 안 불려왔으면 강제 로드
        //if (DataManager.Inst.MonsterDataList == null || DataManager.Inst.MonsterDataList.Count == 0)
        //{
        //    Debug.LogWarning("[디버그] 데이터가 비어있어 강제 로드를 실행합니다.");
        //    DataManager.Inst.LoadFullData();
        //}
      
        var monsterDic = DataManager.Inst.GetMonsterData(_spawnMonsterId);

        _monsterAddressKey = monsterDic.PrefabPath;
        Debug.Log($"<color=green>[성공] 패스 받아옴 성공: {_monsterAddressKey}</color>");

        AsyncMonsterPool().Forget();

    }
   private void OnEnable()
    {
        Debug.Log("스포너활성화");
        
        //_isSpawning = true;
    }
    private void OnDisable()
    {

        _isSpawning = false;
    }
    private async UniTaskVoid AsyncMonsterPool() {
        _isSpawning = false;
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject monsterResource = await ResourceManager.Inst.InstantiateAsync(_monsterAddressKey, transform);//리소스매니저에게 어드레서블을 주고 오브젝트를 받아옴

            if (monsterResource != null)
            {
                Monster monster = monsterResource.GetComponent<Monster>();
                monster.gameObject.SetActive(false);
                _monsterPool.Add(monster);
            }
        }
        _isSpawning = true;
        AutoSpawnMonsterAsync().Forget();
    }
    private async UniTaskVoid AutoSpawnMonsterAsync()
    {

        int delayMilliseconds = Mathf.RoundToInt((1f / (float)_spawnMonsterPerSec) * 1000f);
        var cancellationToken = this.GetCancellationTokenOnDestroy();
        while (true)
        {
            await UniTask.Delay(delayMilliseconds, cancellationToken: cancellationToken);
            if (_isSpawning)
            {
                SpawnMonsterFromPool();
            }
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
  
    public Vector3 GetRandomTransformFromCircle2D(float maxSize, float minSize ,Vector3 centerPos ) {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minSize, maxSize);
        Vector3 resultPosition = centerPos + (Vector3)(randomDirection * randomDistance);
        resultPosition.z = 0f;
        return resultPosition;
    }
}
