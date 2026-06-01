using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class BombSpawner : MonoBehaviour
{
    [SerializeField] private int _poolSize = 10;

    private BombData _bombData;
    private string _bombAddressKey;
    private string _bombId = "bomb_normal_01";
    private float _lastSpawnTime = -99f;
    private float _coolTime = 5f;
    private bool _isOnCoolTime = false;

    private List<BombBase> _bombPool = new List<BombBase>();
    private int _currentPivot = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        //  Inst = this;
    }
    private void Start()
    {

    }
    private void OnDisable()
    {
        foreach (var bomb in _bombPool)
        {
            if (bomb != null && bomb.gameObject.activeSelf)
            {
                bomb.gameObject.SetActive(false);
            }
        }
    }
    private async UniTaskVoid AsyncBombPool()
    {
        if (_bombPool.Count == 0)
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject bombResource = await ResourceManager.Inst.InstantiateAsync(_bombAddressKey, transform);//리소스매니저에게 어드레서블을 주고 오브젝트를 받아옴

                if (bombResource != null)
                {
                    BombBase bomb = bombResource.GetComponent<BombBase>();
                    bomb.gameObject.SetActive(false);
                    _bombPool.Add(bomb);
                }
            }
        }
    }
    public bool SpawnBomb(Vector3 spawnPosition, Vector2 direction)
    {
        if (_bombPool.Count == 0) return false;

        if (Time.time - _lastSpawnTime < _coolTime) {
            Debug.Log($"남은 쿨타임 {_coolTime- (Time.time - _lastSpawnTime)}");
            return false; } 

        BombBase bombToSpawn = null;

        for (int i = 0; i < _bombPool.Count; i++)
        {
            int checkIndex = (_currentPivot + i) % _bombPool.Count;
            if (!_bombPool[checkIndex].gameObject.activeSelf)
            {
                bombToSpawn = _bombPool[checkIndex];
                _currentPivot = (checkIndex + 1) % _bombPool.Count;
                break;
            }
        }
        if (bombToSpawn != null)
        {
            bombToSpawn.transform.position = spawnPosition;
            bombToSpawn.InitBomb(_bombData);
            bombToSpawn.gameObject.SetActive(true);
            bombToSpawn.ThrowBomb(direction);
            _lastSpawnTime = Time.time;
            return true;
        }
        else
        {
            Debug.LogWarning("폭탄 풀이 가득 찼습니다!");
            return false;
        }
    }
    public void InitBombSpawner()
    {
        _bombData = DataManager.Inst.GetBombData(_bombId);
        _bombAddressKey = _bombData.PrefabPath;
        AsyncBombPool().Forget();
    }
    public void ClearAndReleaseSpawner()
    {
        for (int i = 0; i < _bombPool.Count; i++)
        {

            if (_bombPool[i] != null)
            {
                Addressables.ReleaseInstance(_bombPool[i].gameObject);
            }
        }
        _bombPool.Clear();
        _currentPivot = 0;
    }
}
