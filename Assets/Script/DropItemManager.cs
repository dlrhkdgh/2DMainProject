using Cysharp.Threading.Tasks;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class DropItemManager : MonoBehaviour
{
    [SerializeField] private int _poolSize = 100;
    [SerializeField] private string dropItemPrefabPath;
    public static DropItemManager Inst { get; private set; }

    private List<DropItem> _dropItemPool = new List<DropItem>();
    private int _currentPivot = 0;
    private Vector3 _dropPosition;

    private DropTableData _dropTableData;
    private DropItem _dropItemToDrop = null;
    private string _dropTableId;
   
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        AsyncDropItemPool().Forget();
    }
    
    private async UniTaskVoid AsyncDropItemPool()
    {
        
        for (int i = 0; i < _poolSize; i++)
        {
            GameObject dropItemResource = await ResourceManager.Inst.InstantiateAsync(dropItemPrefabPath, transform);//리소스매니저에게 어드레서블을 주고 오브젝트를 받아옴

            if (dropItemResource != null)
            {
                DropItem dropItem = dropItemResource.GetComponent<DropItem>();
                dropItem.gameObject.SetActive(false);
                _dropItemPool.Add(dropItem);
            }
        }
        _currentPivot = 0;
    }
    
    public void DropItemInField(Vector3 dropPosition, string dropTableId)
    {
        if (string.IsNullOrEmpty(dropTableId)) return;

        _dropPosition = dropPosition;
        _dropTableId = dropTableId;
        _dropTableData = DataManager.Inst.GetDropTableData(_dropTableId);

        if (_dropTableData == null)
        {
            Debug.LogWarning($"[DropError] '{dropTableId}'에 해당하는 드롭 테이블 데이터가 존재하지 않습니다.");
            return;
        }

        CheckDropItemAndSpawn();

        _dropTableData = null;
        _dropItemToDrop = null;
        _dropTableId = null;

    }
    public void GetItemInPool() {
        _dropItemToDrop = null;
        
        for (int i = 0; i < _dropItemPool.Count; i++)
        {
            int checkIndex = (_currentPivot + i) % _dropItemPool.Count;
            if (!_dropItemPool[checkIndex].gameObject.activeSelf)
            {
                _dropItemToDrop = _dropItemPool[checkIndex];
                _currentPivot = (checkIndex + 1) % _dropItemPool.Count;
                break;
            }
        }
    }
    public void CheckDropItemAndSpawn() {
        int minCion= _dropTableData.CoinMinAMount;
        int maxCion= _dropTableData.CoinMaxAMount;
        int goldAmount = Random.Range(minCion, maxCion+1);

        if (goldAmount > 0)
        {
            GetItemInPool();
            SpawnDropItemCoin(goldAmount);
        }

        string dropItemIdToSpawn;
        if (!string.IsNullOrEmpty(_dropTableData.DropItemId1))
        {
            if (GetItmeTrueOrFalse(_dropTableData.Item1DropPercent))
            {
                dropItemIdToSpawn = _dropTableData.DropItemId1;
                GetItemInPool();
                SpawnDropItem(dropItemIdToSpawn);
            }
        }
        if (!string.IsNullOrEmpty(_dropTableData.DropItemId2)) 
        {
            if (GetItmeTrueOrFalse(_dropTableData.Item2DropPercent))
            {
                dropItemIdToSpawn = _dropTableData.DropItemId2;
                GetItemInPool();
                SpawnDropItem(dropItemIdToSpawn);
            }
        }
        if (!string.IsNullOrEmpty(_dropTableData.DropItemId3)) 
        {
            if (GetItmeTrueOrFalse(_dropTableData.Item3DropPercent))
            {
                dropItemIdToSpawn = _dropTableData.DropItemId3;
                GetItemInPool();
                SpawnDropItem(dropItemIdToSpawn);
            }
        }

    }
    public void SpawnDropItem(string dropItemId) 
    {
        if (_dropItemToDrop != null)
        {
            ItemData _itemData = DataManager.Inst.GetItemData(dropItemId);

            if (_itemData == null)
            {
                Debug.LogError($"[DropError] 데이터 매니저에 '{dropItemId}'에 대한 정보가 없습니다! ");
                return;
            }

            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);//아이템을 흩뿌리는 효과
            _dropItemToDrop.transform.position = _dropPosition + randomOffset;

            _dropItemToDrop.InitDroppedItemAsync(_itemData).Forget();
            _dropItemToDrop.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("드랍템 풀이 가득 찼습니다!");
        }
    }
    public void SpawnDropItemCoin(int goldAmount)
    {
        if (_dropItemToDrop != null)
        {
            ItemData _itemData = DataManager.Inst.GetItemData("item_coin_01");

            if (_itemData == null)
            {
                Debug.LogError($"[DropError] 데이터 매니저에 '{"item_coin_01"}'에 대한 정보가 없습니다! ");
                return;
            }

            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f,1f), 0);//아이템을 흩뿌리는 효과
            _dropItemToDrop.transform.position = _dropPosition + randomOffset;

            _dropItemToDrop.InitDroppedItemAsync(_itemData, goldAmount).Forget();
            _dropItemToDrop.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("드랍템 풀이 가득 찼습니다!");
        }
    }
    public bool GetItmeTrueOrFalse(int percent)
    {
        if (Random.Range(0, 100) < percent)
        { return true; }
        else
        { return false; }
    }
}
