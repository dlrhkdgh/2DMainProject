using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] Transform _playerTransform;
    [SerializeField] Monster _monsterPrefab;

    [Header("스폰 설정")]
    [SerializeField] private float _minSpawnDistance = 10f;
    [SerializeField] private float _maxSpawnDistance = 15f;
    [SerializeField] private int _spawnMonsterPerSec = 2;
    private Transform _spawnTransform;
    private Coroutine _spawnCoroutine;
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
    public void SpawnMonster() {
        if (_playerTransform == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(_minSpawnDistance, _maxSpawnDistance);
        Vector3 spawnPosition = _playerTransform.position + (Vector3)(randomDirection * randomDistance);
        spawnPosition.z = 0f; 
        Monster newMonster = Instantiate(_monsterPrefab, spawnPosition, Quaternion.identity);

       
        newMonster.SetTargetTransform(_playerTransform);
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
            SpawnMonster();
        }
    }
}
