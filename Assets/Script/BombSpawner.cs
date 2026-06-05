using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public class BombSpawner : MonoBehaviour
{
    [SerializeField] private int _poolSize = 10;

    string _bombEffectAddressKey = "Prefab/BombEffect";
    private BombData _bombData;
    private string _bombAddressKey;
    private string _bombId = "bomb_normal_01";
    private float _lastSpawnTime = -99f;
    private float _coolTime = 5.0f;
    //private bool _isOnCoolTime = false;

    private List<BombBase> _bombPool = new List<BombBase>();
    private List<BombEffect> _bombEffectPool = new List<BombEffect>();
    private int _currentPivot = 0;
    private int _currentPivot2 = 0;

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
        foreach (var effect in _bombEffectPool)
        {
            if (effect != null && effect.gameObject.activeSelf)
            {
                effect.gameObject.SetActive(false);
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
    private async UniTaskVoid AsyncBombEffectPool()
    {
        if (_bombEffectPool.Count == 0)
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject bombEffectResource = await ResourceManager.Inst.InstantiateAsync(_bombEffectAddressKey, transform);//리소스매니저에게 어드레서블을 주고 오브젝트를 받아옴

                if (bombEffectResource != null)
                {
                    BombEffect effect = bombEffectResource.GetComponent<BombEffect>();
                    effect.gameObject.SetActive(false);
                    _bombEffectPool.Add(effect);
                }
            }
        }
    }
    public void SpawnBombEffect(Vector3 spawnPosition)
    {
        if (_bombEffectPool.Count == 0) return ;

        BombEffect effectToSpawn = null;

        for (int i = 0; i < _bombEffectPool.Count; i++)
        {
            int checkIndex = (_currentPivot2 + i) % _bombEffectPool.Count;
            if (!_bombEffectPool[checkIndex].gameObject.activeSelf)
            {
                effectToSpawn = _bombEffectPool[checkIndex];
                _currentPivot2 = (checkIndex + 1) % _bombEffectPool.Count;
                break;
            }
        }
        if (effectToSpawn != null)
        {
            effectToSpawn.transform.position = spawnPosition;
            effectToSpawn.gameObject.SetActive(true);
            return;
        }
        else
        {
            Debug.LogWarning("폭탄이펙트 풀이 가득 찼습니다!");
            return;
        }
    }
    public void InitBombSpawner()
    {
        _bombData = DataManager.Inst.GetBombData(_bombId);
        _bombAddressKey = _bombData.PrefabPath;
        AsyncBombPool().Forget();
        AsyncBombEffectPool().Forget();
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
        for (int i = 0; i < _bombEffectPool.Count; i++)
        {

            if (_bombEffectPool[i] != null)
            {
                Addressables.ReleaseInstance(_bombEffectPool[i].gameObject);
            }
        }
        _bombEffectPool.Clear();
        _currentPivot2 = 0;
    }
}
