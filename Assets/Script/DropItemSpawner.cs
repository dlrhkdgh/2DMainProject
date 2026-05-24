using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class DropItemSpawner : MonoBehaviour
{
    [SerializeField] private int _poolSize = 100;
    [SerializeField] private string dropItemPrefabPath;

    private List<DropItem> _dropItemPool = new List<DropItem>();
    private int _currentPivot = 0;
    
    private void OnDisable()
    {
        foreach (var dropItem in _dropItemPool)
        {
            if (dropItem != null && dropItem.gameObject.activeSelf)
            {
                dropItem.gameObject.SetActive(false);
            }
        }
    }
    private async UniTaskVoid AsyncDropItemPool()
    {
        if (_dropItemPool.Count == 0)
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
    }
    
    public void DropItemInField(Vector3 dropPosition, string dropTableId)
    {
        if (string.IsNullOrEmpty(dropTableId)) return;
       
        DropTableData dropTableData = DataManager.Inst.GetDropTableData(dropTableId);

        if (dropTableData == null)
        {
            Debug.LogWarning($"[DropError] '{dropTableId}'에 해당하는 드롭 테이블 데이터가 존재하지 않습니다.");
            return;
        }
        CheckDropItemAndSpawn(dropPosition, dropTableData);
        
    }
    public DropItem GetItemFromPool() {
       
        for (int i = 0; i < _dropItemPool.Count; i++)
        {
            int checkIndex = (_currentPivot + i) % _dropItemPool.Count;
            if (!_dropItemPool[checkIndex].gameObject.activeSelf)
            {
                _currentPivot = (checkIndex + 1) % _dropItemPool.Count;
                return _dropItemPool[checkIndex];
            }
        }
        return null;
    }
    public void CheckDropItemAndSpawn(Vector3 dropPosition, DropTableData tableData)
    {
        int minCion= tableData.CoinMinAMount;
        int maxCion= tableData.CoinMaxAMount;
        int goldAmount = Random.Range(minCion, maxCion+1);

        if (goldAmount > 0)
        {
            DropItem coinItem = GetItemFromPool();
            SpawnDropItemCoin(coinItem, dropPosition, goldAmount);
        }

          
        if (!string.IsNullOrEmpty(tableData.DropItemId1) && GetItemTrueOrFalse(tableData.Item1DropPercent))
        {
            DropItem targetItem = GetItemFromPool();
            SpawnDropItem(targetItem, dropPosition, tableData.DropItemId1);
        }
        if (!string.IsNullOrEmpty(tableData.DropItemId2) && GetItemTrueOrFalse(tableData.Item2DropPercent))
        {
            DropItem targetItem = GetItemFromPool();
            SpawnDropItem(targetItem, dropPosition, tableData.DropItemId2);
        }

        if (!string.IsNullOrEmpty(tableData.DropItemId3) && GetItemTrueOrFalse(tableData.Item3DropPercent))
        {
            DropItem targetItem = GetItemFromPool();
            SpawnDropItem(targetItem, dropPosition, tableData.DropItemId3);
        }
        
    }
    public void SpawnDropItem(DropItem dropItemObj, Vector3 dropPosition, string dropItemId)
    {
        if (dropItemObj == null)
        {
            Debug.LogWarning("드랍템 풀이 가득 찼습니다!");
            return;
        }

        ItemData itemData = DataManager.Inst.GetItemData(dropItemId);
        if (itemData == null)
        {
            Debug.LogError($"[DropError] 데이터 매니저에 '{dropItemId}'에 대한 정보가 없습니다! ");
            return;
        }

        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        dropItemObj.transform.position = dropPosition + randomOffset;

        dropItemObj.InitDroppedItemAsync(itemData).Forget();
        dropItemObj.gameObject.SetActive(true);
    }

    public void SpawnDropItemCoin(DropItem dropItemObj, Vector3 dropPosition, int goldAmount)
    {
        if (dropItemObj == null)
        {
            Debug.LogWarning("드랍템 풀이 가득 찼습니다!");
            return;
        }

        ItemData itemData = DataManager.Inst.GetItemData("item_coin_01");
        if (itemData == null)
        {
            Debug.LogError($"[DropError] 데이터 매니저에 'item_coin_01'에 대한 정보가 없습니다! ");
            return;
        }
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
        dropItemObj.transform.position = dropPosition + randomOffset;

        dropItemObj.InitDroppedItemAsync(itemData, goldAmount).Forget();
        dropItemObj.gameObject.SetActive(true);
    }

    public bool GetItemTrueOrFalse(int percent)
    {
        if (Random.Range(0, 100) < percent)
        { return true; }
        else
        { return false; }
    }
    public void InitDropItemSpawner() {

        AsyncDropItemPool().Forget();
    }
    public void ClearAndReleaseSpawner()
    {
        for (int i = 0; i < _dropItemPool.Count; i++)
        {

            if (_dropItemPool[i] != null)
            {
                Addressables.ReleaseInstance(_dropItemPool[i].gameObject);
            }
        }
        _dropItemPool.Clear();
        _currentPivot = 0;
    }
}
