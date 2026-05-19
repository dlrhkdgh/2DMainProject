using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Inst { get; private set; }
    [SerializeField] private int _poolSize = 100;
    [SerializeField] private AssetReference _bulletAddressableRef;

    private List<Bullet> _bulletPool = new List<Bullet>();
    private int _currentPivot = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        StartCoroutine(AsyncBulletPoolCo());
    }
    private IEnumerator AsyncBulletPoolCo()//비동기 오브젝트 풀링
    {
        for (int i = 0; i < _poolSize; i++)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(_bulletAddressableRef, transform);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Bullet bullet = handle.Result.GetComponent<Bullet>();
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
            bulletToSpawn.gameObject.SetActive(true);

            bulletToSpawn.Launch(direction);

        }
        else
        {
            Debug.LogWarning("총알 풀이 가득 찼습니다!");
        }
    }
}
