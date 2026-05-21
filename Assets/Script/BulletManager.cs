using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Inst { get; private set; }
    [SerializeField] private int _poolSize = 100;
    
    private BulletData _bulletData;
    private string _bulletAddressKey;
    private string _bulletId = "bullet_normalbullet_01";

    private List<Bullet> _bulletPool = new List<Bullet>();
    private int _currentPivot = 0;
    private bool _isShooting = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {

        _bulletData = DataManager.Inst.GetBulletData(_bulletId);
        _bulletAddressKey = _bulletData.PrefabPath;
        AsyncBulletPool().Forget();
       
    }
   
    private async UniTaskVoid AsyncBulletPool()
    {

        for (int i = 0; i < _poolSize; i++)
        {
            GameObject bulletResource = await ResourceManager.Inst.InstantiateAsync(_bulletAddressKey, transform);//리소스매니저에게 어드레서블을 주고 오브젝트를 받아옴

            if (bulletResource != null)
            {
                Bullet bullet = bulletResource.GetComponent<Bullet>();
                bullet.gameObject.SetActive(false);
                _bulletPool.Add(bullet);
            }
        }
    }
    public void FireBullet(Vector3 spawnPosition, Vector2 direction)
    {
        Bullet bulletToSpawn = null;

        for (int i = 0; i < _bulletPool.Count; i++)
        {
            int checkIndex = (_currentPivot + i) % _bulletPool.Count;
            if (!_bulletPool[checkIndex].gameObject.activeSelf)
            {
                bulletToSpawn = _bulletPool[checkIndex];
                _currentPivot = (checkIndex + 1) % _bulletPool.Count;
                break;
            }
        }
        if (bulletToSpawn != null)
        {
            bulletToSpawn.transform.position = spawnPosition;
            bulletToSpawn.InitBullet(_bulletData);
            bulletToSpawn.gameObject.SetActive(true);

            bulletToSpawn.Launch(direction);

        }
        else
        {
            Debug.LogWarning("총알 풀이 가득 찼습니다!");
        }
    }
}
